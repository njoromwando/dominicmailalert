using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AlertSystem.Api.DTOs;
using AlertSystem.Api.Service.IServices;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

//using MailKit.Security;
using MimeKit;

namespace AlertSystem.Api.Service
{
    public class MailAlertService : IMailAlertService
    {
        private readonly ApplicationDbContext _context;

        private readonly MailSettings _mailSettings;

        public MailAlertService(IOptions<MailSettings> mailSettings, ApplicationDbContext context)
        {
            _context = context;

            _mailSettings = mailSettings.Value;
        }

        public async Task SendAlertEmail(string ToEmail, string name)
        {
            string MailText = $"Item {name} Has Reached Restock Level Please Top Up to avoid Running out";
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(ToEmail));
            email.Subject = $"Stock Alert";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task GetLowStock()
        {
            try
            {
                var query =
                    $"SELECT ItemCode,ItemDescrip,ReOrderLevel FROM dbo.vw_Open_TReceived_TIssued WHERE (ReOrderLevel > OpenStock + TReceived - TIssued)";
                var pending = await _context.AlertsItems.FromSqlRaw(query).ToListAsync();

                foreach (var item in pending)
                {
                    Console.WriteLine(item.ItemCode + '|' + item.ItemDescrip);
                    await SendAlertEmail("jamesnjoroge87@gmail.com", item.ItemCode + '|' + item.ItemDescrip);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}