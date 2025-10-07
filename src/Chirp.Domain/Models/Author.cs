namespace Chirp.Domain
{
    public record Author(int AuthorID, string Name, string Email, IList<Cheep> Cheeps);
}
