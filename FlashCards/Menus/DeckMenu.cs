using FlashCards.Context;
using FlashCards.Demos;
using FlashCards.Models;
using Spectre.Console;

namespace FlashCards.Menus;

public class DeckMenu()
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
            AnsiConsole.Markup("[bold Purple]Welcome to Decks![/]\n");
            AnsiConsole.Markup("[Purple]Please select from the following options[/]\n");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "Add Deck", "Add Demo Stacks", "Delete Deck", "Edit Deck", "View Decks", "Return to Main Menu"
                    ]));

            switch (selection)
            {
                case "Add Deck":
                    AddDeck();
                    break;
                case "Add Demo Stacks":
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
    public static List<Deck> ViewDecks()
    {
        using (var context = new FlashCardsContext())
        {
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

            return decks;
        }
    }

    /// <summary>
    /// Edits the name of a deck in the database
    /// </summary>
    private static void EditDeck()
    {
        using (var context = new FlashCardsContext())
        {
            var decks = context.Decks.ToList();

            if (decks.Count == 0)
            {
                return;
            }

            var deckNames = decks.Select(d => $"{d.Id}: {d.DeckName}").ToList();
            var selectedDeck = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the deck you would like to edit:")
                    .PageSize(10)
                    .AddChoices(deckNames)
            );

            int deckId = int.Parse(selectedDeck.Split(':')[0]);
            var deck = decks.FirstOrDefault(d => d.Id == deckId);
            
            AnsiConsole.WriteLine("Enter the new name for the deck:");
            string newDeckName = Console.ReadLine();

            deck.DeckName = newDeckName;
            context.Decks.Update(deck);
            context.SaveChanges();
            
            AnsiConsole.WriteLine($"Deck '{deck.DeckName}' updated successfully. Press enter to continue.");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Deletes a deck, and all associated Study Sessions and Flash Cards from the database
    /// </summary>
    private static void DeleteDeck()
    {
        using (var context = new FlashCardsContext())
        {
            var decks = context.Decks.ToList();

            if (decks.Count == 0)
            {
                return;
            }
            
            var deckNames = decks.Select(d => $"{d.Id}: {d.DeckName}").ToList();
            var selectedDeck = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the deck you would like to Delete:")
                    .PageSize(10)
                    .AddChoices(deckNames)
            );
            int deckId = int.Parse(selectedDeck.Split(':')[0]);
            var deck = decks.FirstOrDefault(d => d.Id == deckId);

            string confirmation =AnsiConsole.Ask<string>(
                $"Are you sure you want to delete the deck '{deck.DeckName}'? " +
                $"This will delete all associated flash cards and study sessions. (Y/N)");
            
            if (confirmation.ToLower() == "y")
            {
                var flashCards = context.FlashCards.Where(fc => fc.DeckId == deckId).ToList();
                var studySessions = context.StudySessions.Where(ss => ss.DeckStudiedId == deckId).ToList();

                context.FlashCards.RemoveRange(flashCards);
                context.StudySessions.RemoveRange(studySessions);
                context.Decks.Remove(deck);
                context.SaveChanges();

                AnsiConsole.WriteLine($"Deck '{deck.DeckName}' deleted successfully. Press enter to continue.");
                Console.ReadLine();
            }

            if (confirmation.ToLower() == "n")
            {
                AnsiConsole.WriteLine("Deck not deleted. Press enter to continue back to menu.");
                Console.ReadLine();
            }
        }
    }

    /// <summary>
    /// adds a new deck to the database
    /// </summary>
    public static void AddDeck()
    {
        Console.Clear();
        AnsiConsole.WriteLine("Enter the name of the new deck:");
        string deckName = Console.ReadLine();
        using (var context = new FlashCardsContext())
        {
            var newDeck = new Deck { DeckName = deckName };
            context.Decks.Add(newDeck);
            context.SaveChanges();
        }

        AnsiConsole.WriteLine($"Deck '{deckName}' added successfully. Press enter to continue.");
        Console.ReadLine();
    }
}