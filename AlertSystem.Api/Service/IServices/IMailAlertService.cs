using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlertSystem.Api.DTOs;

namespace AlertSystem.Api.Service.IServices
{
    public interface IMailAlertService
    {
        Task SendAlertEmail(string ToEmail, string name);

        Task GetLowStock();
    }
}