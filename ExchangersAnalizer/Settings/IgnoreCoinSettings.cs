// -----------------------------------------------------------------------
// <copyright file="IgnoreCoinSettings.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the IgnoreCoinSettings.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Settings
{
    public class IgnoreCoinSettings
    {
        public string[] Symbols { get; set; }

        public string Exchanger { get; set; }
    }
}