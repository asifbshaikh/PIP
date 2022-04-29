using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using USTGlobal.PIP.ApplicationCore.Entities;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public partial class PipContext : DbContext
    {
        private readonly bool isConfigured = false;

        public PipContext(bool isConfigured)
        {
            this.isConfigured = isConfigured;
        }

        public PipContext(DbContextOptions<PipContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (isConfigured)
            {
                IConfiguration configManual = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .Build();

                optionsBuilder.UseSqlServer(configManual.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).IsRequired();
            });
            modelBuilder.Entity<UserRole>()
              .HasKey(x => new { x.UserId, x.RoleId });


            modelBuilder.Entity<Project>().HasIndex(x => x.SFProjectId).IsUnique();
            modelBuilder.Entity<ProjectLocation>().HasKey(x => new { x.LocationId, x.PIPSheetId });
            modelBuilder.Entity<ProjectControl>().HasKey(x => x.PipSheetId);
            modelBuilder.Entity<ProjectHeader>().HasKey(x => x.PipSheetId);
            modelBuilder.Entity<DeliveryBilling>().HasKey(x => x.ProjectDeliveryTypeId);
            modelBuilder.Entity<Location>().HasKey(x => x.SerialId);
            modelBuilder.Entity<MasterList>().HasKey(x => x.MasterId);
            modelBuilder.Entity<CalculatedValue>().HasKey(x => x.PipSheetId);
            modelBuilder.Entity<PipSheetSaveStatus>().HasKey(x => x.PipSheetId);
            modelBuilder.Entity<OverrideNotification>().HasKey(x => x.PipSheetId);
            modelBuilder.Entity<PIPSheetComments>().HasKey(x => x.PIPSheetCommentId);
            modelBuilder.Entity<UserRoleReadOnly>().HasKey(x => x.UserId);
            modelBuilder.Entity<SharePipSheet>().HasKey(x => new { x.PipSheetId, x.RoleId, x.AccountId, x.SharedByUserId, x.SharedWithUserId });
            modelBuilder.Entity<Account>().HasKey(x => x.AccountId);
            modelBuilder.Entity<ReportKPI>().HasKey(x => x.ReportKPIId);
        }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<MasterList> MasterList { get; set; }
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<Holiday> Holiday { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<MasterVersion> MasterVersion { get; set; }
        public virtual DbSet<Milestone> Milestone { get; set; }
        public virtual DbSet<MilestoneGroup> MilestoneGroup { get; set; }
        public virtual DbSet<Rate> Rate { get; set; }
        public virtual DbSet<Resource> Resource { get; set; }
        public virtual DbSet<ResourceGroup> ResourceGroup { get; set; }
        public virtual DbSet<ServiceLine> ServiceLine { get; set; }
        public virtual DbSet<ServicePortfolio> ServicePortfolio { get; set; }
        public virtual DbSet<Markup> Markup { get; set; }
        public virtual DbSet<ProjectBillingType> ProjectBillingType { get; set; }
        public virtual DbSet<ProjectDeliveryType> ProjectDeliveryType { get; set; }
        public virtual DbSet<ContractingEntity> ContractingEntity { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<DeliveryBilling> DeliveryBilling { get; set; }
        public virtual DbSet<PipSheet> PipSheet { get; set; }
        public virtual DbSet<PIPSheetComments> PipSheetComments { get; set; }
        public virtual DbSet<PipSheetStatus> PipSheetStatus { get; set; }
        public virtual DbSet<ProjectLocation> ProjectLocation { get; set; }
        public virtual DbSet<ProjectMilestone> ProjectMilestone { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<ProjectHeader> ProjectHeader { get; set; }
        public virtual DbSet<ProjectControl> ProjectControl { get; set; }
        public virtual DbSet<Margin> Margin { get; set; }
        public virtual DbSet<StandardCostRate> StandardCostRate { get; set; }
        public virtual DbSet<CorpBillingRate> CorpBillingRate { get; set; }
        public virtual DbSet<CalculatedValue> CalculatedValue { get; set; }
        public virtual DbSet<ProjectResource> ProjectResource { get; set; }
        public virtual DbSet<ProjectResourcePeriodDetails> ProjectResourcePeriodDetails { get; set; }
        public virtual DbSet<PipSheetSaveStatus> PipSheetSaveStatus { get; set; }
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<OverrideNotification> OverrideNotification { get; set; }
        public virtual DbSet<UserRoleReadOnly> UserRoleReadOnly { get; set; }
        public virtual DbSet<SharePipSheet> SharePipSheet { get; set; }
        public virtual DbSet<ReportKPI> ReportKPI { get; set; }
    }
}
