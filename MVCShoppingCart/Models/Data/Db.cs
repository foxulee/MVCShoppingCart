﻿using System.Data.Entity;

namespace MVCShoppingCart.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<PageDto> Pages { get; set; }
        public DbSet<SidebarDto> Sidebar { get; set; }
        public DbSet<CategoryDto> Categories { get; set; }
        public DbSet<ProductDto> Products { get; set; }
        public DbSet<UserDto> Users { get; set; }
        public DbSet<RoleDto> Roles { get; set; }
        public DbSet<UserRoleDto> UserRoles { get; set; }
        public DbSet<OrderDto> Orders { get; set; }
        public DbSet<OrderDetailsDto> OrderDetails { get; set; }
    }
}