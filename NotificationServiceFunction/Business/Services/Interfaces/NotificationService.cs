using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public class NotificationService : INotificationService
    {
        public NotificationService()
        {

        }

        public async Task<bool> ProcessAsync(string queueMessageJson)
        {
            return true;
        }
    }
}
