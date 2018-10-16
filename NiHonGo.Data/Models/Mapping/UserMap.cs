using System.Data.Entity.ModelConfiguration;

namespace NiHonGo.Data.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("User");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.FBToken).HasColumnName("FBToken");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.CreateDateTime).HasColumnName("CreateDateTime");

            // Relationships
            this.HasMany(t => t.Levels)
                .WithMany(t => t.Users)
                .Map(m =>
                {
                    m.ToTable("User_Level_Map");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("LevelId");
                });
        }
    }
}