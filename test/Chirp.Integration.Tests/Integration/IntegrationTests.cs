using Chirp.Core.Models;
using Chirp.Infrastructure.DatabaseContext;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Chirp.Web.Pages;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;

namespace Chirp.Integration.Tests.Integration;

public class IntegrationTests
{
    private ICheepRepository? _cheepRepository;
    private IAuthorRepository? _authorRepository;
    private ICheepService? _cheepService;

    string text6 = "I love Fortnite";
    string text5 = "new 5";
    string text4 = "new 4";
    string text3 = "Newest cheep";
    string text2 = "Older cheep";
    string text1 = "Oldest cheep";

    string name1 = "TestUser1";
    string name2 = "TestUser2";
    string name3 = "Karl Fortnite";
    List<Cheep>? cheeps;

    private void IntegrationTestsServiceAndRepo()
    {
        var author = new Author { AuthorId = 1, Name = name1, Email = "test@test.com" };
        var authors = new List<Author>
        {
            author
        };
        cheeps = new List<Cheep>
        {
            new Cheep{ CheepId = 3, AuthorId = 1, Author = author, Text = text3, TimeStamp = DateTime.Now },
            new Cheep{ CheepId = 2, AuthorId = 1, Author = author, Text = text2, TimeStamp = DateTime.Now.AddHours(-1) },
            new Cheep{ CheepId = 1, AuthorId = 1, Author = author, Text = text1, TimeStamp = DateTime.Now.AddHours(-2) }
        };
        author.Cheeps = cheeps;
        var mockContext = new Mock<IChirpDbContext>();
        var mockCheepSet = cheeps.BuildMockDbSet();
        var mockAuthorSet = authors.BuildMockDbSet();
        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        _cheepRepository = new CheepRepository(mockContext.Object);
        _authorRepository = new AuthorRepository(mockContext.Object);
        _cheepService = new CheepService(_cheepRepository, _authorRepository);
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
        result.Should().HaveCount(cheeps.Count);
        result.First().Text.Should().Be(text3);
        result.Last().Text.Should().Be(text1);
    }

    [Fact]
    public void GetCheepsFromAuthor_FromMockedRepository_ReturnsAuthorsCheepsInDescendingOrder()
    {
        IntegrationTestsServiceAndRepo();
        if (_cheepService is null)
        {
            throw new InvalidOperationException("cheep service is not available.");
        }
        var result = _cheepService.GetCheepsFromAuthorName(name1);
        result.Should().HaveCount(3);
        result.First().Text.Should().Be(text3);
        result.Last().Text.Should().Be(text1);
    }

    private void IntegrationTestsUIAndService()
    {
        var author1 = new Author { AuthorId = 1, Name = name1, Email = "test1@test.com" };
        var author2 = new Author { AuthorId = 2, Name = name2, Email = "test2@test.com" };
        var karlFortnite = new Author { AuthorId = 3, Name = name3, Email = "karl@fortnite.com" };
        var authors = new List<Author> { author1, author2, karlFortnite };

        var cheeps = new Stack<Cheep>();
        cheeps.Push(new Cheep { CheepId = 1, AuthorId = 2, Author = author2, Text = text1, TimeStamp = DateTime.Now.AddHours(-5) });
        cheeps.Push(new Cheep { CheepId = 2, AuthorId = 2, Author = author2, Text = text2, TimeStamp = DateTime.Now.AddHours(-4) });
        cheeps.Push(new Cheep { CheepId = 3, AuthorId = 2, Author = author2, Text = text3, TimeStamp = DateTime.Now.AddHours(-3) });
        cheeps.Push(new Cheep { CheepId = 4, AuthorId = 1, Author = author1, Text = text4, TimeStamp = DateTime.Now.AddHours(-2) });
        cheeps.Push(new Cheep { CheepId = 5, AuthorId = 1, Author = author1, Text = text5, TimeStamp = DateTime.Now.AddHours(-1) });

        var stackToList = () => cheeps.ToList();

        var author2_author = new Author
        {
            AuthorId = author2.AuthorId,
            Name = author2.Name,
            Email = author2.Email,
            Cheeps = stackToList()
        };

        var karlFortniteWithCheeps = new Author
        {
            AuthorId = karlFortnite.AuthorId,
            Name = karlFortnite.Name,
            Email = karlFortnite.Email,
            Cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 100, AuthorId = 3, Author = karlFortnite, Text = text6, TimeStamp = DateTime.Now }
        }
        };

        var mockCheepRepo = new Mock<ICheepRepository>();
        var mockAuthorRepo = new Mock<IAuthorRepository>();

        mockAuthorRepo
            .Setup(c => c.GetAuthorByID(1))
            .Returns(author1);
        mockAuthorRepo
            .Setup(c => c.GetAuthorByID(2))
            .Returns(author2_author);
        mockAuthorRepo
            .Setup(c => c.GetAuthorByID(3))
            .Returns(karlFortniteWithCheeps);

        mockCheepRepo
            .Setup(c => c.InsertCheep(It.IsAny<Cheep>()))
            .Callback<Cheep>(c => cheeps.Push(c))
            .Returns(Task.CompletedTask);

        mockCheepRepo
            .Setup(c => c.GetMainPage(It.IsAny<int>()))
            .ReturnsAsync(() => stackToList());
        
        mockAuthorRepo
            .Setup(c => c.GetAuthorByName(name3))
            .Returns(karlFortniteWithCheeps);

        _cheepService = new CheepService(mockCheepRepo.Object, mockAuthorRepo.Object);
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
        pageModel.OnGet(name3, 0);
        pageModel.Cheeps.Should().NotBeNull();
        pageModel.Cheeps.Should().HaveCount(1);
        pageModel.Cheeps[0].AuthorName.Should().Be(name3);
        pageModel.Cheeps[0].Text.Should().Be(text6);
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

        var pageModel = new PublicModel(_cheepService);

        pageModel.OnGet(0);
        var oldLength = pageModel.Cheeps.Count;
        var oldFirstCheep = pageModel.Cheeps.FirstOrDefault();

        _cheepService.PostCheep(text, 1);

        pageModel.OnGet(0);
        var newLength = pageModel.Cheeps.Count;
        var newFirstCheep = pageModel.Cheeps.First();

        pageModel.Cheeps.Should().NotBeNull();
        newLength.Should().Be(oldLength + 1);
        newFirstCheep.AuthorName.Should().Be(name1);
        newFirstCheep.Text.Should().Be(text);
        newFirstCheep.Text.Should().NotBe(oldFirstCheep?.Text);
    }
}
