namespace FlashCards.Models;

public class Deck()
{
    public int Id { get; set; }
    public string DeckName { get; set; }
    public ICollection<FlashCard> FlashCards { get; set; }
    public ICollection<StudySession> StudySessions { get; set; }
}