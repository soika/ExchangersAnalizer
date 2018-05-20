// -----------------------------------------------------------------------
// <copyright file="PingController.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the PingController.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class PingController : Controller
    {
        [HttpGet("")]
        public IActionResult PingApi()
        {
            return Ok(
                new
                {
                    status = 200,
                    version = "1.0"
                });
        }
    }
}