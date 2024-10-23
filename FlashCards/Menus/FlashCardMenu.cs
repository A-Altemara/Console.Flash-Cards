using FlashCards.Models;
using Spectre.Console;

namespace FlashCards.Menus;

public class FlashCardMenu()
{
    public static void DisplayFlashCardMenu()
    {
        Console.Clear();
        var continueProgram = true;

        while (continueProgram)
        {
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
                    AnsiConsole.WriteLine("View FlashCards, press enter to continue");
                    Console.ReadLine();
                    break;
                case "Return to Main Menu":
                    continueProgram = false;
                    Console.WriteLine("Exiting FlashCard Menu, Press enter to continue");
                    Console.ReadLine();
                    break;
            }
        }
    }

    public static void AddFlashCard()
    {
        AnsiConsole.WriteLine("Add A new FlashCard, press enter to continue");
        Console.ReadLine();
        Console.Clear();
        AnsiConsole.Markup("[bold Purple]Add a new flash card![/]\n");
        AnsiConsole.Markup("[Purple]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your Selection?")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "Add to an existing Deck", "Add a card to a new Deck", "Return to Flash Card Menu"
                ]));

        switch (selection)
        {
            case "add to an existing Deck":
                AddToExistingDeck();
                break;
            case "Add a card to a new Deck":
                AddToANewDeck();
                break;
            case "Return to Flash Card Menu":
                AnsiConsole.WriteLine("Returning to Flash Card Menu, press enter to continue");
                Console.ReadLine();
                break;
            
        }
        
    }

    private static void AddToANewDeck()
    {
        throw new NotImplementedException();
    }

    private static void AddToExistingDeck()
    {
        throw new NotImplementedException();
    }

    public static void DeleteFlashCard()
    {
        AnsiConsole.WriteLine("Delete a FlashCard, press enter to continue");
        Console.ReadLine();
    }

    public static void EditFlashCard()
    {
        AnsiConsole.WriteLine("Edit a FlashCard, press enter to continue");
        Console.ReadLine();
    }

    public static Dictionary<int, FlashCard> ViewFlashCards(List<FlashCard> flashCards)
    {
        var table = new Table();

        table.AddColumns(["Id", "Question", "Answer"]);
        Dictionary<int, FlashCard> flashCardLookup = new();
        for (int i = 0; i < flashCards.Count; i++)
        {
            int displayId = i + 1;
            table.AddRow(
            [
                displayId.ToString(),
                flashCards[i].Question,
                flashCards[i].Answer
            ]);
            flashCardLookup.Add(displayId, flashCards[i]);
        }

        AnsiConsole.Write(table);
        return flashCardLookup;
    }
}