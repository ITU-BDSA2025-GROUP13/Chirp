using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

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
        Cheeps = _service.GetMainPageCheeps(page);
        return Page();
    }
}
