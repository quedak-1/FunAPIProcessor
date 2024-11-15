using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ScaleThreadProcess
{
    internal class Program
    {
        /// <summary>
        /// Main Program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string returnMessage = string.Empty;
            bool isOkToExecute = false;

            //
            // Batch mode
            // - Profile received as the first argument
            //////////////////////////////////////////////////////////////////////////////////////////
            if (args.Length == 1 && args[0].ToString().IndexOf("trace") == -1)
            {
                //
                // Receiving profile as an argument
                // Decode and decompress the Base64 encoded data
                // Deserialize the JSON data

                string base64Data = args[0];
                byte[] compressedData = Convert.FromBase64String(base64Data);
                string jsonData = CommonHelper.Decompress(compressedData);

                ProcessProfile profile = JsonConvert.DeserializeObject<ProcessProfile>(jsonData);

                StandardOutput($"[+] Run ScaleThread Process [{profile.ProcessName}] | ver {Assembly.GetExecutingAssembly().GetName().Version.ToString()}", profile.LogLevel, 10);
                if (profile.RunAccountId != null)
                    StandardOutput($"- Run-Time user: {profile.RunAccountId.ToUpper() ?? "Not Required"}", profile.LogLevel, 10);

                if (profile.Procedure != null && profile.ProcedureMethod != null)
                    StandardOutput($"- Run method: {profile.Procedure} => {profile.ProcedureMethod}", profile.LogLevel, 10);

                CommonHelper.serviceAccountId = profile.RunAccountId;
                CommonHelper.serviceAccountKey = profile.RunAccountKey;

                //
                // Enable Trace logging
                if (CommonHelper.enableTraceLog || profile.LogTraceToFile)
                {
                    string logFilePath = $@"{profile.TraceLogFilePath}\ScaleThreadProcess_{profile.ProcessId}_{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}.log";
                    TextWriterTraceListener textListerner = new TextWriterTraceListener(logFilePath);
                    Trace.Listeners.Add(textListerner);

                    StandardOutput($"Enable tracing to {logFilePath}", profile.LogLevel, 10);
                }

                //
                // Run process profile
                StandardOutput($"- Executing process...", profile.LogLevel, 10);
                returnMessage = RunProcess(profile);

                if (String.IsNullOrEmpty(returnMessage))
                {
                    StandardOutput($"- Process completed", profile.LogLevel, 10);
                }
                else
                    StandardOutput($"{returnMessage}");
            }

            //
            // Development/Debug mode
            // - Load the profile from a file
            //////////////////////////////////////////////////////////////////////////////////////////
            else
            {
                CommonHelper.serviceAccountId = null;
                CommonHelper.serviceAccountKey = null;

                // Dictionary to store parameter names and values (case-insensitive keys)
                var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                // Process the input arguments and populate the dictionary
                foreach (var arg in args)
                {
                    if (arg.StartsWith("/") && arg.Contains(":"))
                    {
                        var parts = arg.Substring(1).Split(new[] { ':' }, 2);
                        if (parts.Length == 2 && !parameters.ContainsKey(parts[0]))
                        {
                            parameters[parts[0]] = parts[1];
                        }
                    }
                }

                // Debug mode - trace logging will be enabled for the application
                CommonHelper.enableTraceLog = GetParameterValue(parameters, "trace").IndexOf("true") != -1;

                // Default process profile path override
                CommonHelper.processProfilePath = GetParameterValue(parameters, "profilePath");

                // run specific profile only
                CommonHelper.runSpecificProcessId = GetParameterValue(parameters, "profileId");


                List<ProcessProfile> profiles;
                string profileFileName = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\" + "ProcessProfile.json";

                if (File.Exists(profileFileName))
                {
                    string json = File.ReadAllText(profileFileName);
                    profiles = JsonConvert.DeserializeObject<List<ProcessProfile>>(json);

                    StandardOutput($"[+] Load process profiles");

                    foreach (var profile in profiles)
                    {
                        if (profile.ProcessActiveIndicator)
                            StandardOutput($"- load: [ {profile.ProcessName} ] process profile");
                    }

                    StandardOutput($"[+] Start processes");

                    foreach (var profile in profiles)
                    {
                        if (profile.AutoStart & profile.ProcessActiveIndicator)
                        {
                            StandardOutput($"[+] Run Process [{profile.ProcessName}] | ver {Assembly.GetExecutingAssembly().GetName().Version.ToString()}", profile.LogLevel, 10);
                            if (profile.RunAccountId != null)
                                StandardOutput($"- Run-Time user: {profile.RunAccountId.ToUpper()}", profile.LogLevel, 10);

                            if (profile.Procedure != null && profile.ProcedureMethod != null)
                                StandardOutput($"- Run method: {profile.Procedure} => {profile.ProcedureMethod}", profile.LogLevel, 10);

                            //
                            // Enable Trace logging
                            if (CommonHelper.enableTraceLog || profile.LogTraceToFile)
                            {
                                string logFilePath = $@"{profile.TraceLogFilePath}\ScaleThreadProcess_{profile.ProcessId}_{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}.log";
                                TextWriterTraceListener textListerner = new TextWriterTraceListener(logFilePath);
                                Trace.Listeners.Add(textListerner);

                                StandardOutput($"- enable tracing to {logFilePath}", profile.LogLevel, 10);
                            }

                            if (!String.IsNullOrEmpty(profile.CopyDependancyRepoPath))
                            {
                                if (!String.IsNullOrEmpty(profile.RunTimePath))
                                {
                                    //string deploymentTarget = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                                    string deploymentTarget = profile.RunTimePath;
                                    var (IsError, errorMessage) = CommonHelper.CopyDirectory(profile.CopyDependancyRepoPath, deploymentTarget);

                                    isOkToExecute = !IsError;
                                }
                            }
                            else
                            {
                                isOkToExecute = true;
                            }

                            if (profile.AutoStart)
                            {
                                returnMessage = RunProcess(profile);

                                if (String.IsNullOrEmpty(returnMessage))
                                {
                                    StandardOutput($"- worker completed the process.");
                                }
                                else
                                    StandardOutput($"- {returnMessage}");
                            }
                        }
                    }
                }
                else
                {
                    StandardOutput($"[+] ERROR: Profile config file not found: {profileFileName}");
                }
            }
        }

        private static void StandardOutput(string message, int logLevel = 0, int thisLogLevel = 0)
        {
            if (logLevel >= thisLogLevel)
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Run the procedure defined in the profile
        ///  - the method will be located in the current Namespace, activated and executed
        /// </summary>
        /// <param name="profile"></param>
        public static string RunProcess(ProcessProfile profile)
        {
            string methodName = profile.ProcedureMethod;
            bool isExecuted = false;

            // Get the current executing assembly (where the methods are defined)
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            // Search through all types in the current assembly
            foreach (Type type in currentAssembly.GetTypes())
            {
                // Check if the type belongs to the same namespace
                if (type.Namespace == typeof(Program).Namespace)
                {
                    // Look for the method within the type
                    MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                    // If method is found, execute it
                    if (methodInfo != null)
                    {
                        try
                        {

                            // Setup Email Service overrides
                            CommonHelper.smtpServer = profile.SmtpServer;
                            CommonHelper.emailServerType = profile.EmailServerType;
                            CommonHelper.fromContactEmail = profile.FromContactEmail;

                            object[] parameters = new object[] { profile };

                            // Check if the method is static or not
                            if (methodInfo.IsStatic)
                            {
                                // If the method is static, you don't need an instance
                                methodInfo.Invoke(null, parameters);
                            }
                            else
                            {

                                // If the method is not static, create an instance of the class
                                object instance = Activator.CreateInstance(type);
                                methodInfo.Invoke(instance, parameters); // Assuming no parameters
                                isExecuted = true;

                                if (!String.IsNullOrEmpty(profile.HeartbeatFileName))
                                {
                                    try
                                    {
                                        // The process profile is setup to report its activity 
                                        using (var heartBeatFile = File.Create(profile.HeartbeatFileName)) { }

                                        using (var sw = new StreamWriter(profile.HeartbeatFileName, true))
                                        {
                                            sw.WriteLine($"[{DateTime.Now}] > Process [{profile.ProcessName}] running on [{System.Environment.MachineName}]");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        StandardOutput($"ERROR: Failed to create process activity file marker: [{profile.HeartbeatFileName}] Error: [{ex.Message}]");
                                    }
                                }
                            }

                            break;
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    }
                }
            }

            if (isExecuted) return String.Empty; else return "ERROR: The process method not found.";
        }

        /// <summary>
        /// Function to get the parameter value (default to empty string if not found) 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        static string GetParameterValue(Dictionary<string, string> parameters, string name) =>
            parameters.TryGetValue(name, out var value) ? value : string.Empty;
    }
}