using FlashCards.Context;
using FlashCards.Models;
using Spectre.Console;

namespace FlashCards.Menus;

public class FlashCardMenu()
{
    /// <summary>
    /// Displays the FlashCard menu and handles user selections for adding, deleting, editing, and viewing FlashCards.
    /// </summary>
    public static void DisplayFlashCardMenu()
    {
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold Purple]Welcome to Flash Cards![/]");
            AnsiConsole.MarkupLine("[Purple]Please select from the following options[/]");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "Add FlashCard", "Delete FlashCard", "Edit FlashCard", "View FlashCards", "Return to Main Menu"
                    ]));

            switch (selection)
            {
                case "Add FlashCard":
                    AddFlashCardMenu();
                    break;
                case "Delete FlashCard":
                    DeleteFlashCard();
                    break;
                case "Edit FlashCard":
                    EditFlashCard();
                    break;
                case "View FlashCards":
                    GetAndViewFlashCards();
                    break;
                case "Return to Main Menu":
                    continueProgram = false;
                    Console.WriteLine("Exiting FlashCard Menu, Press enter to continue");
                    Console.ReadLine();
                    break;
            }
        }
    }

    /// <summary>
    /// retrieves and displays FlashCards for a selected deck
    /// </summary>
    private static void GetAndViewFlashCards()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }
        
        var selectedDeck = DeckMenu.GetDeckSelection(decks);
        var flashCards = GetFlashCards(selectedDeck.Id);
        
        if (flashCards.Count == 0)
        {
            AnsiConsole.WriteLine("No FlashCards available for this deck. Press enter to continue.");
            Console.ReadLine();
            return;
        }

        ViewFlashCards(flashCards);
        AnsiConsole.WriteLine("Press enter to continue.");
        Console.ReadLine();
    }

    /// <summary>
    /// Menu for adding a new FlashCard to an existing deck or a new deck.
    /// </summary>
    public static void AddFlashCardMenu()
    {
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold Purple]Add a new flash card![/]");
            AnsiConsole.MarkupLine("[Purple]Please select from the following options[/]");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "Add to an existing Deck", "Add a card to a new Deck",
                        "Return to Flash Card Menu"
                    ]));

            switch (selection)
            {
                case "Add to an existing Deck":
                    DeckMenu.ViewDecks();
                    AddToExistingDeck();
                    break;
                case "Add a card to a new Deck":
                    AddToANewDeck();
                    break;
                case "Return to Flash Card Menu":
                    continueProgram = false;
                    AnsiConsole.WriteLine("Returning to Flash Card Menu, press enter to continue");
                    Console.ReadLine();
                    break;
            }
        }
    }

    /// <summary>
    /// Adds a new Deck then calls AddToExistingDeck to add a FlashCard to the new Deck.
    /// </summary>
    private static void AddToANewDeck()
    {
        DeckMenu.AddDeck();
        AddToExistingDeck();
    }

    /// <summary>
    /// Adds a new FlashCard to an existing Deck.
    /// </summary>
    private static void AddToExistingDeck()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }
        
        var selectedDeck = DeckMenu.GetDeckSelection(decks);
        var question = AnsiConsole.Ask<string>("Enter the question for the new FlashCard: ");
        var answer = AnsiConsole.Ask<string>("Enter the answer for the new FlashCard: ");
        var flashCard = new FlashCard
        {
            Question = question,
            Answer = answer,
            DeckId = selectedDeck.Id
        };
        var flashCards = GetFlashCards(selectedDeck.Id);
        if (flashCards.Values.Any(fc=>fc.Question == question))
        {
            AnsiConsole.WriteLine("FlashCard already exists in this deck. Press enter to continue.");
            Console.ReadLine();
            return;
        }
        context.FlashCards.Add(flashCard);
        context.SaveChanges();

        AnsiConsole.WriteLine("FlashCard added successfully. Press enter to continue.");
        Console.ReadLine();
    }

    /// <summary>
    /// Deletes a FlashCard from the database.
    /// </summary>
    public static void DeleteFlashCard()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }
        
        var selectedDeck = DeckMenu.GetDeckSelection(decks);
        var flashCards = GetFlashCards(selectedDeck.Id);
        if (flashCards.Count == 0)
        {
            AnsiConsole.WriteLine("No FlashCards available for this deck. Press enter to continue.");
            Console.ReadLine();
            return;
        }
        
        var flashCardDisplay = flashCards.Select(f => $"{f.Key}: {f.Value.Question}").ToList();
        var selectedFlashCard = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the Flash Card you would like to Delete:")
                .PageSize(10)
                .AddChoices(flashCardDisplay)
        );
        
        var flashCardId = int.Parse(selectedFlashCard.Split(':')[0]);
        if (!flashCards.TryGetValue(flashCardId, out var flashCard))
        {
            AnsiConsole.WriteLine("FlashCard not found. Press enter to continue.");
            Console.ReadLine();
            return;
        }

        context.FlashCards.Remove(flashCard);
        context.SaveChanges();
        AnsiConsole.WriteLine("FlashCard deleted successfully. Press enter to continue.");
        Console.ReadLine();
    }

    /// <summary>
    /// Edits a FlashCard in the database.
    /// </summary>
    public static void EditFlashCard()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }
        
        var selectedDeck = DeckMenu.GetDeckSelection(decks);
        var flashCards = GetFlashCards(selectedDeck.Id);
        if (flashCards.Count == 0)
        {
            AnsiConsole.WriteLine("No FlashCards available for this deck. Press enter to continue.");
            Console.ReadLine();
            return;
        }
        
        var flashCardDisplay = flashCards.Select(f => $"{f.Key}: {f.Value.Question}").ToList();
        var selectedFlashCard = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the Flash Card you would like to Delete:")
                .PageSize(10)
                .AddChoices(flashCardDisplay)
        );
        
        var flashCardId = int.Parse(selectedFlashCard.Split(':')[0]);
        if (!flashCards.TryGetValue(flashCardId, out var flashCard))
        {
            AnsiConsole.WriteLine("FlashCard not found. Press enter to continue.");
            Console.ReadLine();
            return;
        }

        var newQuestion = AnsiConsole.Ask<string>("Enter the new question for the FlashCard: ");
        var newAnswer = AnsiConsole.Ask<string>("Enter the new answer for the FlashCard: ");
        if (flashCards.Values.Any(fc=>fc.Question == newQuestion))
        {
            AnsiConsole.WriteLine("FlashCard already exists in this deck. Press enter to continue.");
            Console.ReadLine();
            return;
        }
            
        flashCard.Question = newQuestion;
        flashCard.Answer = newAnswer;
        context.FlashCards.Update(flashCard);
        context.SaveChanges();
        AnsiConsole.WriteLine("FlashCard updated successfully. Press enter to continue.");
        Console.ReadLine();
    }

    /// <summary>
    /// gets all FlashCards for a given deck
    /// </summary>
    /// <param name="deckId"></param>
    /// <returns>
    /// dictionary of FlashCards for a given deck with keys as the display index
    /// </returns>
    public static Dictionary<int, FlashCard> GetFlashCards(int deckId)
    {
        using var context = new FlashCardsContext();
        var flashCards = context.FlashCards
            .Where(fc => fc.DeckId == deckId)
            .ToList();
        return flashCards
            .Select((fc, index) => new { Index = index + 1, FlashCard = fc })
            .ToDictionary(item => item.Index, item => item.FlashCard);
    }

    /// <summary>
    /// Displays FlashCards in a table format
    /// </summary>
    /// <param name="flashCards"></param>
    /// <returns>
    /// Dictionary of FlashCards
    /// </returns>
    public static Dictionary<int, FlashCard> ViewFlashCards(Dictionary<int, FlashCard> flashCards)
    {
        var table = new Table();

        table.AddColumns(["Id", "Question", "Answer"]);
        Dictionary<int, FlashCard> flashCardLookup = new();
        int displayId = 1;
        foreach (var flashCard in flashCards.Values)
        {
            table.AddRow(
            [
                displayId.ToString(),
                flashCard.Question,
                flashCard.Answer
            ]);
            flashCardLookup.Add(displayId, flashCard);
            displayId++;
        }

        AnsiConsole.Write(table);
        return flashCardLookup;
    }
}