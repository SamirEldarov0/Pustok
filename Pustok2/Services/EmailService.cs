using MailKit.Net.Smtp;//smtp
using MailKit.Security;//+SecureSocketOptions
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;


//using System.Net.Mail;
using System.Security.Cryptography;

namespace Pustok2.Services
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html);
    }
    public class EmailService:IEmailService
    {
        //private readonly AppSettings _appSettings;
        //public EmailService(IOptions<AppSettings> appSettings)
        //{
        //    _appSettings = appSettings.Value;
        //port,user,sifre,mail,host bunlar appsettingsde saxlanirlir json icinde
        //}
        public void Send(string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("samir.eldarov0@yandex.com"));//from
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();

            smtp.Connect("smtp.yandex.com", 465, SecureSocketOptions.StartTls);
            //smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            //host-smtp.gmail.com,port-587

            //465-------samir.eldarov0@yandex.com
            smtp.Authenticate("samir.eldarov0@yandex.com", "SaMkA2003-");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
