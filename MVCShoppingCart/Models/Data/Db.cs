using System.Data.Entity;

namespace MVCShoppingCart.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<PageDto> Pages { get; set; }
        public DbSet<SidebarDto> Sidebar { get; set; }
    }
}