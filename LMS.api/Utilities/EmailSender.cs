using LMS.api.Interfaces;
using System.Net;
using System.Net.Mail;

namespace LMS.api.Utilities
{
    public class EmailSender : IEmailSender
    {
        Task IEmailSender.SendEmailAsync(string email, string subject, string message)
        {
            var mail = "eg.lms.dummy@gmail.com";
            var pw = "wytfgzoffzesplfz";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)

            };

            return client.SendMailAsync(new MailMessage(
                from:mail,
                to:email,
                subject:subject,
                message
                ));
        }
    }
}
