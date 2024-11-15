using System;


namespace ScaleThreadProcess
{
    public class ProcessProfile : IDisposable
    {
        public int ProcessId { get; set; }
        public bool ProcessActiveIndicator { get; set; }
        public string ProcessName { get; set; }
        public string SupportContact { get; set; }

        public string EmailServerType { get; set; }
        public string SmtpServer { get; set; }
        public string FromContactEmail { get; set; }

        public string WorkerUIMode { get; set; }
        public string Procedure { get; set; }
        public string ProcedureType { get; set; }
        public string ProcedureMethod { get; set; }
        public string ProcedureParameters { get; set; }
        public bool UseShellExecute { get; set; }
        public bool CreateNoWindow { get; set; }
        public bool RedirectStandardOutput { get; set; }
        public bool RedirectStandardInput { get; set; }
        public bool RedirectStandardError { get; set; }

        public TimeSpan Interval { get; set; }
        public bool LogTraceToFile { get; set; }
        public string TraceLogFilePath { get; set; }
        public bool OutputToFile { get; set; }
        public string OutputFullFilePath { get; set; }
        public int LogLevel { get; set; }
        public bool AutoStart { get; set; }
        public string ScheduleConfig { get; set; }
        public string AuthenticationType { get; set; }
        public bool SaveCredientials { get; set; }
        public string DbConnectionType { get; set; }
        public string DbConnectionString { get; set; }
        public string RunAccountId { get; set; }
        public string RunAccountKey { get; set; }
        public string ReferencePath { get; set; }
        public string LicensePath { get; set; }
        public string CopyDependancyRepoPath { get; set; }
        public string ExternalComponentPath { get; set; }
        public string LinkDLLPath { get; set; }
        public string RunTimePath { get; set; }
        public string HeartbeatFileName { get; set; }

        public ProcessProfile(
                  int processId
                , bool processActiveIndicator
                , string processName
                , string supportContact

                , string emailServerType
                , string smtpServer
                , string fromContactEmail

                , string workerUIMode

                , string procedure
                , string procedureType
                , string procedureMethod
                , string procedureParameters
                , bool useShellExecute
                , bool createNoWindow
                , bool redirectStandardOutput
                , bool redirectStandardInput
                , bool redirectStandardError

                , TimeSpan interval
                , bool autoStart
                , bool logTraceToFile
                , string traceLogFilePath
                , bool outputToFile
                , string outputFullFilePath
                , int logLevel
                , string scheduleConfig
                , string heartbeatFileName
                , string authenticationType
                , bool saveCredientials
                , string dbConnectionType
                , string dbConnectionString
                , string referencePath
                , string licensePath
                , string copyDependancyRepoPath
                , string externalComponentPath
                , string linkDLLPath
                , string runTimePath
            )
        {
            ProcessId = processId;
            ProcessName = processName;
            ProcessName = processName;
            SupportContact = supportContact;

            EmailServerType = emailServerType;
            SmtpServer = smtpServer;
            FromContactEmail = fromContactEmail;

            WorkerUIMode = workerUIMode;

            Procedure = procedure;
            ProcedureType = procedureType;
            ProcedureMethod = procedureMethod;
            ProcedureParameters = procedureParameters;
            UseShellExecute = useShellExecute;
            CreateNoWindow = createNoWindow;
            RedirectStandardOutput = redirectStandardOutput;
            RedirectStandardInput = redirectStandardInput;
            RedirectStandardError = redirectStandardError;

            Interval = interval;
            AutoStart = autoStart;
            LogTraceToFile = logTraceToFile;
            TraceLogFilePath = traceLogFilePath;
            OutputToFile = outputToFile;
            OutputFullFilePath = outputFullFilePath;
            LogLevel = logLevel;
            ScheduleConfig = scheduleConfig;
            HeartbeatFileName = heartbeatFileName;
            AuthenticationType = authenticationType;
            SaveCredientials = saveCredientials;
            DbConnectionString = dbConnectionString;
            DbConnectionType = dbConnectionType;
            ProcessActiveIndicator = processActiveIndicator;
            ReferencePath = referencePath;
            LicensePath = licensePath;
            CopyDependancyRepoPath = copyDependancyRepoPath;
            ExternalComponentPath = externalComponentPath;
            LinkDLLPath = linkDLLPath;
            RunTimePath = runTimePath;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
