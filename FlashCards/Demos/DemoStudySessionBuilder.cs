using FlashCards.Context;
using FlashCards.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace FlashCards.Demos;

public class DemoStudySessionBuilder
{
    /// <summary>
    /// Creates and adds random study sessions to the database for all decks.
    /// </summary>
    public static void BuildRandomStudySessions()
    {
        var Random = new Random();
        var deckIds = new List<int>();

        using (var context = new FlashCardsContext())
        {
            deckIds = context.Decks.Select(d => d.Id).ToList();
        }

        if (deckIds.Count == 0)
        {
            AnsiConsole.Markup("[red]No decks found in the database. Please add some decks first. Press enter to continue.[/]");
            Console.ReadLine();
            return;
        }

        AnsiConsole.MarkupLine("\n[green]Decks found in the database. Generating random study sessions...[/]");
        foreach (var deckId in deckIds)
        {
            using var context = new FlashCardsContext();
            var deck = context.Decks.Include(d => d.FlashCards).FirstOrDefault(d => d.Id == deckId);

            if (deck is null)
            {
                AnsiConsole.MarkupLine($"[red]Deck with ID {deckId} not found in the database. Skipping...[/]");
                continue;
            }

            if (deck.FlashCards.Count == 0)
            {
                AnsiConsole.MarkupLine($"[red]Deck '{deck.DeckName}' has no cards. Skipping...[/]");
                continue;
            }

            var studySessions = new List<StudySession>();
            int counter = 10;
            while (counter > 0)
            {
                var numberQuestionsAsked = Random.Next(1, deck.FlashCards.Count);
                var numberAnswersCorrect = numberQuestionsAsked > 0 ? Random.Next(0, numberQuestionsAsked + 1) : 0;
                var randomStudyDate = DateTime.Now.AddDays(-Random.Next(1, 365));
                var studySession = new StudySession
                {
                    NumberAsked = numberQuestionsAsked,
                    NumberCorrect = numberAnswersCorrect,
                    DeckStudiedId = deckId,
                    DeckStudied = deck,
                    DateStudied = randomStudyDate
                };

                studySessions.Add(studySession);
                counter--;
            }

            context.StudySessions.AddRange(studySessions);
            context.SaveChanges();

            AnsiConsole.MarkupLine($"[green]Study Sessions for Deck '{deck.DeckName}' created with 10 cards.[/]");
        }

        AnsiConsole.WriteLine("Press enter to continue.");
        Console.ReadLine();
    }
}