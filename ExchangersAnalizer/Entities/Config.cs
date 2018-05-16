// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the Config.cs class.
// </summary>
// -----------------------------------------------------------------------

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