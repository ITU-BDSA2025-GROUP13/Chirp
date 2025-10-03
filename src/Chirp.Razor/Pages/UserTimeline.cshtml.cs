using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Domain;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<Cheep> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
        Cheeps = new List<Cheep>();
    }

    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        Console.WriteLine($"{author}, {page}");
        Cheeps = _service.GetCheepsFromAuthor(author, page);
        return Page();
    }
}
