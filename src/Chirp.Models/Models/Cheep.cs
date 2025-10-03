namespace Chirp.Models
{
    public record Cheep(string Text, DateTime TimeStamp, Author Author);

    public record Author(int AuthorID, string Name, string Email, IList<Cheep> Cheeps);
}
