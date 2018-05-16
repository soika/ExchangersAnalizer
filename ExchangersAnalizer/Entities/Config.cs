using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangersAnalizer.Entities
{
    public class Config
    {
        public int Id { get; set; }

        public int RefreshInMinutes { get; set; }

        public int NumberOfCoinsToSend { get; set; }

        public string TelegramKey { get; set; }

        public string TelegramChatGroup { get; set; }
    }
}
