using FlashCards.Context;
using FlashCards.Models;
using Spectre.Console;

namespace FlashCards.Menus;

public class FlashCardMenu()
{
    public static void DisplayFlashCardMenu()
    {
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();
            AnsiConsole.Markup("[bold Purple]Welcome to Flash Cards![/]\n");
            AnsiConsole.Markup("[Purple]Please select from the following options[/]\n");
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
                    AddFlashCard();
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

    private static void GetAndViewFlashCards()
    {
        DeckMenu.ViewDecks();
        var deckId = AnsiConsole.Ask<int>("Which Deck ID would you like to view FlashCards for?");
        var flashCards = GetFlashCards(deckId);
        if (flashCards.Count == 0)
        {
            AnsiConsole.WriteLine("No FlashCards available for this deck. Press enter to continue.");
            Console.ReadLine();
            return;
        }

        ViewFlashCards(flashCards);
    }

    public static void AddFlashCard()
    {
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();
            AnsiConsole.Markup("[bold Purple]Add a new flash card![/]\n");
            AnsiConsole.Markup("[Purple]Please select from the following options[/]\n");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "Add to an existing Deck", "Add a card to a new Deck", "View FlashCards",
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
                case "View FlashCards":
                    DeckMenu.ViewDecks();
                    var deckId = AnsiConsole.Ask<int>("Which Deck ID would you like to view FlashCards for?");
                    var flashCards = GetFlashCards(deckId);
                    ViewFlashCards(flashCards);
                    AnsiConsole.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                    break;
                case "Return to Flash Card Menu":
                    continueProgram = false;
                    AnsiConsole.WriteLine("Returning to Flash Card Menu, press enter to continue");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private static void AddToANewDeck()
    {
        DeckMenu.AddDeck();
        DeckMenu.ViewDecks();
        AddToExistingDeck();
    }

    private static void AddToExistingDeck()
    {
        var deckId = AnsiConsole.Ask<int>("Enter the Deck Id for the new FlashCard: ");
        var question = AnsiConsole.Ask<string>("Enter the question for the new FlashCard: ");
        var answer = AnsiConsole.Ask<string>("Enter the answer for the new FlashCard: ");
        var flashCard = new FlashCard
        {
            Question = question,
            Answer = answer,
            DeckId = deckId
        };
        using (var context = new FlashCardsContext())
        {
            context.FlashCards.Add(flashCard);
            context.SaveChanges();
        }

        AnsiConsole.WriteLine("FlashCard added successfully. Press enter to continue.");
        Console.ReadLine();
    }

    public static void DeleteFlashCard()
    {
        DeckMenu.ViewDecks();
        var deckId = AnsiConsole.Ask<int>("What Deck would you like to delete a FlashCard from?");
        var flashCards = GetFlashCards(deckId);
        var flashCardLookup = ViewFlashCards(flashCards);
        var flashCardId = AnsiConsole.Ask<int>("Enter the Id of the FlashCard you would like to delete: ");
        if (!flashCardLookup.TryGetValue(flashCardId, out var flashCard))
        {
            AnsiConsole.WriteLine("FlashCard not found. Press enter to continue.");
            Console.ReadLine();
            return;
        }

        using (var context = new FlashCardsContext())
        {
            context.FlashCards.Remove(flashCard);
            context.SaveChanges();
        }
        AnsiConsole.WriteLine("FlashCard deleted successfully. Press enter to continue.");
        Console.ReadLine();
    }

    public static void EditFlashCard()
    {
        DeckMenu.ViewDecks();
        var deckId = AnsiConsole.Ask<int>("What Deck would you like to edit a FlashCard from?");
        var flashCards = GetFlashCards(deckId);
        var flashCardLookup = ViewFlashCards(flashCards);
        var flashCardId = AnsiConsole.Ask<int>("Enter the Id of the FlashCard you would like to edit: ");
        if (!flashCardLookup.TryGetValue(flashCardId, out var flashCard))
        {
            AnsiConsole.WriteLine("FlashCard not found. Press enter to continue.");
            Console.ReadLine();
            return;
        }

        var newQuestion = AnsiConsole.Ask<string>("Enter the new question for the FlashCard: ");
        var newAnswer = AnsiConsole.Ask<string>("Enter the new answer for the FlashCard: ");
        using (var context = new FlashCardsContext())
        {
            flashCard.Question = newQuestion;
            flashCard.Answer = newAnswer;
            context.FlashCards.Update(flashCard);
            context.SaveChanges();
        }
        AnsiConsole.WriteLine("FlashCard updated successfully. Press enter to continue.");
        Console.ReadLine();
    }

    private static Dictionary<int, FlashCard> GetFlashCards(int deckId)
    {
        using (var context = new FlashCardsContext())
        {
            var flashCards = context.FlashCards
                .Where(fc => fc.DeckId == deckId)
                .ToList();
            return flashCards
                .Select((fc, index) => new { Index = index + 1, FlashCard = fc })
                .ToDictionary(item => item.Index, item => item.FlashCard);
        }
    }

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