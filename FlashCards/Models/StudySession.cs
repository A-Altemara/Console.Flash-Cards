namespace FlashCards.Models;

public class StudySession
{
    public int Id { get; set; }
    public int NumberAsked { get; set; }
    public int NumberCorrect { get; set; }
    public int DeckStudiedId { get; set; } 
    public required Deck DeckStudied { get; set; }
}