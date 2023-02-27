using Fanex.MailDelivery.Core.Models;

namespace Fanex.BADailyWinLoss.Business.Model
{
    public class ExecuteResultModel
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public Mail Mail { get; set; } = new Mail();
    }
}
