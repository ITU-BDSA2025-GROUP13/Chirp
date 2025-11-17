using Chirp.Core.Models;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    private readonly IChirpUserService _chirpUserService;
    private readonly UserManager<ChirpUser> _userManager;
    public List<CheepDTO> Cheeps { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage => CurrentPage > 0;

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; }

    [BindProperty]
    public string? CheepMessage { get; set; }
    public string? ErrorMessage { get; set; }
    [BindProperty]
    public int CheepIdForDeletion { get; set; }
    [BindProperty]
    public string? ToggleFollowForUserId { get; set; }

    [BindProperty]
    public CheepReply? Reply { get; set; }

    [BindProperty]
    public int CheepIdForEditing { get; set; }

    [BindProperty]
    public string? EditedCheepText { get; set; }

    public PublicModel(ICheepService service, IChirpUserService chirpUserService, UserManager<ChirpUser> userManager)
    {
        _service = service;
        _userManager = userManager;
        _chirpUserService = chirpUserService;
        Cheeps = new List<CheepDTO>();
    }

    public ActionResult OnGet()
    {
        Cheeps = _service.GetMainPageCheeps(CurrentPage);
        HasNextPage = _service.GetMainPageCheeps(CurrentPage + 1).Any();
        return Page();
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

    public ActionResult OnPost()
    {
        Cheeps = _service.GetMainPageCheeps();
        HasNextPage = _service.GetMainPageCheeps(1).Any();

        if (!IsValidMessage(CheepMessage))
        {
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

        _service.PostCheep(CheepMessage!, user.Id);
        return RedirectToPage("/Public");
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
        HasNextPage = _service.GetMainPageCheeps(CurrentPage + 1).Any();

        if (!IsValidMessage(EditedCheepText))
        {
            Cheeps = _service.GetMainPageCheeps(CurrentPage);
            return Page();
        }

        _service.EditCheep(CheepIdForEditing, EditedCheepText!);
        Cheeps = _service.GetMainPageCheeps(CurrentPage);
        return Page();
    }

    public ActionResult OnPostReply()
    {
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
            ErrorMessage = "User not found! Try logging out and in again.";
            CurrentPage = 0;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }

        if (Reply == null || Reply.Reply == null)
        {
            ErrorMessage = "Reply not found!";
            CurrentPage = 0;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }

        _service.ReplyToCheep(Reply.CheepID, Reply.Reply, user);
        return RedirectToPage("/Public");
    }
}

/// <summary>
/// <param name="CheepID">The ID of the Cheep that's replied to</param>
/// <param name="Reply">The Reply to the Cheep</param>
/// </summary>
public class CheepReply
{
    public int CheepID { get; set; }
    public string? Reply { get; set; }
}
