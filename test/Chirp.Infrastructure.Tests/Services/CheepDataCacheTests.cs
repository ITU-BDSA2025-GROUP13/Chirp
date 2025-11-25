using Chirp.Infrastructure.Services;

namespace Chirp.Infrastructure.Tests.Services;

public class CheepDataCacheTests
{
    [Fact]
    public void SetFollowedUsers_WhenCalled_CachesFollowedUsers()
    {
        // Arrange
        var follower = Guid.NewGuid().ToString();
        var followedUsers = new[] { "alice", "bob" };

        // Act
        CheepDataCache.Instance.SetFollowedUsers(follower, followedUsers);

        // Assert
        Assert.True(CheepDataCache.Instance.UserIsFollowing(follower, "alice"));
        Assert.True(CheepDataCache.Instance.UserIsFollowing(follower, "bob"));
    }

    [Fact]
    public void IsFollowing_WhenUserNotCached_ReturnsFalse()
    {
        // Arrange
        var follower = Guid.NewGuid().ToString();

        // Act
        var result = CheepDataCache.Instance.UserIsFollowing(follower, "charlie");

        // Assert
        Assert.False(result);
    }
}
