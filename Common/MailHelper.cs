﻿using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Common
{
    public class MailHelper
    {
        public void SendMail(string toEmail, string subject, string content)
        {
            var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
            var fromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"].ToString();
            var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
            var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
            var smtpPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();

            bool enableSs1 = bool.Parse(ConfigurationManager.AppSettings["EnableSSL"].ToString());
            string body = content;
            MailMessage message = new MailMessage(fromEmailAddress, fromEmailDisplayName);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            var client = new SmtpClient();
            client.Credentials = new NetworkCredential(fromEmailPassword, fromEmailPassword);
            client.Host = smtpHost;
            client.EnableSsl = enableSs1;
            client.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32((string)smtpPort) : 0;
            client.Send(message);
        }
    }
}
