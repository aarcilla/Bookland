using System;
using System.Configuration;
using System.Net.Mail;

namespace Bookland.Helpers
{
    public class MailHelpers : Abstract.IMailHelpers
    {
        public bool SendAdminEmail(string toAddress, string subject, string body)
        {
            string smtpHost = ConfigurationManager.AppSettings["adminSmtpHost"];
            int smtpPort = Int32.Parse(ConfigurationManager.AppSettings["adminSmtpPort"]);
            bool smtpSslEnabled = bool.Parse(ConfigurationManager.AppSettings["adminSmtpSslEnabled"]);
            string adminUserName = ConfigurationManager.AppSettings["adminUserName"];
            string adminEmail = ConfigurationManager.AppSettings["adminEmail"];
            string adminPassword = ConfigurationManager.AppSettings["adminPassword"];

            using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(adminUserName, adminPassword);
                smtpClient.EnableSsl = smtpSslEnabled;

                MailMessage mail = new MailMessage(adminEmail, toAddress, subject, body);
                mail.IsBodyHtml = true;
                try
                {
                    smtpClient.Send(mail);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }
    }
}