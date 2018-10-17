using System.Data.Entity.ModelConfiguration;

namespace NiHonGo.Data.Models.Mapping
{
    public class VideoMap : EntityTypeConfiguration<Video>
    {
        public VideoMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.YoutubeUrl)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.JapaneseTitle)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.JapaneseContent)
                .IsRequired();

            this.Property(t => t.ChineseTitle)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.ChineseContent)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Video");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.YoutubeUrl).HasColumnName("YoutubeUrl");
            this.Property(t => t.JapaneseTitle).HasColumnName("JapaneseTitle");
            this.Property(t => t.JapaneseContent).HasColumnName("JapaneseContent");
            this.Property(t => t.ChineseTitle).HasColumnName("ChineseTitle");
            this.Property(t => t.ChineseContent).HasColumnName("ChineseContent");
            this.Property(t => t.CreateDateTime).HasColumnName("CreateDateTime");
            this.Property(t => t.UpdateDateTime).HasColumnName("UpdateDateTime");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.Videos)
                .HasForeignKey(d => d.UserId);

            this.HasMany(t => t.Levels)
                .WithMany(t => t.Videos)
                .Map(m =>
                    {
                        m.ToTable("Video_Level_Map");
                        m.MapLeftKey("VideoId");
                        m.MapRightKey("LevelId");
                    });

            this.HasMany(t => t.Words)
                .WithMany(t => t.Videos)
                .Map(m =>
                {
                    m.ToTable("Video_Word_Map");
                    m.MapLeftKey("VideoId");
                    m.MapRightKey("WordId");
                });

            this.HasMany(t => t.Grammars)
                .WithMany(t => t.Videos)
                .Map(m =>
                {
                    m.ToTable("Video_Grammar_Map");
                    m.MapLeftKey("VideoId");
                    m.MapRightKey("GrammarId");
                });
        }
    }
}