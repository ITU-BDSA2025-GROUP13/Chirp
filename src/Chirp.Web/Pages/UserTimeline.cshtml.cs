using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    private readonly IChirpUserService _chirpUserService;
    public List<CheepDTO> Cheeps { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage => CurrentPage > 0;
    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Author { get; set; } = null!;

    [BindProperty]
    public int CheepIdForDeletion { get; set; }
    [BindProperty]
    public string? ToggleFollowForUserId { get; set; }

    public List<string>? FollowCache { get; set; }

    [BindProperty]
    public int CheepIdForEditing { get; set; }

    [BindProperty]
    public string? EditedCheepText { get; set; }

    
    public UserTimelineModel(ICheepService service, IChirpUserService chirpUserService)
    {
        _service = service;
        _chirpUserService = chirpUserService;
        Cheeps = new List<CheepDTO>();
    }

    public ActionResult OnGet()
    {
        if (User.Identity?.Name != null)
        {
            FollowCache = _service.GetListOfFollowerNames(User.Identity.Name);
        }
        else
        {
            FollowCache = new List<string>();
        }
        if (Author == User.Identity?.Name)
        {
            Cheeps = _service.GetOwnPrivateTimeline(Author, CurrentPage);
            HasNextPage = _service.GetOwnPrivateTimeline(Author, CurrentPage + 1).Any();
            return Page();
        }

        Cheeps = _service.GetCheepsFromAuthorName(Author, CurrentPage);
        HasNextPage = _service.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();
        return Page();
    }

    public ActionResult OnPostDelete()
    {
        _service.DeleteCheep(CheepIdForDeletion);
        return RedirectToPage("/Public");
    }

    public ActionResult OnPostFollow()
    {
        if (User.Identity?.Name != null && ToggleFollowForUserId != null)
            _chirpUserService.ToggleUserFollowing(User.Identity.Name, ToggleFollowForUserId);

        return RedirectToPage("/Public");
    }

    public ActionResult OnPostEdit()
    {
        HasNextPage = _service.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();

        if (!IsValidMessage(EditedCheepText))
        {
            Cheeps = _service.GetCheepsFromAuthorName(Author, CurrentPage);
            if (CurrentPage == 0)
            {
                return LocalRedirect($"/user/{Author}");
            }
            return LocalRedirect($"/user/{Author}?page={CurrentPage}");
        }

        _service.EditCheep(CheepIdForEditing, EditedCheepText!);
        Cheeps = _service.GetCheepsFromAuthorName(Author, CurrentPage);
        if (CurrentPage == 0)
        {
            return LocalRedirect($"/user/{Author}");
        }
        return LocalRedirect($"/user/{Author}?page={CurrentPage}");

    }

    public bool IsValidMessage(string? message)
    {
        if (message == null)
        {
            ErrorMessage = "Cheep message cannot be empty.";
            return false;
        }
        else if (message.Length > 160)
        {
            ErrorMessage = "Cheep message cannot be longer than 160 characters.";
            return false;
        }
        return true;
    }
}
