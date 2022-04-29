using Microsoft.EntityFrameworkCore;
using USTGlobal.PIP.EmailScheduler.Entities;

namespace USTGlobal.PIP.EmailScheduler
{
    public partial class PipContext : DbContext
    {
        private readonly string connectionString;

        public PipContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public PipContext(DbContextOptions<PipContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PipSheetComments>().HasKey(x => x.PIPSheetCommentId);
        }

        public virtual DbSet<PipSheet> PipSheet { get; set; }
        public virtual DbSet<PipSheetComments> PipSheetComments { get; set; }
    }
}
