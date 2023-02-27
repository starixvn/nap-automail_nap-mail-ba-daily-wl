using Fanex.BADailyWinLoss.Business.Model;
using Fanex.BADailyWinLoss.Business.Service;
using Fanex.BADailyWinLoss.Service.Services;
using Fanex.MailDelivery.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Fanex.BADailyWinLoss.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BADailyWinLossController : ControllerBase
    {
        private readonly Log log;

        private readonly MailingService mailingService;

        public BADailyWinLossController(MailingService mailingService, Log log)
        {
            this.mailingService = mailingService;
            this.log = log;
        }

        [HttpGet]
        public JobMessage Get()
        {
            var result = new ExecuteResultModel();
            JobMessage jobMessage = new();
            try
            {
                result = SendNotification();
            }
            catch (Exception e)
            {
                result.Success = false;

                result.Message = e.Message;
                log.Log(e);
            }
            finally
            {
                jobMessage.Mail = result.Mail;
                jobMessage.JobStatus = result.Success ? JobStatus.Success : JobStatus.DataIsNotReady;
                jobMessage.TaskLog = new TaskLog
                {
                    LogTime = DateTime.Now,
                    Message = result.Message
                };
            }

            return jobMessage;
        }

        private ExecuteResultModel SendNotification()
        {
            var mail = mailingService.SendMailToVIPGroup();

            return new ExecuteResultModel
            { 
                Mail = mail,
                Success = true
            };
        }
    }
}
