// -----------------------------------------------------------------------
// <copyright file="GroupMessage.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the GroupMessage.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Models
{
    using Telegram.Bot.Types.Enums;

    public class GroupMessage
    {
        public string Content { get; set; }

        public ParseMode ParseMode { get; set; } = ParseMode.Html;
    }
}