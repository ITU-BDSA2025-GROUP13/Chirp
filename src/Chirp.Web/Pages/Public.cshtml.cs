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
    public string? ErrorMessage { get; set; }

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
        if (CheepMessage == null)
        {
            ErrorMessage = "Cheep message cannot be empty.";
            CurrentPage = 0;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }
        
        if (CheepMessage.Length > 160)
        {
            ErrorMessage = "Cheep message cannot be longer than 160 characters.";
            CurrentPage = 0;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }
        
        string? name = User.Identity?.Name;
        if (name == null)
        {
            ErrorMessage = "You must be logged in to post a cheep.";
            CurrentPage = 0;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }
        
        ChirpUser? user = _userManager.FindByNameAsync(name).GetAwaiter().GetResult();
        if (user == null)
        {
            ErrorMessage = "User not found!";
            CurrentPage = 0;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }
        
        _service.PostCheep(CheepMessage, user.Id);
        return RedirectToPage("/Public");
    }
}
