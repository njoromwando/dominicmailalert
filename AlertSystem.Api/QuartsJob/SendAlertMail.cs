using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlertSystem.Api.Service.IServices;
using Quartz;

namespace AlertSystem.Api.QuartsJob
{
    public class SendAlertMail : IJob
    {
        private readonly IMailAlertService _mailAlertService;

        public SendAlertMail(IMailAlertService mailAlertService)
        {
            _mailAlertService = mailAlertService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                Task.Run(() => _mailAlertService.GetLowStock()).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Task.CompletedTask;
        }
    }
}