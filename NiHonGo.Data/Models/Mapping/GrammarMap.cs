using System.Data.Entity.ModelConfiguration;

namespace NiHonGo.Data.Models.Mapping
{
    public class GrammarMap : EntityTypeConfiguration<Grammar>
    {
        public GrammarMap()
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
            this.ToTable("Grammar");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasMany(t => t.Levels)
                .WithMany(t => t.Grammars)
                .Map(m =>
                {
                    m.ToTable("Grammar_Level_Map");
                    m.MapLeftKey("GrammarId");
                    m.MapRightKey("LevelId");
                });
        }
    }
}