using FlashCards.Demos;
using FlashCards.Menus;
using Spectre.Console;
    
namespace FlashCards;

class Program
{
    /// <summary>
    /// Entry point for the FlashCards program. Displays the main menu and handles user selections.
    /// </summary>
    static void Main()
    {
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();

            AnsiConsole.Markup("[bold Blue]Welcome to Flash Cards![/]\n");
            AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "Manage Flash Cards", "Manage Decks", "Study Menu", "Add Demo Stacks", "Exit Program"
                    ]));

            switch (selection)
            {
                case "Manage Flash Cards":
                    FlashCardMenu.DisplayFlashCardMenu();
                    break;
                case "Manage Decks":
                    DeckMenu.DisplayDecksMenu();
                    break;
                case "Study Menu":
                    StudySessionMenu.DisplayFlashCardMenu();
                    Console.ReadLine();
                    break;
                case "Add Demo Stacks":
                    DemoStackBuilder.ImportDemoStack();
                    break;
                case "Exit Program":
                    continueProgram = false;
                    Console.WriteLine("Exited Program");
                    break;
            }
        }
    }
}