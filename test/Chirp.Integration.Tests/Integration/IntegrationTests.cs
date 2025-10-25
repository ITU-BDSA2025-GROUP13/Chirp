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
        var author = new Author { AuthorId = 1, Name = _name1, Email = "test@test.com" };
        var authors = new List<Author>
        {
            author
        };

        cheeps = new List<Cheep>
        {
            new Cheep{ CheepId = 3, AuthorId = 1, Author = author, Text = _text3, TimeStamp = DateTime.Now },
            new Cheep{ CheepId = 2, AuthorId = 1, Author = author, Text = _text2, TimeStamp = DateTime.Now.AddHours(-1) },
            new Cheep{ CheepId = 1, AuthorId = 1, Author = author, Text = _text1, TimeStamp = DateTime.Now.AddHours(-2) }
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
        result.First().Text.Should().Be(_text3);
        result.Last().Text.Should().Be(_text1);
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
        result.Should().HaveCount(3);
        result.First().Text.Should().Be(_text3);
        result.Last().Text.Should().Be(_text1);
    }

    private void IntegrationTestsUIAndService()
    {
        var author1 = new Author { AuthorId = 1, Name = _name1, Email = "test1@test.com" };
        var author2 = new Author { AuthorId = 2, Name = _name2, Email = "test2@test.com" };
        var karlFortnite = new Author { AuthorId = 3, Name = _name3, Email = "karl@fortnite.com" };
        var authors = new List<Author> { author1, author2, karlFortnite };

        var cheeps = new Stack<Cheep>();
        cheeps.Push(new Cheep { CheepId = 1, AuthorId = 2, Author = author2, Text = _text1, TimeStamp = DateTime.Now.AddHours(-5) });
        cheeps.Push(new Cheep { CheepId = 2, AuthorId = 2, Author = author2, Text = _text2, TimeStamp = DateTime.Now.AddHours(-4) });
        cheeps.Push(new Cheep { CheepId = 3, AuthorId = 2, Author = author2, Text = _text3, TimeStamp = DateTime.Now.AddHours(-3) });
        cheeps.Push(new Cheep { CheepId = 4, AuthorId = 1, Author = author1, Text = _text4, TimeStamp = DateTime.Now.AddHours(-2) });
        cheeps.Push(new Cheep { CheepId = 5, AuthorId = 1, Author = author1, Text = _text5, TimeStamp = DateTime.Now.AddHours(-1) });

        List<Cheep> stackToList() => cheeps.ToList();

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
                new Cheep { CheepId = 100, AuthorId = 3, Author = karlFortnite, Text = _text6, TimeStamp = DateTime.Now }
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
        
        mockAuthorRepo
            .Setup(c => c.GetAuthorByName(_name3))
            .Returns(karlFortniteWithCheeps);

        mockCheepRepo
            .Setup(c => c.InsertCheep(It.IsAny<Cheep>()))
            .Callback<Cheep>(c => cheeps.Push(c))
            .Returns(Task.CompletedTask);

        mockCheepRepo
            .Setup(c => c.GetMainPage(It.IsAny<int>()))
            .ReturnsAsync(() => stackToList());

        mockCheepRepo
            .Setup(c => c.GetAuthorPage(It.IsAny<Author>(), It.IsAny<int>()))
            .ReturnsAsync(() => stackToList());

        mockCheepRepo
            .Setup(c => c.GetAuthorPage(karlFortniteWithCheeps, It.IsAny<int>()))
            .ReturnsAsync(() => karlFortniteWithCheeps.Cheeps);

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
        pageModel.OnGet(_name3, 0);
        pageModel.Cheeps.Should().NotBeNull();
        pageModel.Cheeps.Should().HaveCount(1);
        pageModel.Cheeps[0].AuthorName.Should().Be(_name3);
        pageModel.Cheeps[0].Text.Should().Be(_text6);
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
        newFirstCheep.AuthorName.Should().Be(_name1);
        newFirstCheep.Text.Should().Be(text);
        newFirstCheep.Text.Should().NotBe(oldFirstCheep?.Text);
    }
}
