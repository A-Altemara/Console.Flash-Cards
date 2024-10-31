using FlashCards.Context;
using FlashCards.Models;
using Spectre.Console;

namespace FlashCards.Menus;

public class StudySessionMenu
{
    public static void DisplayFlashCardMenu()
    {
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();
            AnsiConsole.Markup("[bold blue]Welcome to Study Sessions![/]\n");
            AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "Study FlashCards", "View All Study Sessions", "View Deck specific Study sessions",
                        "Return to Main Menu"
                    ]));

            switch (selection)
            {
                case "Study FlashCards":
                    StudyFlashCards();
                    break;
                case "View All Study Sessions":
                    ViewAllStudySessions();
                    break;
                case "View Deck specific Study sessions":
                    ViewStudySessions();
                    break;
                case "Return to Main Menu":
                    continueProgram = false;
                    Console.WriteLine("Exiting FlashCard Menu, Press enter to continue");
                    break;
            }
        }
    }

    private static void ViewAllStudySessions()
    {
        using (var context = new FlashCardsContext())
        {
            var decks = context.Decks.ToList();

            if (decks.Count == 0)
            {
                AnsiConsole.WriteLine("No decks available. Press enter to continue");
                Console.ReadLine();
                return;
            }

            var deckIds = decks.Select(d => d.Id).ToList();

            var table = new Table();
            table.AddColumns("ID", "Deck Name", "Number Asked", "Number Correct", "Percent Correct");

            foreach (var deckid in deckIds)
            {
                var studySessions = GetStudySessions(deckid);
                var selectedDeck = decks.FirstOrDefault(d => d.Id == deckid);
                if (studySessions.Count == 0)
                {
                    AnsiConsole.WriteLine($"No Study Sessions available for {selectedDeck.DeckName} Press enter to continue.");
                    Console.ReadLine();
                }

                foreach (var studySession in studySessions)
                {
                    var percentCorrect = (decimal)studySession.NumberCorrect / studySession.NumberAsked * 100;
                    table.AddRow(studySession.Id.ToString(), selectedDeck.DeckName,
                        studySession.NumberAsked.ToString(),
                        studySession.NumberCorrect.ToString(), percentCorrect.ToString("0.00") + "%");
                }
            }

            AnsiConsole.Write(table);
        }
        AnsiConsole.WriteLine("Press enter to continue.");
        Console.ReadLine();
    }

    private static void ViewStudySessions()
    {
        using (var context = new FlashCardsContext())
        {
            var decks = context.Decks.ToList();

            if (decks.Count == 0)
            {
                return;
            }

            var deckIds = decks.Select(d => d.Id).ToList();
            var selectedDeckId = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the deck you would like to edit:")
                    .PageSize(10)
                    .AddChoices(deckIds.Select(id => id.ToString()))
            );
            
            var deckId = int.Parse(selectedDeckId);
            var selectedDeck = decks.FirstOrDefault(d => d.Id == deckId);
            var studySessions = GetStudySessions(deckId);
            if (studySessions.Count == 0)
            {
                AnsiConsole.WriteLine($"No Study Sessions available for {selectedDeck.DeckName}. Press enter to continue.");
                Console.ReadLine();
                return;
            }

            decimal percentCorrect;

            var table = new Table();
            table.AddColumns("Study Session ID", "Deck Name", "Number Asked", "Number Correct", "Percent Correct");

            foreach (var studySession in studySessions)
            {
                percentCorrect = (decimal)studySession.NumberCorrect / studySession.NumberAsked * 100;
                table.AddRow(studySession.Id.ToString(), selectedDeck.DeckName,
                    studySession.NumberAsked.ToString(),
                    studySession.NumberCorrect.ToString(), percentCorrect.ToString("0.00") + "%");
            }

            AnsiConsole.Write(table);
        }
        AnsiConsole.WriteLine("Press enter to continue.");
        Console.ReadLine();
    }

    private static List<StudySession> GetStudySessions(int deckid)
    {
        using (var context = new FlashCardsContext())
        {
            var studySessions = context.StudySessions
                .Where(s => s.DeckStudiedId == deckid)
                .ToList();
            return studySessions;
        }
    }

    private static void StudyFlashCards()
    {
        Random random = new Random();
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }

        var deckLookup = decks.ToDictionary(d => $"{d.Id}: {d.DeckName}", d => d);
        var selectedDeckKey = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the deck you would like to study from:")
                .PageSize(10)
                .AddChoices(deckLookup.Keys)
        );
            
        var selectedDeck = deckLookup[selectedDeckKey];
        var studySession = new StudySession { DeckStudied = selectedDeck };

        var deckId = selectedDeck.Id;
        var studySessionCount = AnsiConsole.Ask<int>("How many FlashCards would you like to study?");
        var flashCards = FlashCardMenu.GetFlashCards(deckId);
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
                AnsiConsole.WriteLine("[red]Incorrect![/]\n");
                AnsiConsole.WriteLine($"The correct answer is: {flashCard.Answer}");
            }

            AnsiConsole.WriteLine("press enter to continue");
            Console.ReadLine();
        }

        studySession.DeckStudied = selectedDeck;
        context.StudySessions.Add(studySession);
        context.SaveChanges();
    }
}