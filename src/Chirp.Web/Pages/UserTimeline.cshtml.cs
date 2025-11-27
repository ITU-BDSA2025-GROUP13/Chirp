using Chirp.Core.Models;
using Chirp.Infrastructure.Services;
using Chirp.Web.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IChirpUserService _chirpUserService;
    private readonly UserManager<ChirpUser> _userManager;
    public List<CheepDTO> Cheeps { get; set; }
    public string? ErrorMessage { get; set; }

    // Pagination
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; }

    // Author?
    [BindProperty(SupportsGet = true)]
    public string Author { get; set; } = null!;

    // Deleting
    [BindProperty]
    public int CheepIdForDeletion { get; set; }

    // Following
    [BindProperty]
    public string? ToggleFollowForUserId { get; set; }

    // Replying
    [BindProperty]
    public CheepReply? Reply { get; set; }


    // Editing
    [BindProperty]
    public int CheepIdForEditing { get; set; }

    [BindProperty]
    public string? EditedCheepText { get; set; }

    // Liking
    [BindProperty]
    public int CheepIdForLike { get; set; }

    public UserTimelineModel(ICheepService service, IChirpUserService chirpUserService, UserManager<ChirpUser> userManager)
    {
        _cheepService = service;
        _chirpUserService = chirpUserService;
        _userManager = userManager;
        Cheeps = new List<CheepDTO>();
        CurrentPage = 1;
    }

    public ActionResult OnGet()
    {
        if (User.Identity?.Name != null)
        {
            var followedUsers = _chirpUserService.GetFollowedUsernames(User.Identity.Name);
            CheepDataCache.Instance.SetFollowedUsers(User.Identity.Name, followedUsers);
        }
        LoadTimeline(CurrentPage);
        LoadLikes();
        return Page();
    }

    public ActionResult OnPostDelete()
    {
        HasNextPage = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();

        _cheepService.DeleteCheep(CheepIdForDeletion);
        return LocalRedirect($"/user/{Author}?page={CurrentPage}");
    }

    public ActionResult OnPostFollow()
    {
        if (User.Identity?.Name != null && ToggleFollowForUserId != null)
        {
            _chirpUserService.ToggleUserFollowing(User.Identity.Name, ToggleFollowForUserId);
        }

        LoadLikes();
        Cheeps = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage);
        return LocalRedirect($"/user/{Author}?page={CurrentPage}");
    }

    public ActionResult OnPostEdit()
    {
        HasNextPage = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();

        if (!IsValidMessage(EditedCheepText))
        {
            Cheeps = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage);
            if (CurrentPage == 0)
            {
                return LocalRedirect($"/user/{Author}");
            }
            LoadLikes();
            return LocalRedirect($"/user/{Author}?page={CurrentPage}");
        }

        _cheepService.EditCheep(CheepIdForEditing, EditedCheepText!);
        Cheeps = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage);
        if (CurrentPage == 0)
        {
            return LocalRedirect($"/user/{Author}");
        }
        return LocalRedirect($"/user/{Author}?page={CurrentPage}");
    }

    public ActionResult OnPostReply()
    {
        string? name = User.Identity?.Name;
        if (name == null)
        {
            ErrorMessage = "You must be logged in to post a cheep.";
            LoadTimeline(1);
            return Page();
        }

        ChirpUser? user = _userManager.FindByNameAsync(name).GetAwaiter().GetResult();
        if (user == null)
        {
            ErrorMessage = "User not found! Try logging out and in again.";
            LoadTimeline(1);
            return Page();
        }

        if (Reply == null || Reply.Text == null)
        {
            ErrorMessage = "Reply not found!";
            LoadTimeline(CurrentPage);
            return Page();
        }

        LoadLikes();
        _cheepService.ReplyToCheep(Reply.CheepID, Reply.Text, user);
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

    public ActionResult OnPostLike()
    {
        HasNextPage = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();

        string? name = User.Identity?.Name;
        if (name == null)
        {
            ErrorMessage = "You must be logged in to like a cheep.";
            Cheeps = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage);
            LoadLikes();
            return Page();
        }

        ChirpUser? user = _userManager.Users
            .Include(u => u.LikedCheeps)
            .FirstOrDefault(u => u.UserName == name);
        if (user == null)
        {
            ErrorMessage = "User not found!";
            Cheeps = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage);
            LoadLikes();
            return Page();
        }

        if (user.LikedCheeps.Any(c => c.CheepId == CheepIdForLike))
        {
            _cheepService.UnLikeCheep(CheepIdForLike, user.Id);
        }
        else
        {
            _cheepService.LikeCheep(CheepIdForLike, user.Id);
        }

        Cheeps = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage);
        LoadLikes();
        return LocalRedirect($"/user/{Author}?page={CurrentPage}");
    }

    private void LoadTimeline(int page)
    {
        CurrentPage = page < 1 ? 1 : page;
        bool isViewingOwnTimeline = Author == User.Identity?.Name;

        if (isViewingOwnTimeline)
        {
            Cheeps = _cheepService.GetOwnPrivateTimeline(Author, CurrentPage);
            HasNextPage = _cheepService.GetOwnPrivateTimeline(Author, CurrentPage + 1).Any();
        }
        else
        {
            Cheeps = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage);
            HasNextPage = _cheepService.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();
        }
    }

    private void LoadLikes()
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
}
