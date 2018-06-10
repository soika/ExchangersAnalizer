// -----------------------------------------------------------------------
// <copyright file="SiteSettings.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the SiteSettings.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Settings
{
    public class SiteSettings
    {
        public int RefreshInMinutes { get; set; }

        public int NumberOfCoinsToSend { get; set; }

        public string TelegramKey { get; set; }

        public string AllowIPs { get; set; }

        public int SendMessageInMinutes { get; set; }

        public IgnoreCoinSettings[] IgnoreCoinSettings { get; set; }

        public int CriticalPercent { get; set; }
    }
}