using Chirp.Core.Models;
using Chirp.Infrastructure.DatabaseContext;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;

namespace Chirp.Integration.Tests.Integration;

public class IntegrationTests
{
    private ICheepRepository? _cheepRepository;
    private UserManager<ChirpUser> _userManager;
    private ICheepService? _cheepService;

    readonly string _text1 = "Oldest cheep";
    readonly string _text2 = "Older cheep";
    readonly string _text3 = "Newest cheep";
    readonly string _text4 = "new 4";
    readonly string _text5 = "new 5";
    readonly string _text6 = "I love Fortnite";

    readonly string _name1 = "TestUser1";
    readonly string _name2 = "TestUser2";
    readonly string _name3 = "Karl Fortnite";

    List<Cheep>? cheeps;

    private void IntegrationTestsServiceAndRepo()
    {
        string authorID = "1";
        var author = new ChirpUser { Id = authorID, UserName = _name1, Email = "test@test.com" };
        var authors = new List<ChirpUser>
        {
            author
        };

        cheeps = new List<Cheep>
        {
            new Cheep{ CheepId = 3, AuthorId = authorID, Author = author, Text = _text3, TimeStamp = DateTime.Now },
            new Cheep{ CheepId = 2, AuthorId = authorID, Author = author, Text = _text2, TimeStamp = DateTime.Now.AddHours(-1) },
            new Cheep{ CheepId = 1, AuthorId = authorID, Author = author, Text = _text1, TimeStamp = DateTime.Now.AddHours(-2) }
        };

        author.Cheeps = cheeps;
        var mockContext = new Mock<IChirpDbContext>();
        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        _cheepRepository = new CheepRepository(mockContext.Object);
        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        userStore
            .Setup(x => x.FindByIdAsync(authorID, CancellationToken.None))
            .ReturnsAsync(author);
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userStore
            .Setup(x => x.FindByNameAsync(_name1, CancellationToken.None))
            .ReturnsAsync(author);
        _userManager = userManager;
        _cheepService = new CheepService(_cheepRepository, userManager);
    }

    [Fact]
    public void GetCheeps_FromMockedRepository_ReturnsAllCheepsInDescendingOrder()
    {
        IntegrationTestsServiceAndRepo();
        if (_cheepService is null)
        {
            throw new InvalidOperationException("cheep service is not available.");
        }
        if (cheeps is null)
        {
            throw new InvalidOperationException("cheeps is not available.");
        }

        var result = _cheepService.GetMainPageCheeps(0);
        Assert.Equal(cheeps.Count, result.Count());
        Assert.Equal(_text3, result.First().Text);
        Assert.Equal(_text1, result.Last().Text);
    }

    [Fact]
    public void GetCheepsFromAuthor_FromMockedRepository_ReturnsAuthorsCheepsInDescendingOrder()
    {
        IntegrationTestsServiceAndRepo();
        if (_cheepService is null)
        {
            throw new InvalidOperationException("cheep service is not available.");
        }
        var result = _cheepService.GetCheepsFromAuthorName(_name1);
        Assert.Equal(3, result.Count);
        Assert.Equal(_text3, result.First().Text);
        Assert.Equal(_text1, result.Last().Text);
    }

    private void IntegrationTestsUIAndService()
    {
        var author1 = new ChirpUser { Id = "1", UserName = _name1, Email = "test1@test.com" };
        var author2 = new ChirpUser { Id = "2", UserName = _name2, Email = "test2@test.com" };
        var karlFortnite = new ChirpUser { Id = "3", UserName = _name3, Email = "karl@fortnite.com" };
        var authors = new List<ChirpUser> { author1, author2, karlFortnite };

        var cheeps = new Stack<Cheep>();
        cheeps.Push(new Cheep { CheepId = 1, AuthorId = "2", Author = author2, Text = _text1, TimeStamp = DateTime.Now.AddHours(-5) });
        cheeps.Push(new Cheep { CheepId = 2, AuthorId = "2", Author = author2, Text = _text2, TimeStamp = DateTime.Now.AddHours(-4) });
        cheeps.Push(new Cheep { CheepId = 3, AuthorId = "2", Author = author2, Text = _text3, TimeStamp = DateTime.Now.AddHours(-3) });
        cheeps.Push(new Cheep { CheepId = 4, AuthorId = "1", Author = author1, Text = _text4, TimeStamp = DateTime.Now.AddHours(-2) });
        cheeps.Push(new Cheep { CheepId = 5, AuthorId = "1", Author = author1, Text = _text5, TimeStamp = DateTime.Now.AddHours(-1) });

        List<Cheep> stackToList() => cheeps.ToList();

        var author2_author = new ChirpUser
        {
            Id = author2.Id,
            UserName = author2.UserName,
            Email = author2.Email,
            Cheeps = stackToList()
        };

        var karlFortniteWithCheeps = new ChirpUser
        {
            Id = karlFortnite.Id,
            UserName = karlFortnite.UserName,
            Email = karlFortnite.Email,
            Cheeps = new List<Cheep>
            {
                new Cheep { CheepId = 100, AuthorId = "3", Author = karlFortnite, Text = _text6, TimeStamp = DateTime.Now }
            }
        };

        var mockCheepRepo = new Mock<ICheepRepository>();
        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();

        userStore
            .Setup(c => c.FindByIdAsync("1", CancellationToken.None))
            .ReturnsAsync(author1);

        userStore
            .Setup(c => c.FindByIdAsync("2", CancellationToken.None))
            .ReturnsAsync(author2_author);

        userStore
            .Setup(c => c.FindByIdAsync("3", CancellationToken.None))
            .ReturnsAsync(karlFortniteWithCheeps);

        userStore
            .Setup(c => c.FindByNameAsync(_name3, CancellationToken.None))
            .ReturnsAsync(karlFortniteWithCheeps);

        mockCheepRepo
            .Setup(c => c.InsertCheep(It.IsAny<Cheep>()))
            .Callback<Cheep>(c => cheeps.Push(c))
            .Returns(Task.CompletedTask);

        mockCheepRepo
            .Setup(c => c.GetMainPage(It.IsAny<int>()))
            .ReturnsAsync(() => stackToList());

        mockCheepRepo
            .Setup(c => c.GetAuthorPage(It.IsAny<ChirpUser>(), It.IsAny<int>()))
            .ReturnsAsync(() => stackToList());

        mockCheepRepo
            .Setup(c => c.GetAuthorPage(karlFortniteWithCheeps, It.IsAny<int>()))
            .ReturnsAsync(() => karlFortniteWithCheeps.Cheeps);

        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _cheepService = new CheepService(mockCheepRepo.Object, userManager);
    }

    [Fact]
    public void UserTimelineModelOnGet_WithMockedService_PopulatesCheepsForSpecificUser()
    {
        IntegrationTestsUIAndService();
        if (_cheepService is null)
        {
            throw new InvalidOperationException("cheep service is not available.");
        }
        var pageModel = new UserTimelineModel(_cheepService);
        pageModel.Author = _name3;
        pageModel.CurrentPage = 0;
        pageModel.OnGet();
        Assert.NotNull(pageModel.Cheeps);
        Assert.Single(pageModel.Cheeps);
        Assert.Equal(_name3, pageModel.Cheeps[0].AuthorName);
        Assert.Equal(_text6, pageModel.Cheeps[0].Text);
    }

    [Fact]
    public void PublicModelOnGet_AfterPostingCheep_ShowsNewCheepFirst()
    {
        string text = "New test cheep";
        IntegrationTestsUIAndService();
        if (_cheepService is null)
        {
            throw new InvalidOperationException("cheep service is not available.");
        }

        var pageModel = new PublicModel(_cheepService, _userManager);

        pageModel.CurrentPage = 0;
        pageModel.OnGet();
        var oldLength = pageModel.Cheeps.Count;
        var oldFirstCheep = pageModel.Cheeps.FirstOrDefault();

        _cheepService.PostCheep(text, "1");

        pageModel.CurrentPage = 0;
        pageModel.OnGet();
        var newLength = pageModel.Cheeps.Count;
        var newFirstCheep = pageModel.Cheeps.First();

        Assert.NotNull(pageModel.Cheeps);
        Assert.Equal(oldLength + 1, newLength);
        Assert.Equal(_name1, newFirstCheep.AuthorName);
        Assert.Equal(text, newFirstCheep.Text);
        Assert.NotEqual(oldFirstCheep?.Text, newFirstCheep.Text);
    }

    [Fact]
    public void LoginModelOnGet_AfterPostingCheep_ShowsNewCheepFirst()
    {
        string text = "New test cheep";
        IntegrationTestsUIAndService();
        if (_cheepService is null)
        {
            throw new InvalidOperationException("cheep service is not available.");
        }

        var pageModel = new PublicModel(_cheepService, _userManager);

        pageModel.CurrentPage = 0;
        pageModel.OnGet();
        var oldLength = pageModel.Cheeps.Count;
        var oldFirstCheep = pageModel.Cheeps.FirstOrDefault();

        _cheepService.PostCheep(text, "1");

        pageModel.CurrentPage = 0;
        pageModel.OnGet();
        var newLength = pageModel.Cheeps.Count;
        var newFirstCheep = pageModel.Cheeps.First();

        Assert.NotNull(pageModel.Cheeps);
        Assert.Equal(oldLength + 1, newLength);
        Assert.Equal(_name1, newFirstCheep.AuthorName);
        Assert.Equal(text, newFirstCheep.Text);
        Assert.NotEqual(oldFirstCheep?.Text, newFirstCheep.Text);
    }

    [Fact]
    public void GetCheepsWithReplies_FromMockedRepository_ReturnsRepliesInDescendingOrder()
    {
        string authorID = "1";
        var author = new ChirpUser { Id = authorID, UserName = _name1, Email = "test@test.com" };

        string newestReply = "Newest reply";
        string middleReply = "Middle reply";
        string oldestReply = "Oldest reply";

        var parentCheep = new Cheep
        {
            CheepId = 1,
            AuthorId = authorID,
            Author = author,
            Text = "Parent cheep",
            TimeStamp = DateTime.Now.AddHours(-3),
            ParentCheep = null,
            Replies = new List<Cheep>()
        };

        var reply1 = new Cheep
        {
            CheepId = 2,
            AuthorId = authorID,
            Author = author,
            Text = oldestReply,
            TimeStamp = DateTime.Now.AddHours(-2),
            ParentCheep = parentCheep,
            Replies = new List<Cheep>()
        };

        var reply2 = new Cheep
        {
            CheepId = 3,
            AuthorId = authorID,
            Author = author,
            Text = middleReply,
            TimeStamp = DateTime.Now.AddHours(-1),
            ParentCheep = parentCheep,
            Replies = new List<Cheep>()
        };

        var reply3 = new Cheep
        {
            CheepId = 4,
            AuthorId = authorID,
            Author = author,
            Text = newestReply,
            TimeStamp = DateTime.Now,
            ParentCheep = parentCheep,
            Replies = new List<Cheep>()
        };

        parentCheep.Replies = new List<Cheep> { reply1, reply2, reply3 };
        var cheeps = new List<Cheep> { parentCheep, reply1, reply2, reply3 };
        author.Cheeps = cheeps;

        var mockContext = new Mock<IChirpDbContext>();
        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext
            .Setup(c => c.Cheeps)
            .Returns(mockCheepSet.Object);
        _cheepRepository = new CheepRepository(mockContext.Object);

        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        userStore
            .Setup(x => x.FindByIdAsync(authorID, CancellationToken.None))
            .ReturnsAsync(author);
        userStore
            .Setup(x => x.FindByNameAsync(_name1, CancellationToken.None))
            .ReturnsAsync(author);
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _userManager = userManager;
        _cheepService = new CheepService(_cheepRepository, userManager);

        var result = _cheepService.GetMainPageCheeps(0);
        var parentCheepDTO = result.FirstOrDefault(c => c.CheepId == 1);
        Assert.NotNull(parentCheepDTO); Assert.NotNull(parentCheepDTO.Replies);

        Assert.Equal(3, parentCheepDTO.Replies.Count);
        Assert.Equal(oldestReply, parentCheepDTO.Replies[0].Text);
        Assert.Equal(middleReply, parentCheepDTO.Replies[1].Text);
        Assert.Equal(newestReply, parentCheepDTO.Replies[2].Text);
    }
}
