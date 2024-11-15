using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace ScaleThreadProcess
{
    public class CommonHelper
    {
        public static bool enableTraceLog = false;

        public static String envOverride;
        public static bool emailerEnabled = true;
        public static string RegistryPath = @"Software\NFA\ScaleThread";

        public static string TraceLogPath = @"c:\temp";
        public static string processProfilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}";
        public static string processProfileFileName = "ProcessProfile.json";
        public static string runSpecificProcessId = string.Empty;

        public static bool isImporsonateAccount;
        public static string serviceAccountId;
        public static string serviceAccountKey;
        public static string connectionString;
        public static string connectionType;

        public static string emailServerType;
        public static string smtpServer;
        public static string fromContactEmail;

        /// <summary>
        /// Decompress string content
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decompress(byte[] data)
        {
            using (var inputStream = new MemoryStream(data))
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                gzipStream.CopyTo(outputStream);
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }
        }

        /// <summary>
        /// Compress string content
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(string data)
        {
            byte[] rawData = Encoding.UTF8.GetBytes(data);
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(rawData, 0, rawData.Length);
                }
                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Returns Previous Quarter Date Range based on the date passed
        /// </summary>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static (DateTime startDate, DateTime endDate) GetPreviousQuarterDateRange_(DateTime inputDate)
        {
            int year = inputDate.Year;
            int previousQuarter;
            int previousQuarterYear = year;

            // Determine the quarter of the input date
            int quarter = (inputDate.Month - 1) / 3 + 1;

            // Determine the previous quarter and adjust year if necessary
            if (quarter == 1)
            {
                previousQuarter = 4;
                previousQuarterYear = year - 1;
            }
            else
            {
                previousQuarter = quarter - 1;
            }

            // Determine the start and end dates of the previous quarter
            DateTime startDate;
            DateTime endDate;

            switch (previousQuarter)
            {
                case 1:
                    startDate = new DateTime(previousQuarterYear, 1, 1);
                    endDate = new DateTime(previousQuarterYear, 3, 31);
                    break;
                case 2:
                    startDate = new DateTime(previousQuarterYear, 4, 1);
                    endDate = new DateTime(previousQuarterYear, 6, 30);
                    break;
                case 3:
                    startDate = new DateTime(previousQuarterYear, 7, 1);
                    endDate = new DateTime(previousQuarterYear, 9, 30);
                    break;
                case 4:
                    startDate = new DateTime(previousQuarterYear, 10, 1);
                    endDate = new DateTime(previousQuarterYear, 12, 31);
                    break;
                default:
                    throw new InvalidOperationException("Invalid quarter");
            }

            // Return the date range as a struct
            return (startDate, endDate);
        }

        /// <summary>
        /// Get appliaction Environment
        /// </summary>
        /// <param name="envMasterOverride"></param>
        /// <returns></returns>
        public static string GetEnvironment(string envMasterOverride = "")
        {

            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string localPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string envOverride = System.Configuration.ConfigurationManager.AppSettings["EnvOverride"];

            if (!String.IsNullOrEmpty(envMasterOverride)) envOverride = envMasterOverride;

            //
            // 
            // The production mode will allow certain users to run the monitor in the Viewer mode from their laptops
            // and view the states of the processes in the production
            // the production mode

            if (CommonHelper.envOverride != null) return CommonHelper.envOverride;

            if (envOverride != "") // Custom
            {
                return envOverride;
            }

            if (localPath.ToUpper().IndexOf("NFAPROD") > 0) //Prod 
            {
                return "PROD";
            }

            if (localPath.ToUpper().IndexOf("NFATESTDEV") > 0)//Test
            {
                return "TEST";
            }
            return "DEV"; //Dev
        }

        /// <summary>
        /// Read a setting from an external .INI file
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public string GetWorkerSettingAttribute(WorkerSettingSection sectionName, WorkerSettingAttribute settingName)
        {
            string applPathFile = $@"{Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}\ScaleThread.ini";
            string settingValue;

            if (!File.Exists(applPathFile))
            {
                // create a new file if it does not exist
                using (var iniFile = File.Create(applPathFile)) { }

                IniParser parserINI = new IniParser(applPathFile);
                // some processes will be tried several times before an error will be reported

                parserINI.AddSetting(WorkerSettingSection.EMAIL.ToString(), WorkerSettingAttribute.DEAFULT_SUPPORT_DEBUG_CONTACT.ToString(), @"");
                parserINI.AddSetting(WorkerSettingSection.EMAIL.ToString(), WorkerSettingAttribute.DEAFULT_SUPPORT_CONTACT.ToString(), @"gsosnovsky@nfa.furures.org");
                parserINI.AddSetting(WorkerSettingSection.EMAIL.ToString(), WorkerSettingAttribute.DEAFULT_SUPPORT_BCC_CONTACT.ToString(), @"");
                parserINI.AddSetting(WorkerSettingSection.EMAIL.ToString(), WorkerSettingAttribute.DISABLE_EMAILER.ToString(), @"yes");
                parserINI.AddSetting(WorkerSettingSection.EMAIL.ToString(), WorkerSettingAttribute.SMTP_EMAIL_SERVER.ToString(), @"email.futures.org");
                parserINI.AddSetting(WorkerSettingSection.EMAIL.ToString(), WorkerSettingAttribute.SMTP_EMAIL_FROM_CONTACT.ToString(), @"NFAN@NFA.Futures.Org");
                parserINI.AddSetting(WorkerSettingSection.DEPLOYMENT.ToString(), WorkerSettingAttribute.DEPLOYMENT_REPO_PATH.ToString(), @"\\nfatestdev01\appldbs\ScaleThread\Deploy_DLL");
                parserINI.SaveSettings(applPathFile);
            }

            IniParser parser = new IniParser(applPathFile);

            settingValue = parser.GetSetting(sectionName.ToString(), settingName.ToString());
            return settingValue;

        }

        #region Installation Helper
        /// <summary>
        /// Create application shortcuts
        /// </summary>
        public static void CreateShortCuts()
        {
            string exeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeFolderLocation = System.IO.Path.GetDirectoryName(@exeFilePath);

            IShellLink link = (IShellLink)new ShellLink();
            IPersistFile file = (IPersistFile)link;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            // setup shortcut information
            link.SetDescription("ScaleThread");
            //link.SetArguments("master");
            link.SetWorkingDirectory(@exeFolderLocation);
            link.SetPath(@exeFilePath);

            // save it
            file.Save(Path.Combine(desktopPath, "ScaleThread.lnk"), false);

            // setup shortcut information
            link.SetDescription("ScaleThread Debug");
            link.SetArguments("trace");
            link.SetWorkingDirectory(@exeFolderLocation);
            link.SetPath(@exeFilePath);

            // save it
            file.Save(Path.Combine(desktopPath, "ScaleThread Debug.lnk"), false);
        }

        // Internal class to create shortcuts for the application
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }
        #endregion

        #region EncryptionHelper

        /// <summary>
        /// Setting the passphrase
        /// </summary>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        private static byte[] GetKey(string passphrase)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
            }
        }

        /// <summary>
        /// Encrypt plain test (with passphrase)
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string passphrase)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey(passphrase);
                aes.IV = new byte[16]; // Use a zero IV (not recommended for real-world applications)

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(encrypted);
            }
        }

        /// <summary>
        /// Decrypt plain test (with passphrase)
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText, string passphrase)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey(passphrase);
                aes.IV = new byte[16]; // Use the same IV as encryption (zero IV)

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] decrypted = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return Encoding.UTF8.GetString(decrypted);
            }
        }

        #endregion

        #region RegistryHelper

        /// <summary>
        /// Save Login Credentials
        /// </summary>
        /// <param name="serviceAccountId"></param>
        /// <param name="serviceAccountKey"></param>
        public static (string errorMessage, string errorSource, bool isError) SaveCredentials(string serviceAccountId, string serviceAccountKey, int processId = 0)
        {
            try
            {
                string machineName = Environment.MachineName;
                string encryptedserviceAccountId = Encrypt(serviceAccountId, machineName);
                string encryptedserviceAccountKey = Encrypt(serviceAccountKey, machineName);

                RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath + $"\\Process{processId}");
                key.SetValue("01", encryptedserviceAccountId);
                key.SetValue("02", encryptedserviceAccountKey);
                key.Close();

                return ("", "", false);
            }
            catch (Exception ex)
            {
                return (ex.Message, ex.Source, true);
            }
        }

        /// <summary>
        /// Read Login Credentials
        /// </summary>
        /// <returns></returns>
        public static (string serviceAccountId, string serviceAccountKey) ReadCredentials(int processId = 0)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath + $"\\Process{processId}");

            if (key != null)
            {
                string machineName = Environment.MachineName;
                string encryptedserviceAccountId = key.GetValue("01")?.ToString();
                string encryptedserviceAccountKey = key.GetValue("02")?.ToString();

                if (!string.IsNullOrEmpty(encryptedserviceAccountId) && !string.IsNullOrEmpty(encryptedserviceAccountKey))
                {
                    try
                    {
                        string serviceAccountId = Decrypt(encryptedserviceAccountId, machineName);
                        string serviceAccountKey = Decrypt(encryptedserviceAccountKey, machineName);

                        return (serviceAccountId, serviceAccountKey);
                    }
                    catch (Exception)
                    {
                        return (null, null);
                    }
                }

                key.Close();
            }

            return (null, null);
        }

        /// <summary>
        /// Delete Credentials
        /// </summary>
        /// <param name="processId"></param>
        public static void ClearCredentials(int processId = 0)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath + $"\\Process{processId}", writable: true);
            if (key != null)
            {
                key.DeleteValue("01", false);
                key.DeleteValue("02", false);
                key.Close();
            }

        }
        #endregion

        #region Impersonation Helper

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
        int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        public static bool RunImpersonated(string domain, string username, string password, Action action)
        {
            IntPtr tokenHandle = IntPtr.Zero;

            bool success = LogonUser(username, domain, password,
                2, // LOGON32_LOGON_INTERACTIVE
                0, // LOGON32_PROVIDER_DEFAULT
                out tokenHandle);

            if (!success)
            {
                //throw new UnauthorizedAccessException("Logon failed.");
            }

            if (success)
            {
                using (WindowsIdentity identity = new WindowsIdentity(tokenHandle))
                {
                    WindowsIdentity.RunImpersonated(identity.AccessToken, () =>
                    {
                        action();
                    });
                }

                CloseHandle(tokenHandle);
            }

            return success;
        }

        #endregion

        #region File Tools

        /// <summary>
        /// Smart folder copy (with sub-directories)
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destinationDir"></param>
        /// <param name="copySubDirs"></param>
        /// <returns></returns>
        public static (bool isError, string errorMessage) CopyDirectory(string sourceDir, string destinationDir, bool copySubDirs = true)
        {
            bool isError = false;
            string errorMessage = String.Empty;

            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(sourceDir);

                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDir}");
                }

                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destinationDir, file.Name);

                    if (!File.Exists(tempPath))
                    {
                        file.CopyTo(tempPath, true);
                    }
                    else
                    {
                        FileInfo destFile = new FileInfo(tempPath);

                        if (file.LastWriteTime != destFile.LastWriteTime)
                        {
                            file.CopyTo(tempPath, true);  // true to overwrite existing files
                        }
                    }
                }

                // If copying subdirectories, copy them and their contents to new location.

                if (copySubDirs)
                {
                    DirectoryInfo[] subDirs = dir.GetDirectories();
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        string tempPath = Path.Combine(destinationDir, subDir.Name);
                        CopyDirectory(subDir.FullName, tempPath, copySubDirs);
                    }
                }
            }
            catch (Exception ex)
            {
                isError = true;
                errorMessage = ex.Message;
                Trace.WriteLine($"[{DateTime.Now}] > Error: {errorMessage}");
                Trace.Flush();
            }

            return (isError, errorMessage);
        }

        static string CalcFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                }
            }
        }

        #endregion
    }
}
