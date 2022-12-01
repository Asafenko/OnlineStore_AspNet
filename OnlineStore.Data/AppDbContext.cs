﻿using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;

namespace OnlineStore.Data.Repositories;

public class AppDbContext : DbContext
{
    // ORM
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Account> Accounts => Set<Account>();
    public AppDbContext(
        DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

}