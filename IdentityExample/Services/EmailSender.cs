using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace IdentityExample.Services
{
    public interface IEmailSender 
    {
        public Task<AsyncVoidMethodBuilder> SendEmail(string senderName, string senderEmail, string recieverName,
            string recieverEmail, string subject, string body, string host, int port);
 
    }
    public class EmailSender : IEmailSender
    {
        public async Task<AsyncVoidMethodBuilder> SendEmail(string senderName, string senderEmail, string recieverName,
            string recieverEmail, string subject, string body, string host, int port)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(senderName,senderEmail));
            emailMessage.To.Add(new MailboxAddress(recieverName, recieverEmail));
            emailMessage.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                var secureSocketOptions = SecureSocketOptions.None;
                await client.ConnectAsync(host, port, secureSocketOptions);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

            return AsyncVoidMethodBuilder.Create();

        }
    }
}
