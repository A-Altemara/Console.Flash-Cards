namespace FlashCards.Models;

public class StudySession
{
    public int Id { get; set; }
    public string Answers { get; set; }
    public string Correct { get; set; }
    public int DeckId { get; set; }
    public Deck Deck { get; set; }
}