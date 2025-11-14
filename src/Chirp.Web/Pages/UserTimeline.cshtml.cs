using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage => CurrentPage > 0;

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Author { get; set; } = null!;

    [BindProperty]
    public int CheepIdForDeletion { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
        Cheeps = new List<CheepDTO>();
    }

    public ActionResult OnGet()
    {
        Cheeps = _service.GetCheepsFromAuthorName(Author, CurrentPage);
        HasNextPage = _service.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();
        return Page();
    }

    public ActionResult OnPostDelete()
    {
        _service.DeleteCheep(CheepIdForDeletion);
        return RedirectToPage("/Public");
    }
}
