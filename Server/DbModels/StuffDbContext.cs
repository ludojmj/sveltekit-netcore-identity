using Microsoft.EntityFrameworkCore;

namespace Server.DbModels
{
    public partial class StuffDbContext : DbContext
    {
        public StuffDbContext()
        {
        }

        public StuffDbContext(DbContextOptions<StuffDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TStuff> TStuffs { get; set; }
        public virtual DbSet<TUser> TUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TStuff>(entity =>
            {
                entity.HasKey(e => e.StfId);

                entity.ToTable("t_stuff");

                entity.Property(e => e.StfId).HasColumnName("stf_id");

                entity.Property(e => e.StfCreatedAt).HasColumnName("stf_created_at");

                entity.Property(e => e.StfDescription).HasColumnName("stf_description");

                entity.Property(e => e.StfLabel)
                    .IsRequired()
                    .HasColumnName("stf_label");

                entity.Property(e => e.StfOtherInfo).HasColumnName("stf_other_info");

                entity.Property(e => e.StfUpdatedAt).HasColumnName("stf_updated_at");

                entity.Property(e => e.StfUserId)
                    .IsRequired()
                    .HasColumnName("stf_user_id");

                entity.HasOne(d => d.StfUser)
                    .WithMany(p => p.TStuffs)
                    .HasForeignKey(d => d.StfUserId);
            });

            modelBuilder.Entity<TUser>(entity =>
            {
                entity.HasKey(e => e.UsrId);

                entity.ToTable("t_user");

                entity.Property(e => e.UsrId).HasColumnName("usr_id");

                entity.Property(e => e.UsrCreatedAt).HasColumnName("usr_created_at");

                entity.Property(e => e.UsrEmail).HasColumnName("usr_email");

                entity.Property(e => e.UsrFamilyName).HasColumnName("usr_family_name");

                entity.Property(e => e.UsrGivenName).HasColumnName("usr_given_name");

                entity.Property(e => e.UsrName).HasColumnName("usr_name");

                entity.Property(e => e.UsrUpdatedAt).HasColumnName("usr_updated_at");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
