using System;
using System.Net.Mail;

namespace ScaleThreadProcess
{
    public class EmailClass : IDisposable
    {
        /// <summary>
        /// Send Email procedure
        /// </summary>
        /// <param name="strTo"></param>
        /// <param name="strToBCC"></param>
        /// <param name="strSubject"></param>
        /// <param name="strMsg"></param>
        /// <param name="isHtmlEmail"></param>
        /// <returns></returns>
        public static bool SendEmail(string strTo, string strToCC, string strToBCC, string strSubject, string strMsg, bool isHtmlEmail = true)
        {
            using (MailMessage mail = new MailMessage())
            {

                string fromContactEmail = CommonHelper.fromContactEmail ?? new CommonHelper().GetWorkerSettingAttribute(WorkerSettingSection.EMAIL, WorkerSettingAttribute.SMTP_EMAIL_FROM_CONTACT);
                mail.From = new MailAddress(fromContactEmail);

                //In DEV emails will get sent to DEV_EMAIL, original emails will be listed on the bottom
                //In TEST emails will get sent to TEST_EMAIL, original emails will be listed on the bottom
                //In PROD emails will get sent to passed in strTo
                switch (CommonHelper.GetEnvironment())
                {
                    case "DEV":
                        //strMsg = strMsg + string.Format("<p /><p>PROD Recipicent Email(s): {0}</p>", strTo);
                        strSubject = "[DEV]-" + strSubject;
                        // strTo = _util.GetWebAppProperty("DEV_EMAIL");
                        break;

                    case "QA":
                        strSubject = "[QA]-" + strSubject;
                        break;

                    case "TEST":
                        //strMsg = strMsg + string.Format("<p /><p>PROD Recipicent Email(s): {0}</p>", strTo);
                        strSubject = "[TEST]-" + strSubject;
                        //strTo = _util.GetWebAppProperty("TEST_EMAIL");
                        break;
                }

                if (strTo.Contains(";"))
                {
                    string[] emailAddress = strTo.Split(';');
                    foreach (string item in emailAddress)
                    {
                        if (item.Trim().Length > 0)
                        {
                            mail.To.Add(item);
                        }

                    }
                }
                else
                {
                    mail.To.Add(strTo);
                }

                if (!String.IsNullOrEmpty(strToBCC))
                {
                    if (strToBCC.Contains(";"))
                    {
                        string[] emailAddress = strToBCC.Split(';');
                        foreach (string item in emailAddress)
                        {
                            mail.Bcc.Add(item);
                        }
                    }
                    else
                    {
                        mail.Bcc.Add(strToBCC);
                    }
                }

                if (!String.IsNullOrEmpty(strToCC))
                {
                    if (strToCC.Contains(";"))
                    {
                        string[] emailAddress = strToCC.Split(';');
                        foreach (string item in emailAddress)
                        {
                            mail.CC.Add(item);
                        }
                    }
                    else
                    {
                        mail.CC.Add(strToCC);
                    }
                }

                mail.Subject = strSubject;
                mail.Body = strMsg;
                mail.IsBodyHtml = isHtmlEmail;
                mail.Priority = MailPriority.Normal;

                // Set SMTP server from the process profile (if available)
                string smtpServer = CommonHelper.smtpServer ?? new CommonHelper().GetWorkerSettingAttribute(WorkerSettingSection.EMAIL, WorkerSettingAttribute.SMTP_EMAIL_SERVER);

                using (SmtpClient smtp = new SmtpClient(smtpServer))
                {
                    smtp.Send(mail);
                }
            }

            return true;
        }

        public static void SendTestEmail()
        {
            SendEmail("gsosnovsky@nfa.futures.org", "gsosnovsky@nfa.futures.org", "gsosnovsky@nfa.futures.org", "Test Email - All", "Test email from (gsosnovsky@nfa.futures.org) To CC/BCC (gsosnovsky@nfa.futures.org)");
            SendEmail("drosenberg@nfa.futures.org", "gsosnovsky@nfa.futures.org", "", "Test Email - CC", "Test email To (drosenberg@nfa.futures.org) CC (gsosnovsky@nfa.futures.org)");
            SendEmail("drosenberg@nfa.futures.org", "", "gsosnovsky@nfa.futures.org", "Test Email - BCC", "Test email To (drosenberg@nfa.futures.org) BCC (gsosnovsky@nfa.futures.or)");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
