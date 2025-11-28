using Chirp.Core.Models;
using Chirp.Infrastructure.DatabaseContext;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Tests.Repositories;

public class ChirpUserRepositoryTests
{
    [Fact]
    public async Task AddToFollowerList_WhenUsersExist_PersistsRelation()
    {
        await using var context = CreateInMemoryContext();
        var repository = new ChirpUserRepository(context);

        var userA = CreateUser("user-a");
        var userB = CreateUser("user-b");

        await context.ChirpUsers.AddRangeAsync(userA, userB);
        await context.SaveChangesAsync();

        await repository.AddToFollowerList(userA, userB);

        context.ChangeTracker.Clear();

        var persistedFollower = await context.ChirpUsers
            .Include(u => u.FollowsList)
            .SingleAsync(u => u.Id == userA.Id);

        Assert.Single(persistedFollower.FollowsList);
        Assert.Equal(userB.Id, persistedFollower.FollowsList.Single().Id);
    }

    [Fact]
    public async Task RemoveFromFollowerList_WhenRelationExists_RemovesRelation()
    {
        await using var context = CreateInMemoryContext();
        var repository = new ChirpUserRepository(context);

        var userA = CreateUser("user-a");
        var userB = CreateUser("user-b");

        await context.ChirpUsers.AddRangeAsync(userA, userB);
        await context.SaveChangesAsync();

        await repository.AddToFollowerList(userA, userB);
        context.ChangeTracker.Clear();

        var trackedFollower = await context.ChirpUsers
            .Include(u => u.FollowsList)
            .SingleAsync(u => u.Id == userA.Id);
        var trackedFollowee = await context.ChirpUsers.SingleAsync(u => u.Id == userB.Id);

        await repository.RemoveFromFollowerList(trackedFollower, trackedFollowee);

        context.ChangeTracker.Clear();

        var followerAfterRemoval = await context.ChirpUsers
            .Include(u => u.FollowsList)
            .SingleAsync(u => u.Id == userA.Id);

        Assert.Empty(followerAfterRemoval.FollowsList);
    }

    [Fact]
    public async Task ContainsRelation_WhenCalled_ReturnsExpectedResult()
    {
        await using var context = CreateInMemoryContext();
        var repository = new ChirpUserRepository(context);

        var userA = CreateUser("user-a");
        var userB = CreateUser("user-b");
        var userC = CreateUser("user-c");

        await context.ChirpUsers.AddRangeAsync(userA, userB, userC);
        await context.SaveChangesAsync();

        await repository.AddToFollowerList(userA, userB);
        context.ChangeTracker.Clear();

        var follower = await context.ChirpUsers.SingleAsync(u => u.Id == userA.Id);
        var followed = await context.ChirpUsers.SingleAsync(u => u.Id == userB.Id);
        var notFollowed = await context.ChirpUsers.SingleAsync(u => u.Id == userC.Id);

        Assert.True(repository.ContainsRelation(follower, followed));
        Assert.False(repository.ContainsRelation(follower, notFollowed));
    }

    [Fact]
    public async Task GetFollowedUsers_WhenRelationsExist_ReturnsFollowedUsers()
    {
        await using var context = CreateInMemoryContext();
        var repository = new ChirpUserRepository(context);

        var follower = CreateUser("follower");
        var followedA = CreateUser("followed-a");
        var followedB = CreateUser("followed-b");

        await context.ChirpUsers.AddRangeAsync(follower, followedA, followedB);
        await context.SaveChangesAsync();

        await repository.AddToFollowerList(follower, followedA);
        await repository.AddToFollowerList(follower, followedB);

        context.ChangeTracker.Clear();

        var persistedFollower = await context.ChirpUsers.SingleAsync(u => u.Id == follower.Id);

        var followedUsers = repository.GetFollowedUsers(persistedFollower);

        Assert.Equal(2, followedUsers.Count);
        Assert.Contains(followedUsers, u => u.Id == followedA.Id);
        Assert.Contains(followedUsers, u => u.Id == followedB.Id);
    }

    [Fact]
    public async Task GetListOfFollowers_WhenRelationsExist_ReturnsFollowers()
    {
        await using var context = CreateInMemoryContext();
        var repository = new ChirpUserRepository(context);

        var follower = CreateUser("follower");
        var followedA = CreateUser("followed-a");
        var followedB = CreateUser("followed-b");

        await context.ChirpUsers.AddRangeAsync(follower, followedA, followedB);
        await context.SaveChangesAsync();

        await repository.AddToFollowerList(follower, followedA);
        await repository.AddToFollowerList(follower, followedB);

        context.ChangeTracker.Clear();

        var persistedFollower = await context.ChirpUsers.SingleAsync(u => u.Id == follower.Id);

        var followedUsers = await repository.GetListOfFollowers(persistedFollower);

        Assert.Equal(2, followedUsers.Count);
        Assert.Contains(followedUsers, u => u.Id == followedA.Id);
        Assert.Contains(followedUsers, u => u.Id == followedB.Id);
    }

    [Fact]
    public async Task ForgetUser_WhenCalled_AnonymizesUserData()
    {
        await using var context = CreateInMemoryContext();
        var repository = new ChirpUserRepository(context);

        var user = CreateUser("testuser");
        var follower = CreateUser("follower");
        var followed = CreateUser("followed");

        await context.ChirpUsers.AddRangeAsync(user, follower, followed);
        await context.SaveChangesAsync();

        // Add some relationships
        await repository.AddToFollowerList(user, followed);
        await repository.AddToFollowerList(follower, user);

        context.ChangeTracker.Clear();

        var trackedUser = await context.ChirpUsers.SingleAsync(u => u.Id == user.Id);

        await repository.ForgetUser(trackedUser);

        context.ChangeTracker.Clear();

        var forgottenUser = await context.ChirpUsers
            .Include(u => u.FollowsList)
            .Include(u => u.FollowedByList)
            .SingleAsync(u => u.Id == user.Id);

        Assert.Equal($"[{user.Id}]", forgottenUser.UserName);
        Assert.Null(forgottenUser.Email);
        Assert.Null(forgottenUser.PhoneNumber);
        Assert.Null(forgottenUser.PasswordHash);
        Assert.Empty(forgottenUser.FollowsList);
        Assert.Empty(forgottenUser.FollowedByList);
    }

    private static ChirpDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ChirpDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static ChirpUser CreateUser(string userName)
    {
        return new ChirpUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userName,
            Email = $"{userName}@example.com"
        };
    }
}
