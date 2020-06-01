using NiHonGo.Data.Models.Mapping;
using System.Data.Entity;

namespace NiHonGo.Data.Models
{
    public partial class NiHonGoContext : DbContext
    {
        static NiHonGoContext()
        {
#if DEBUG
            Database.SetInitializer<NiHonGoContext>(new CreateDatabaseIfNotExists<NiHonGoContext>());
#else
            Database.SetInitializer<NiHonGoContext>(null);
#endif
        }

        public NiHonGoContext()
            : base("Name=NiHonGoContext")
        {
        }

        public DbSet<Word> Words { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Grammar> Grammars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new GrammarMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new LevelMap());
            modelBuilder.Configurations.Add(new VideoMap());
            modelBuilder.Configurations.Add(new WordMap());
        }
    }
}