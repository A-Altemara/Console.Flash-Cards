using Spectre.Console;
using System.Text.Json;
using FlashCards.Context;
using FlashCards.Models;

namespace FlashCards.Demos;

public class DemoStackBuilder
{
    /// <summary>
    /// Imports the DemoCards.json file and adds the demo decks to the database. 
    /// </summary>
    public static async Task ImportDemoStack()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Demos/DemoCards.json");
        if (!File.Exists(path))
        {
            AnsiConsole.Markup("[red]DemoCards.json not found![/]");
            return;
        }

        var json = await File.ReadAllTextAsync(path);
        var demoStack = JsonSerializer.Deserialize<List<Deck>>(json);
        if (demoStack is null)
        {
            AnsiConsole.Markup("[red]Failed to deserialize DemoCards.json![/]");
            return;
        }

        await using (var context = new FlashCardsContext())
        {
            foreach (var deck in demoStack)
            {
                if (context.Decks.Any(d => d.DeckName == deck.DeckName))
                {
                    AnsiConsole.Markup($"[yellow]Deck '{deck.DeckName}' already exists. Skipping...[/]\n");
                    continue;
                }

                var addedDeck = await AddDeck(context, deck);
                if (addedDeck is null)
                {
                    AnsiConsole.Markup("[red]Failed to add Deck.[/]\n");
                    AnsiConsole.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    AnsiConsole.Markup($"[green]Added {addedDeck.DeckName} and cards to the database![/]\n");
                }
            }

            AnsiConsole.WriteLine("\nDemo Decks added to the database. Press Enter to continue.");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Adds a deck and all associated flashcards to the database.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="deck"></param>
    /// <returns> A task that represents the Asynchronous Process </returns>
    private static async Task<Deck> AddDeck(FlashCardsContext context, Deck deck)
    {
        var newDeck = new Deck { DeckName = deck.DeckName, FlashCards = deck.FlashCards };
        context.Decks.Add(newDeck);
        context.FlashCards.AddRange(deck.FlashCards);
        await context.SaveChangesAsync();
        return newDeck;
    }
}