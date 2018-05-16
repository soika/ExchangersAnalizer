// -----------------------------------------------------------------------
// <copyright file="ApplicationDbContext.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the ApplicationDbContext.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Data
{
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Symbol> Symbols { get; set; }
    }
}