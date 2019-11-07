using Logic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .HasKey(i => i.UserId);

            blder.Entity<UserState>()
                .HasKey(i => i.UserId);

            blder.Entity<UserInfo>()
                .HasOne(i => i.User)
                .WithOne();

            blder.Entity<Question>()
                .HasMany(i => i.Answers)
                .WithOne(i => i.Origin);

            blder.Entity<UserInfo>()
                .HasOne(i => i.State)
                .WithOne(i => i.UserInfo)
                .HasForeignKey<UserState>(i => i.UserId);

            SeedData(blder);

            base.OnModelCreating(blder);
        }

        private void SeedData(ModelBuilder blder)
        {
            DataSeeder.Seed(blder);
        }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Solution> Solutions { get; set; }
    }


}
