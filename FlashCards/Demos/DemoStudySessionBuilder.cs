using FlashCards.Context;
using FlashCards.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace FlashCards.Demos;

public static class DemoStudySessionBuilder
{
    /// <summary>
    /// Creates and adds random study sessions to the database for all decks.
    /// </summary>
    public static void BuildRandomStudySessions()
    {
        var random = new Random();

        using var context = new FlashCardsContext();
        var decks = context.Decks.Include(deck => deck.FlashCards).ToList();

        if (decks.Count == 0)
        {
            AnsiConsole.Markup(
                "[red]No decks found in the database. Please add some decks first. Press enter to continue.[/]");
            Console.ReadLine();
            return;
        }

        AnsiConsole.MarkupLine("\n[green]Decks found in the database. Generating random study sessions...[/]");
        
        foreach (var deck in decks)
        {
            if (deck.FlashCards.Count == 0)
            {
                AnsiConsole.MarkupLine($"[red]Deck '{deck.DeckName}' has no cards. Skipping...[/]");
                continue;
            }

            var studySessions = new List<StudySession>();
            
            for(int i = 0; i < 10; i++)
            {
                var numberQuestionsAsked = random.Next(1, deck.FlashCards.Count);
                var numberAnswersCorrect = numberQuestionsAsked > 0 ? random.Next(0, numberQuestionsAsked + 1) : 0;
                var randomStudyDate = DateTime.Now.AddDays(-random.Next(1, 365));
               
                var studySession = new StudySession
                {
                    NumberAsked = numberQuestionsAsked,
                    NumberCorrect = numberAnswersCorrect,
                    DeckStudiedId = deck.Id,
                    DeckStudied = deck,
                    DateStudied = randomStudyDate
                };

                studySessions.Add(studySession);
            }
            
            context.StudySessions.AddRange(studySessions);
            context.SaveChanges();

            AnsiConsole.MarkupLine($"[green]Study Sessions for Deck '{deck.DeckName}' created with 10 cards.[/]");
        }

        AnsiConsole.WriteLine("Press enter to continue.");
        Console.ReadLine();
    }
}