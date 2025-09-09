using EmailApp.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EmailApp.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>().HasMany(message => message.SentMessages).WithOne(s=>s.Sender).HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<AppUser>().HasMany(message => message.RecieverMessages).WithOne(y=>y.Reciever).HasForeignKey(x => x.RecieverId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
        public DbSet<Message> Messages { get; set; }
    }
}
