using FlashCards.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Context;

public class FlashCardsContext : DbContext
{
    public DbSet<Deck> Decks { get; set; }
    public DbSet<FlashCard> FlashCards { get; set; }
    public DbSet<StudySession> StudySessions { get; set; }
    
    public string DbPath { get; }

    public FlashCardsContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "FlashCardsApp@localhost");
    }
    public FlashCardsContext(DbContextOptions<FlashCardsContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlashCard>()
            .HasOne<Deck>(f => f.Deck)
            .WithMany(d => d.FlashCards)
            .HasForeignKey(f => f.DeckId);
        modelBuilder.Entity<StudySession>()
            .HasOne<Deck>(s => s.DeckStudied)
            .WithMany(d => d.StudySessions)
            .HasForeignKey(s => s.DeckStudiedId);
        base.OnModelCreating(modelBuilder);
    }
}