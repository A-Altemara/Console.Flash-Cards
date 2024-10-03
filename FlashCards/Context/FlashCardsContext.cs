using FlashCards.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Context;

public class FlashCardsContext : DbContext
{
    public DbSet<Deck> Decks { get; set; }
    public DbSet<FlashCard> FlashCards { get; set; }
    public DbSet<StudySession> StudySessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlashCard>()
            .HasOne<Deck>(f => f.Deck)
            .WithMany(d => d.FlashCards)
            .HasForeignKey(f => f.DeckId);
        modelBuilder.Entity<StudySession>()
            .HasOne<Deck>(s => s.Deck)
            .WithMany(d => d.StudySessions)
            .HasForeignKey(s => s.DeckId);
        base.OnModelCreating(modelBuilder);
    }
}