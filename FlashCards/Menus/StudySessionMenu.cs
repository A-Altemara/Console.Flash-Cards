using System.Runtime.InteropServices.JavaScript;
using FlashCards.Context;
using FlashCards.Demos;
using FlashCards.Models;
using Spectre.Console;

namespace FlashCards.Menus;

public static class StudySessionMenu
{
    /// <summary>
    /// Displays the FlashCard menu and handles user selections for studying flashcards, viewing all study sessions, 
    /// viewing deck-specific study sessions, or returning to the main menu.
    /// </summary>
    public static void DisplayFlashCardMenu()
    {
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold blue]Welcome to Study Sessions![/]");
            AnsiConsole.MarkupLine("[blue]Study Sessions can not be edited or deleted.[/]");
            AnsiConsole.MarkupLine("[blue]Please select from the following options[/]");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "Study FlashCards", "Add Demo Study Sessions", "View All Study Sessions",
                        "View Deck specific Study sessions",
                        "Return to Main Menu"
                    ]));

            switch (selection)
            {
                case "Study FlashCards":
                    StudyFlashCards();
                    break;
                case "Add Demo Study Sessions":
                    DemoStudySessionBuilder.BuildRandomStudySessions();
                    break;
                case "View All Study Sessions":
                    ViewAllStudySessions();
                    break;
                case "View Deck specific Study sessions":
                    ViewDeckSpecificStudySessions();
                    break;
                case "Return to Main Menu":
                    continueProgram = false;
                    Console.WriteLine("Exiting FlashCard Menu, Press enter to continue");
                    break;
            }
        }
    }

    /// <summary>
    /// Displays all study sessions for all decks.
    /// </summary>
    private static void ViewAllStudySessions()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            AnsiConsole.WriteLine("No decks available. Press enter to continue");
            Console.ReadLine();
            return;
        }

        foreach (var deck in decks)
        {
            var table = new Table();
            table.AddColumns("ID", "Deck Name", "Date Studied DD/MM/YYY", "Number Asked", "Number Correct",
                "Percent Correct");

            var studySessions = GetStudySessions(deck.Id);
            if (studySessions.Count == 0)
            {
                AnsiConsole.MarkupLine($"[Red]No Study Sessions available for {deck.DeckName}.[/]");
                continue;
            }

            foreach (var studySession in studySessions)
            {
                var studySessionDate = studySession.DateStudied.Day + "/" + studySession.DateStudied.Month + "/" +
                                       studySession.DateStudied.Year;
                var percentCorrect = (decimal)studySession.NumberCorrect / studySession.NumberAsked * 100;
                table.AddRow(studySession.Id.ToString(), deck.DeckName, studySessionDate,
                    studySession.NumberAsked.ToString(),
                    studySession.NumberCorrect.ToString(), percentCorrect.ToString("0.00") + "%");
            }

            AnsiConsole.Write(table);
        }

        AnsiConsole.WriteLine("Press enter to continue.");
        Console.ReadLine();
    }

    /// <summary>
    /// displays all study sessions for a specific deck.
    /// </summary>
    private static void ViewDeckSpecificStudySessions()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }

        var selectedDeck = DeckMenu.GetDeckSelection(decks);
        var studySessions = GetStudySessions(selectedDeck.Id);

        if (studySessions.Count == 0)
        {
            AnsiConsole.WriteLine($"No Study Sessions available for {selectedDeck.DeckName}.");
            return;
        }

        var table = new Table();
        table.AddColumns("Study Session ID", "Deck Name", "Date Studied DD/MM/YYY", "Number Asked", "Number Correct",
            "Percent Correct");

        foreach (var studySession in studySessions)
        {
            var studySessionDate = studySession.DateStudied.Day + "/" + studySession.DateStudied.Month + "/" +
                                   studySession.DateStudied.Year;
            var percentCorrect = (decimal)studySession.NumberCorrect / studySession.NumberAsked * 100;
            table.AddRow(studySession.Id.ToString(), selectedDeck.DeckName, studySessionDate,
                studySession.NumberAsked.ToString(),
                studySession.NumberCorrect.ToString(), percentCorrect.ToString("0.00") + "%");
        }

        AnsiConsole.Write(table);

        AnsiConsole.WriteLine("Press enter to continue.");
        Console.ReadLine();
    }

    /// <summary>
    ///  Retrieves all study sessions for a specific deck.
    /// </summary>
    /// <param name="deckId"></param>
    /// <returns>
    /// list of study sessions for a specific deck.
    /// </returns>
    private static List<StudySession> GetStudySessions(int deckId)
    {
        using var context = new FlashCardsContext();
        var studySessions = context.StudySessions
            .Where(s => s.DeckStudiedId == deckId)
            .ToList();
        return studySessions;
    }

    /// <summary>
    /// Creates randomized list of flashcards from a selected deck and prompts the user to answer each flashcard.
    /// </summary>
    private static void StudyFlashCards()
    {
        Random random = new Random();
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }

        var selectedDeck = DeckMenu.GetDeckSelection(decks);
        var studySession = new StudySession { DeckStudied = selectedDeck };

        var studySessionCount = AnsiConsole.Prompt(new TextPrompt<int>("How Many FlashCards would you like to study?")
            .Validate(number =>
                number > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Please enter a number greater than 0")));
            

        var flashCards = FlashCardMenu.GetFlashCards(selectedDeck.Id);
        if (flashCards.Count == 0)
        {
            AnsiConsole.WriteLine("No FlashCards available for this deck. Press enter to continue.");
            Console.ReadLine();
            return;
        }

        var randomizedFlashCards = flashCards.Values.OrderBy(f => random.Next()).ToList();
        var studyList = randomizedFlashCards.Take(studySessionCount).ToList();

        foreach (var flashCard in studyList)
        {
            Console.Clear();
            var userAnswer = AnsiConsole.Ask<string>($"[bold blue]Question:[/]\n{flashCard.Question}");
            studySession.NumberAsked++;

            if (userAnswer == flashCard.Answer)
            {
                Console.WriteLine("Correct!");
                studySession.NumberCorrect++;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Incorrect![/]");
                AnsiConsole.WriteLine($"The correct answer is: {flashCard.Answer}");
            }

            AnsiConsole.WriteLine("press enter to continue");
            Console.ReadLine();
        }

        AnsiConsole.WriteLine(
            $"You have completed the study session. You scored {(decimal)studySession.NumberCorrect / studySession.NumberAsked:P2}  Press enter to continue.");
        Console.ReadLine();

        studySession.DateStudied = DateTime.Now;
        studySession.DeckStudied = selectedDeck;
        context.StudySessions.Add(studySession);
        context.SaveChanges();
    }
}