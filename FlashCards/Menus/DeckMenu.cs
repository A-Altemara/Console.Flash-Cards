using FlashCards.Context;
using FlashCards.Models;
using Spectre.Console;

namespace FlashCards.Menus;

public class DeckMenu()
{
    public static void DisplayDecksMenu()
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
                        "Add Deck", "Delete Deck", "Edit Deck", "View Decks", "Return to Main Menu"
                    ]));

            switch (selection)
            {
                case "Add Deck":
                    AddDeck();
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

    private static void EditDeck()
    {
        var deckList = ViewDecks();
        if (deckList.Count == 0)
        {
            return;
        }

        AnsiConsole.WriteLine("Enter the ID of the deck you would like to edit:");
        int deckId = Convert.ToInt32(Console.ReadLine());
        var deck = deckList.FirstOrDefault(d => d.Id == deckId);
        if (deck == null)
        {
            AnsiConsole.WriteLine("Deck not found. Press enter to continue.");
            Console.ReadLine();
            return;
        }
        
        AnsiConsole.WriteLine("Enter the new name for the deck:");
        string newDeckName = Console.ReadLine();
        using (var context = new FlashCardsContext())
        {
            deck.DeckName = newDeckName;
            context.Decks.Update(deck);
            context.SaveChanges();
        }

        AnsiConsole.WriteLine($"Deck '{deck.DeckName}' updated successfully. Press enter to continue.");
        Console.ReadLine();
    }

    private static void DeleteDeck()
    {
        var deckList = ViewDecks();
        if (deckList.Count == 0)
        {
            return;
        }

        AnsiConsole.WriteLine("Enter the ID of the deck you would like to delete:");
        int deckId = Convert.ToInt32(Console.ReadLine());
        var deck = deckList.FirstOrDefault(d => d.Id == deckId);
        if (deck == null)
        {
            AnsiConsole.WriteLine("Deck not found. Press enter to continue.");
            Console.ReadLine();
            return;
        }
        
        AnsiConsole.WriteLine(
            $"Are you sure you want to delete the deck '{deck.DeckName}'? This will delete all associated flash cards and study sessions. (Y/N)");
        string confirmation = Console.ReadLine();
        if (confirmation.ToLower() == "y")
        {
            using (var context = new FlashCardsContext())
            {
                context.Decks.Remove(deck);
                context.SaveChanges();
            }

            AnsiConsole.WriteLine($"Deck '{deck.DeckName}' deleted successfully. Press enter to continue.");
            Console.ReadLine();
        }

        if (confirmation.ToLower() == "n")
        {
            AnsiConsole.WriteLine("Deck not deleted. Press enter to continue back to menu.");
            Console.ReadLine();
        }
    }

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