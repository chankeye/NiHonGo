using System.Data.Entity.ModelConfiguration;

namespace NiHonGo.Data.Models.Mapping
{
    public class GrammerMap : EntityTypeConfiguration<Grammer>
    {
        public GrammerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Grammer");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasMany(t => t.Levels)
                .WithMany(t => t.Grammers)
                .Map(m =>
                {
                    m.ToTable("Grammer_Level_Map");
                    m.MapLeftKey("GrammerId");
                    m.MapRightKey("LevelId");
                });
        }
    }
}