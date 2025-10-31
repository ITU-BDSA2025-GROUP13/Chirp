using Chirp.Infrastructure.Services;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }
    public int CurrentPage { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage => CurrentPage > 0;

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
        Cheeps = new List<CheepDTO>();
    }

    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        CurrentPage = page;
        Cheeps = _service.GetCheepsFromAuthorName(author, page);
        HasNextPage = _service.GetCheepsFromAuthorName(author, page + 1).Any();
        return Page();
    }
}
