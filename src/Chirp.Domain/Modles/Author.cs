namespace Chirp.Domain
{
    public record Author(int Id, string Message, List<Cheep> Cheeps);
}
