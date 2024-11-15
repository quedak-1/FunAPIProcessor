using ScaleThreadProcess.Data;
using System;
using System.Collections.Generic;

namespace ScaleThreadProcess
{
    public class EmailNotification : IDisposable
    {
        public long EventId { get; set; }  // Unique identifier for the process
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactCC { get; set; }
        public string ContactBCC { get; set; }
        public int EmailType { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailFooter { get; set; }
        public string EmailBodyTemplate { get; set; }
        public string EmailSubjectTemplate { get; set; }

        /// <summary>
        /// Long Constructor
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="contactName"></param>
        /// <param name="contactEmail"></param>
        /// <param name="contactCC"></param>
        /// <param name="contactBCC"></param>
        /// <param name="emailType"></param>
        /// <param name="emailSubject"></param>
        /// <param name="emailBody"></param>
        /// <param name="emailBodyTemplate"></param>
        /// <param name="emailSubjectTemplate"></param>
        public EmailNotification(long eventId
                            , string contactName
                            , string contactEmail
                            , string contactCC
                            , string contactBCC
                            , int emailType
                            , string emailSubject
                            , string emailBody
                            , string emailBodyTemplate
                            , string emailSubjectTemplate)
        {
            ContactName = contactName;
            ContactEmail = contactEmail;
            ContactCC = contactCC;
            ContactBCC = contactBCC;
            EmailType = emailType;
            EmailSubject = emailSubject;
            EmailBody = emailBody;
            EmailBodyTemplate = emailBodyTemplate;
            EmailSubjectTemplate = emailSubjectTemplate;
        }

        /// <summary>
        /// Short Contructor
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="contactName"></param>
        /// <param name="contactEmail"></param>
        /// <param name="emailType"></param>
        /// <param name="emailBodyTemplate"></param>
        public EmailNotification(long eventId
                            , string contactName
                            , string contactEmail
                            , string contactCC
                            , string contactBCC
                            , int emailType
                            , string emailSubjectTemplate
                            , string emailBodyTemplate)
        {
            ContactName = contactName;
            ContactEmail = contactEmail;
            ContactCC = contactCC;
            ContactBCC = contactBCC;
            EmailType = emailType;
            EmailSubjectTemplate = emailSubjectTemplate;
            EmailBodyTemplate = emailBodyTemplate;
        }

        /// <summary>
        /// Prepare notification - replace tags with values
        /// </summary>
        /// <returns></returns>
        public bool PrepareNotification(List<Tuple<string, string>> tagValuePair, string connectionKey = null)
        {

            // Replace tags with values in the text template
            EmailBody = EmailBodyTemplate;
            EmailSubject = EmailSubjectTemplate;

            foreach (var pair in tagValuePair)
            {
                EmailBody = EmailBody.Replace(pair.Item1, pair.Item2);
                EmailSubject = EmailSubject.Replace(pair.Item1, pair.Item2);
            }

            if (connectionKey != null)
            {
                DataClass dataClass = new DataClass();
                dataClass.SQL_ExecuteNonQuery($"update [HR].[HR_PROHIBITED_TRADE_NOTIFY_EVENT] " +
                                                 $"set [EVENT_MESSAGE_TITLE] = '{EmailSubject}'" +
                                                 $"  , [EVENT_MESSAGE_BODY] = '{EmailBody}'" +
                                                 $"  , [EVENT_MESSAGE_FOOTER] = '{EmailFooter}'" +
                                                 $"  , [NOTIFY_CONTACT_EMAIL] = '{ContactEmail}'" +
                                              $" where [NOTIFY_EVENT_ID] = {EventId}", connectionKey);
            }

            return true;
        }

        /// <summary>
        /// Send Error Report
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="profile"></param>
        public static void SendErrorReport(string errorMessage, ProcessProfile profile)
        {
            // Prepare the report for emailing
            string emailSubject = $"*** ERROR: Prohibited Trade Audit [{DateTime.Now}]";

            EmailClass.SendEmail(profile.SupportContact, "", "", emailSubject, $"An error is detected executing the process.<br><br>Error: [{errorMessage}]", true);
        }

        /// <summary>
        /// Send Notification
        /// </summary>
        /// <param name="debugContact"></param>
        /// <returns></returns>
        public bool SendEmail(string debugContact = null)
        {
            EmailClass emailClass = new EmailClass();

            if (!String.IsNullOrEmpty(debugContact))
            {
                // Override
                ContactEmail = debugContact;
            }

            EmailClass.SendEmail(ContactEmail, ContactCC, ContactBCC, EmailSubject, EmailBody, (EmailType == (int)NotifyMethod.emailHtml));
            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
