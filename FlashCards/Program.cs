using FlashCards.Menus;
using Spectre.Console;
    
namespace FlashCards;

class Program
{
    static void Main(string[] args)
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
                        "Manage Flash Cards", "Manage Decks", "Study Flash Cards", "View Study Sessions", "Exit Program"
                    ]));

            switch (selection)
            {
                case "Manage Flash Cards":
                    FlashCardMenu.DisplayFlashCardMenu();
                    break;
                case "Manage Decks":
                    AnsiConsole.WriteLine("Manage Decks Menu, press enter to continue");
                    Console.ReadLine();
                    break;
                case "Study Flash Cards":
                    AnsiConsole.WriteLine("Study Flash Cards, press enter to continue");
                    Console.ReadLine();
                    break;
                case "View Study Sessions":
                    AnsiConsole.WriteLine("View Study Sessions, press enter to continue");
                    Console.ReadLine();
                    break;
                case "Exit Program":
                    continueProgram = false;
                    Console.WriteLine("Exited Program");
                    break;
            }
        }
    }
}