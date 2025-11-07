using Chirp.Core.Models;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    private readonly UserManager<ChirpUser> _userManager;
    public List<CheepDTO> Cheeps { get; set; }
    public int CurrentPage { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage => CurrentPage > 0;
    [BindProperty]
    public string? CheepMessage { get; set; }

    public PublicModel(ICheepService service, UserManager<ChirpUser> userManager)
    {
        _service = service;
        _userManager = userManager;
        Cheeps = new List<CheepDTO>();
    }

    public ActionResult OnGet([FromQuery] int page)
    {
        CurrentPage = page;
        Cheeps = _service.GetMainPageCheeps(page);
        HasNextPage = _service.GetMainPageCheeps(page + 1).Any();
        return Page();
    }

    public ActionResult OnPost()
    {
        if (CheepMessage == null) return RedirectToPage("/Public");
        string? name = User.Identity?.Name;
        if (name == null) return RedirectToPage("/Public"); //TODO: Maybe error message for user here? (And below)
        ChirpUser? user = _userManager.FindByNameAsync(name).GetAwaiter().GetResult();
        if (user == null) return RedirectToPage("/Public");
        _service.PostCheep(CheepMessage, user.Id);
        return RedirectToPage("/Public");
    }
}
