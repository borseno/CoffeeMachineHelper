using Logic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace Logic
{
    public class AppContext : DbContext
    {
        public AppContext() : base()
        { 

        }

        public AppContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder blder)
        {
            blder.Entity<Key>()
                .HasKey(i => i.Value);

            blder.Entity<UserInfo>()
                .HasOne(i => i.User)
                .WithOne();

            blder.Entity<UserInfo>()
                .HasKey(i => i.UserId);

            base.OnModelCreating(blder);
        }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Key> Keys { get; set; }
    }
}
