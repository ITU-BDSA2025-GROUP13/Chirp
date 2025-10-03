using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Domain;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<Cheep> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
        Cheeps = new List<Cheep>();
    }

    public ActionResult OnGet([FromQuery] int page)
    {
        Console.WriteLine(page);
        Cheeps = _service.GetCheeps(page);
        return Page();
    }
}
