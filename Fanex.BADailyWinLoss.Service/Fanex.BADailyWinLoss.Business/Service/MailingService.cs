using Fanex.BADailyWinLoss.Business.Constants;
using Fanex.BADailyWinLoss.Business.Model;
using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.BADailyWinLoss.Business.Service
{
    public interface MailingService
    {
        Mail SendMailToVIPGroup();
    }

    public class MailingServiceImp : MailingService
    {
        private readonly BAReportService baReportService;

        public MailingServiceImp(BAReportService baReportService)
        {
            this.baReportService = baReportService;
        }

        public Mail SendMailToVIPGroup()
        {
            var isTesting = ConfigService.Config.IsTest;
            var testedDate = ConfigService.Config.TestedDate;
            var winLossDate = isTesting ? testedDate : DateTime.Now.AddDays(-1);

            baReportService.GenerateReport(winLossDate, out string fileName, out string directoryOfFileName);

            var fileNamesPath = new List<string>()
            {
                directoryOfFileName
            };

            var mailInfo = new MailInfo()
            {
                FileName = fileName,
                FileNamesPath = fileNamesPath,
                Subject = string.Format(ConfigService.Config.Subject, DateTime.Now.ToString("yyyy/MM/dd"))
            };

            var mail = CreateMailWithAttachedFile(mailInfo);

            return mail;
        }

        private static Mail CreateMailWithAttachedFile(MailInfo mailInfo)
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = root + MailTemplateConstants.AttachedFileEmail;
            string template = File.ReadAllText(filePath);
            var emailContent = Engine.Razor.RunCompile(template, "attachedFileEmail", typeof(MailInfo), mailInfo);

            Mail mail = new()
            {
                Subject = mailInfo.Subject,
                Title = ConfigService.Config.Title,
                Body = emailContent,
                Attachments = new List<MailAttachment>()
            };

            foreach (string fileNamePath in mailInfo.FileNamesPath)
            {
                mail.Attachments.Add(new MailAttachment(fileNamePath, FileExtension.MS_EXCEL));
            }

            return mail;
        }
    }
}
