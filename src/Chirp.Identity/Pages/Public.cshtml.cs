using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }
    public int CurrentPage { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage => CurrentPage > 0;

    public PublicModel(ICheepService service)
    {
        _service = service;
        Cheeps = new List<CheepDTO>();
    }

    public ActionResult OnGet([FromQuery] int page)
    {
        CurrentPage = page;
        Cheeps = _service.GetMainPageCheeps(page);
        HasNextPage = _service.GetMainPageCheeps(page + 1).Any();
        return Page();
    }
}
