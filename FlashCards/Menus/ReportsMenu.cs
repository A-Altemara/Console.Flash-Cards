using FlashCards.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace FlashCards.Menus;

public class ReportsMenu
{
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
                        "View total Study Sessions by Month", "View Average Study Session score over a Year",
                        "Return to Main Menu"
                    ]));

            switch (selection)
            {
                case "View total Study Sessions by Month":
                    TotalStudySessionsByMonth();
                    break;
                case "View Average Study Session score over a Year":
                    AverageStudySessionScoreOverYear();
                    break;
                case "Return to Main Menu":
                    continueProgram = false;
                    Console.WriteLine("Exiting FlashCard Menu, Press enter to continue");
                    break;
            }
        }
    }

private static void AverageStudySessionScoreOverYear()
{
    using var context = new FlashCardsContext();
    var decks = context.Decks.ToList();

    if (decks.Count == 0)
    {
        return;
    }

    var selectedDeck = DeckMenu.GetDeckSelection(decks);
    var year = AnsiConsole.Ask<int>("Enter the year for which you want to view the average study session scores:");

    var studySessions = context.StudySessions
        .Where(ss => ss.DeckStudiedId == selectedDeck.Id && ss.DateStudied.Year == year)
        .Select(ss => new
        {
            Month = ss.DateStudied.Month,
            Score = (double)ss.NumberCorrect / ss.NumberAsked * 100
        })
        .ToList();

    var monthlyScores = studySessions
        .GroupBy(ss => ss.Month)
        .Select(g => new
        {
            Month = g.Key,
            AverageScore = g.Average(ss => ss.Score)
        })
        .OrderBy(ms => ms.Month)
        .ToList();

    var table = new Table();
    table.AddColumn("Month");
    table.AddColumn("Average Score");

    for (int i = 1; i <= 12; i++)
    {
        var monthName = new DateTime(year, i, 1).ToString("MMMM");
        var averageScore = monthlyScores.FirstOrDefault(ms => ms.Month == i)?.AverageScore ?? 0;
        table.AddRow(monthName, averageScore.ToString("0.00") + "%");
    }

    AnsiConsole.Write(table);

    AnsiConsole.WriteLine("Press enter to continue.");
    Console.ReadLine();
}

    private static void TotalStudySessionsByMonth()
    {
        using var context = new FlashCardsContext();
        var decks = context.Decks.ToList();

        if (decks.Count == 0)
        {
            return;
        }

        var selectedDeck = DeckMenu.GetDeckSelection(decks);
        var year = AnsiConsole.Ask<int>("Enter the year for which you want to view the total number of study session:");
        
        var studySessions = context.StudySessions
            .Where(ss => ss.DeckStudiedId == selectedDeck.Id && ss.DateStudied.Year == year)
            .Select(ss => new
            {
                Month = ss.DateStudied.Month,
                StudySessionCount = 1
            })
            .ToList();

        var monthlyStudySessionCounts = studySessions
            .GroupBy(ss => ss.Month)
            .Select(g => new
            {
                Month = g.Key,
                StudySessionCount = g.Sum(ss => ss.StudySessionCount)
            });
        
        var table = new Table();
        table.AddColumn("Month");
        table.AddColumn("Total Study Sessions");
        
        for (int i = 1; i <= 12; i++)
        {
            var monthName = new DateTime(year, i, 1).ToString("MMMM");
            var studySessionCount = monthlyStudySessionCounts.FirstOrDefault(ms => ms.Month == i)?.StudySessionCount ?? 0;
            table.AddRow(monthName, studySessionCount.ToString());
        }
        
        AnsiConsole.Write(table);

        AnsiConsole.WriteLine("Press enter to continue.");
        Console.ReadLine();
    }
}