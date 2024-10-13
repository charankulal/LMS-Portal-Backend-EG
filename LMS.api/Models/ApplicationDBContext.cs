using Microsoft.EntityFrameworkCore;

namespace LMS.api.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }
        
        // Following code first approach

        public DbSet<Users> Users { get; set; }

        public DbSet<Batches> Batches { get; set; }

        public DbSet<Certificates> Certificates { get; set; }

        public DbSet<Sprints> Sprints { get; set; }

        public DbSet<Contents> Contents { get; set; }

        public DbSet<BatchUsers> BatchUsers { get; set; }

        public DbSet<Attendances> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            // Composite primary key for BatchUsers
            modelBuilder.Entity<BatchUsers>()

                .HasKey(bu => new {bu.BatchId,bu.UserId }); 

            base.OnModelCreating(modelBuilder);

            // Composite primary key for Attendance
            modelBuilder.Entity<Attendances>()

                .HasKey(at => new { at.BatchId, at.UserId, at.Date }); 

            base.OnModelCreating(modelBuilder);
        }
        }
    }
