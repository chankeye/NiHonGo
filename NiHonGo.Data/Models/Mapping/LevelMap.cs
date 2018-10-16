using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NiHonGo.Data.Models.Mapping
{
    public class LevelMap : EntityTypeConfiguration<Level>
    {
        public LevelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Display)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Level");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Display).HasColumnName("Display");
        }
    }
}
