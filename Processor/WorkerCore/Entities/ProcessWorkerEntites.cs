using System.ComponentModel;

namespace ScaleThreadProcess
{
    public enum NotifyMethod
    {
        [Description("None (disabled)")]
        None = 1,

        [Description("Email (html)")]
        emailHtml = 1,

        [Description("Email (text)")]
        emailText = 2,

        [Description("API")]
        API = 3
    }
    public enum WorkerSettingAttribute
    {
        DEAFULT_SUPPORT_CONTACT,
        DEAFULT_SUPPORT_BCC_CONTACT,
        DEAFULT_SUPPORT_DEBUG_CONTACT,
        DISABLE_EMAILER,
        SMTP_EMAIL_SERVER,
        SMTP_EMAIL_FROM_CONTACT,
        MARKET_CLOSED_DAYS,
        DEPLOYMENT_REPO_PATH
    }
    public enum EnvironmentDbType
    {
        PROD,
        QA,
        TEST,
        DEV
    }
    public enum WorkerDbType
    {
        [Description("Database Type MSSQL")]
        MSSQL,
        [Description("Database Type Oracle")]
        ORA
    }
    public enum EmailTypes
    {
        email_html,
        email_text
    }
    public enum WorkerSettingSection
    {
        SCHEDULE,
        EMAIL,
        DEPLOYMENT
    }
    public enum ScheduleItemType
    {
        All,
        M2F,
        Skip,
        SkipMarketClosedDays,
        SkipHolidays,
        Mon,
        Tue,
        Wed,
        Thu,
        Fri,
        Sat,
        Sun,
    }
    public enum MonitorScheduleType
    {
        schedule_fixed,
        schedule_interval,
    }
    public enum EventState
    {
        [Description("Event Processed")]
        event_processed = 1,
        [Description("Event New")]
        event_new = 0
    }
}
