using Chirp.Core.Models;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    private readonly IChirpUserService _chirpUserService;
    private readonly UserManager<ChirpUser> _userManager;
    public List<CheepDTO> Cheeps { get; set; }

    // Pagination
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage => CurrentPage > 0;

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; }

    // Posting
    [BindProperty]
    public string? CheepMessage { get; set; }
    public string? ErrorMessage { get; set; }

    // Deleting
    [BindProperty]
    public int CheepIdForDeletion { get; set; }
    [BindProperty]
    public string? ToggleFollowForUserId { get; set; }

    // Replying
    [BindProperty]
    public CheepReply? Reply { get; set; }

    [BindProperty]
    public int CheepIdForEditing { get; set; }

    [BindProperty]
    public string? EditedCheepText { get; set; }

    [BindProperty]
    public int CheepIdForLike { get; set; }

    public PublicModel(ICheepService service, IChirpUserService chirpUserService, UserManager<ChirpUser> userManager)
    {
        _service = service;
        _userManager = userManager;
        _chirpUserService = chirpUserService;
        Cheeps = new List<CheepDTO>();
    }

    public ActionResult OnGet()
    {
        if (User.Identity?.Name != null)
        {
            var followedUsers = _service.GetListOfNamesOfFollowedUsers(User.Identity.Name);
            CheepDataCache.Instance.SetFollowedUsers(User.Identity.Name, followedUsers);
        }

        Cheeps = _service.GetMainPageCheeps(CurrentPage);
        HasNextPage = _service.GetMainPageCheeps(CurrentPage + 1).Any();
        LoadLikedCheeps();
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
            LoadLikedCheeps();
            return Page();
        }

        string? name = User.Identity?.Name;
        if (name == null)
        {
            ErrorMessage = "You must be logged in to post a cheep.";
            CurrentPage = 0;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            LoadLikedCheeps();
            return Page();
        }

        ChirpUser? user = _userManager.FindByNameAsync(name).GetAwaiter().GetResult();
        if (user == null)
        {
            ErrorMessage = "User not found!";
            CurrentPage = 0;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            LoadLikedCheeps();
            return Page();
        }

        _service.PostCheep(CheepMessage!, user.Id);
        return RedirectToPage("/Public");
    }

    public ActionResult OnPostDelete()
    {
        HasNextPage = _service.GetMainPageCheeps(CurrentPage + 1).Any();

        _service.DeleteCheep(CheepIdForDeletion);

        Cheeps = _service.GetMainPageCheeps(CurrentPage);
        LoadLikedCheeps();
        return Page();
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
            if (CurrentPage == 0)
            {
                return RedirectToPage("/Public");
            }
            LoadLikedCheeps();
            return RedirectToPage($"/Public?page={CurrentPage}");
        }

        _service.EditCheep(CheepIdForEditing, EditedCheepText!);
        Cheeps = _service.GetMainPageCheeps(CurrentPage);
        if (CurrentPage == 0)
        {
            return RedirectToPage("/Public");
        }
        return RedirectToPage($"/Public?page={CurrentPage}");
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

        if (Reply == null || Reply.Text == null)
        {
            ErrorMessage = "Reply not found!";
            CurrentPage = 0;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }

        _service.ReplyToCheep(Reply.CheepID, Reply.Text, user);
        return RedirectToPage("/Public");
    }

    private void LoadLikedCheeps()
    {
        if (User?.Identity != null && User.Identity.IsAuthenticated)
        {
            string? name = User.Identity.Name;
            if (name != null)
            {
                ChirpUser? user = _userManager.Users
                    .Include(u => u.LikedCheeps)
                    .FirstOrDefault(u => u.UserName == name);
                if (user != null)
                {
                    CheepDataCache.Instance.SetLikedCheeps(name, user.LikedCheeps.Select(c => c.CheepId).ToHashSet());
                }
            }
        }
    }

    public ActionResult OnPostLike()
    {
        HasNextPage = _service.GetMainPageCheeps(CurrentPage + 1).Any();

        string? name = User.Identity?.Name;
        if (name == null)
        {
            ErrorMessage = "You must be logged in to like a cheep.";
            Cheeps = _service.GetMainPageCheeps(CurrentPage);
            LoadLikedCheeps();
            return Page();
        }

        ChirpUser? user = _userManager.Users
            .Include(u => u.LikedCheeps)
            .FirstOrDefault(u => u.UserName == name);
        if (user == null)
        {
            ErrorMessage = "User not found!";
            Cheeps = _service.GetMainPageCheeps(CurrentPage);
            LoadLikedCheeps();
            return Page();
        }

        if (user.LikedCheeps.Any(c => c.CheepId == CheepIdForLike))
        {
            _service.UnLikeCheep(CheepIdForLike, user.Id);
        }
        else
        {
            _service.LikeCheep(CheepIdForLike, user.Id);
        }

        Cheeps = _service.GetMainPageCheeps(CurrentPage);
        LoadLikedCheeps();
        return Page();
    }
}

/// <summary>
/// <param name="CheepID">The ID of the Cheep that's replied to</param>
/// <param name="Reply">The Reply to the Cheep</param>
/// </summary>
public class CheepReply
{
    public int CheepID { get; set; }
    public string? Text { get; set; }
}
