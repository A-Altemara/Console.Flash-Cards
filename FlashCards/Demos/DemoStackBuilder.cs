using Spectre.Console;
using System.Text.Json;
using FlashCards.Context;
using FlashCards.Models;

namespace FlashCards.Demos;

public class DemoStackBuilder
{
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
        if(demoStack is null)
        {
            AnsiConsole.Markup("[red]Failed to deserialize DemoCards.json![/]");
            return;
        }

        foreach (var deck in demoStack)
        {
            var addedDeck = await AddDeck(deck);
            if (addedDeck is null)
            {
                AnsiConsole.Markup("[red]Failed to add Deck.[/]");
                AnsiConsole.WriteLine("press enter to continue.");
                Console.ReadLine();
                continue;
            }
            else
            {
                AnsiConsole.Markup($"[green]Added {addedDeck.DeckName} and cards to the database![/]");
            }
            AnsiConsole.WriteLine("Demo Decks added to the database. Press Enter to continue.");
            Console.ReadLine();
        }
    }

    private static async Task<Deck> AddDeck(Deck deck)
    {
        await using (var context = new FlashCardsContext())
        {
            var newDeck = new Deck { DeckName = deck.DeckName, FlashCards = deck.FlashCards };
            context.Decks.Add(newDeck);
            context.FlashCards.AddRange(deck.FlashCards);
            await context.SaveChangesAsync();
        }
        return deck;
    }
}