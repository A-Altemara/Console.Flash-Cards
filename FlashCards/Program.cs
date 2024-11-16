using FlashCards.Menus;
using Spectre.Console;
    
namespace FlashCards;

public static class Program
{
    /// <summary>
    /// Entry point for the FlashCards program. Displays the main menu and handles user selections.
    /// </summary>
    private static async Task Main()
    {
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();

            AnsiConsole.MarkupLine("[bold Blue]Welcome to Flash Cards![/]");
            AnsiConsole.MarkupLine("[blue]Please select from the following options[/]");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "Manage Flash Cards", "Manage Decks", "Study Menu", "Reports Menu", "Exit Program"
                    ]));

            switch (selection)
            {
                case "Manage Flash Cards":
                    FlashCardMenu.DisplayFlashCardMenu();
                    break;
                case "Manage Decks":
                   await DeckMenu.DisplayDecksMenu();
                    break;
                case "Study Menu":
                    StudySessionMenu.DisplayFlashCardMenu();
                    Console.ReadLine();
                    break;
                case "Reports Menu":
                    ReportsMenu.DisplayFlashCardMenu();
                    break;
                case "Exit Program":
                    continueProgram = false;
                    Console.WriteLine("Exited Program");
                    break;
            }
        }
    }
}