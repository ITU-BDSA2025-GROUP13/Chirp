using Chirp.Core.Models;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
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
        _service = service;
        _chirpUserService = chirpUserService;
        _userManager = userManager;
        Cheeps = new List<CheepDTO>();
        CurrentPage = 1;
    }

    public ActionResult OnGet()
    {
        if (User.Identity?.Name != null)
        {
            var followedUsers = _service.GetListOfNamesOfFollowedUsers(User.Identity.Name);
            CheepDataCache.Instance.SetFollowedUsers(User.Identity.Name, followedUsers);
        }

        if (Author == User.Identity?.Name)
        {
            Cheeps = _service.GetOwnPrivateTimeline(Author, CurrentPage);
            HasNextPage = _service.GetOwnPrivateTimeline(Author, CurrentPage + 1).Any();
            return Page();
        }

        Cheeps = _service.GetCheepsFromAuthorName(Author, CurrentPage);
        HasNextPage = _service.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();
        LoadLikedCheeps();
        return Page();
    }

    public ActionResult OnPostDelete()
    {
        HasNextPage = _service.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();

        _service.DeleteCheep(CheepIdForDeletion);
        return LocalRedirect($"/user/{Author}?page={CurrentPage}");
    }

    public ActionResult OnPostFollow()
    {
        if (User.Identity?.Name != null && ToggleFollowForUserId != null)
        {
            _chirpUserService.ToggleUserFollowing(User.Identity.Name, ToggleFollowForUserId);
        }

        Cheeps = _service.GetCheepsFromAuthorName(Author, CurrentPage);
        LoadLikedCheeps();
        return LocalRedirect($"/user/{Author}?page={CurrentPage}");
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
            LoadLikedCheeps();
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

    public ActionResult OnPostReply()
    {
        string? name = User.Identity?.Name;
        if (name == null)
        {
            ErrorMessage = "You must be logged in to post a cheep.";
            CurrentPage = 1;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }

        ChirpUser? user = _userManager.FindByNameAsync(name).GetAwaiter().GetResult();
        if (user == null)
        {
            ErrorMessage = "User not found! Try logging out and in again.";
            CurrentPage = 1;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }

        if (Reply == null || Reply.Text == null)
        {
            ErrorMessage = "Reply not found!";
            CurrentPage = 1;
            Cheeps = _service.GetMainPageCheeps();
            HasNextPage = _service.GetMainPageCheeps(1).Any();
            return Page();
        }

        LoadLikedCheeps();
        _service.ReplyToCheep(Reply.CheepID, Reply.Text, user);
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
        HasNextPage = _service.GetCheepsFromAuthorName(Author, CurrentPage + 1).Any();

        string? name = User.Identity?.Name;
        if (name == null)
        {
            ErrorMessage = "You must be logged in to like a cheep.";
            Cheeps = _service.GetCheepsFromAuthorName(Author, CurrentPage);
            LoadLikedCheeps();
            return Page();
        }

        ChirpUser? user = _userManager.Users
            .Include(u => u.LikedCheeps)
            .FirstOrDefault(u => u.UserName == name);
        if (user == null)
        {
            ErrorMessage = "User not found!";
            Cheeps = _service.GetCheepsFromAuthorName(Author, CurrentPage);
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

        Cheeps = _service.GetCheepsFromAuthorName(Author, CurrentPage);
        LoadLikedCheeps();
        return Page();
    }
}
