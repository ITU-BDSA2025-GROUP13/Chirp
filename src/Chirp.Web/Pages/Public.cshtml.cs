using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
        Cheeps = new List<CheepDTO>();
    }

    public ActionResult OnGet([FromQuery] int page)
    {
        Console.WriteLine(page);
        Cheeps = _service.GetCheeps(page);
        return Page();
    }
}
