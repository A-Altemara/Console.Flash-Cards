using FlashCards.Context;
using FlashCards.Demos;
using FlashCards.Models;
using Spectre.Console;

namespace FlashCards.Menus;

public static class DeckMenu
{
    /// <summary>
    /// Displays the Deck menu and handles user selections for adding, deleting, editing, and viewing decks.
    /// </summary>
    public static async Task DisplayDecksMenu()
    {
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold Purple]Welcome to Decks![/]");
            AnsiConsole.MarkupLine("[Purple]Please select from the following options[/]");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "Add Deck", "Add Demo Decks", "Delete Deck", "Edit Deck", "View Decks", "Return to Main Menu"
                    ]));

            switch (selection)
            {
                case "Add Deck":
                    AddDeck();
                    break;
                case "Add Demo Decks":
                    await DemoStackBuilder.ImportDemoStack();
                    break;
                case "Delete Deck":
                    DeleteDeck();
                    break;
                case "Edit Deck":
                    EditDeck();
                    break;
                case "View Decks":
                    ViewDecks();
                    AnsiConsole.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                    break;
                case "Return to Main Menu":
                    continueProgram = false;
                    Console.WriteLine("Exiting Decks Menu, Press enter to continue");
                    Console.ReadLine();
                    break;
            }
        }
    }

    /// <summary>
    /// displays all decks in the database
    /// </summary>
    /// <returns>
    /// list of all decks in the database
    /// </returns>
    public static void ViewDecks()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            AnsiConsole.WriteLine("No decks available.");
        }
        else
        {
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Deck Name");

            foreach (var deck in decks)
            {
                table.AddRow(deck.Id.ToString(), deck.DeckName);
            }

            AnsiConsole.Write(table);
        }
    }

    /// <summary>
    /// Edits the name of a deck in the database
    /// </summary>
    private static void EditDeck()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }

        var selectedDeck = GetDeckSelection(decks);
        string newDeckName =
            AnsiConsole.Ask<string>("Enter the new name for the deck. Note Deck Names are Case Sensitive.");

        if (decks.Any(d => d.DeckName == newDeckName))
        {
            AnsiConsole.WriteLine("Deck already exists. Press enter to continue.");
            Console.ReadLine();
            return;
        }

        selectedDeck.DeckName = newDeckName;
        context.Decks.Update(selectedDeck);
        context.SaveChanges();

        AnsiConsole.WriteLine($"Deck '{selectedDeck.DeckName}' updated successfully. Press enter to continue.");
        Console.ReadLine();
    }

    public static Deck GetDeckSelection(List<Deck> decks)
    {
        var deckNames = decks.Select(d => $"{d.Id}: {d.DeckName}").ToList();
        var selectedDeck = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the deck you would like to access:")
                .PageSize(10)
                .AddChoices(deckNames)
        );

        var deckId = int.Parse(selectedDeck.Split(':')[0]);
        var deck = decks.First(d => d.Id == deckId);
        return deck;
    }

    /// <summary>
    /// Deletes a deck, and all associated Study Sessions and Flash Cards from the database
    /// </summary>
    private static void DeleteDeck()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }

        var selectedDeck = GetDeckSelection(decks);
        var confirmation = AnsiConsole.Prompt(new ConfirmationPrompt(
            $"Are you sure you want to delete the deck '{selectedDeck.DeckName}'? " +
            $"This will delete all associated flash cards and study sessions. (Y/N)"));

        if (confirmation)
        {
            var flashCards = context.FlashCards.Where(fc => fc.DeckId == selectedDeck.Id).ToList();
            var studySessions = context.StudySessions.Where(ss => ss.DeckStudiedId == selectedDeck.Id).ToList();

            context.FlashCards.RemoveRange(flashCards);
            context.StudySessions.RemoveRange(studySessions);
            context.Decks.Remove(selectedDeck);
            context.SaveChanges();

            AnsiConsole.WriteLine($"Deck '{selectedDeck.DeckName}' deleted successfully. Press enter to continue.");
            Console.ReadLine();
        }
        else
        {
            AnsiConsole.WriteLine("Deck not deleted. Press enter to continue back to menu.");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// adds a new deck to the database
    /// </summary>
    public static void AddDeck()
    {
        Console.Clear();
        string deckName =
            AnsiConsole.Ask<string>("Enter the name of the new deck. Note Deck Names are Case Sensitive.");
        using (var context = new FlashCardsContext())
        {
            var decks = context.Decks.ToList();
            if (decks.Any(d => d.DeckName == deckName))
            {
                AnsiConsole.WriteLine("Deck already exists. Press enter to continue.");
                Console.ReadLine();
                return;
            }

            var newDeck = new Deck { DeckName = deckName };
            context.Decks.Add(newDeck);
            context.SaveChanges();
        }

        AnsiConsole.WriteLine($"Deck '{deckName}' added successfully. Press enter to continue.");
        Console.ReadLine();
    }
}