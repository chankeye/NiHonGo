using System.Data.Entity.ModelConfiguration;

namespace NiHonGo.Data.Models.Mapping
{
    public class WordMap : EntityTypeConfiguration<Word>
    {
        public WordMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Japanese)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Chinese)
               .IsRequired()
               .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Word");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Japanese).HasColumnName("Japanese");
            this.Property(t => t.Chinese).HasColumnName("Chinese");

            // Relationships
            this.HasMany(t => t.Levels)
                .WithMany(t => t.Words)
                .Map(m =>
                {
                    m.ToTable("Word_Level_Map");
                    m.MapLeftKey("WordId");
                    m.MapRightKey("LevelId");
                });
        }
    }
}