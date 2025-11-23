using Chirp.Infrastructure.Services;

namespace Chirp.Infrastructure.Tests.Services;

public class FollowCacheTests
{
    [Fact]
    public void SetFollowedUsers_WhenCalled_CachesFollowedUsers()
    {
        // Arrange
        var follower = Guid.NewGuid().ToString();
        var followedUsers = new[] { "alice", "bob" };

        // Act
        FollowCache.Instance.SetFollowedUsers(follower, followedUsers);

        // Assert
        Assert.True(FollowCache.Instance.IsFollowing(follower, "alice"));
        Assert.True(FollowCache.Instance.IsFollowing(follower, "bob"));
    }

    [Fact]
    public void IsFollowing_WhenUserNotCached_ReturnsFalse()
    {
        // Arrange
        var follower = Guid.NewGuid().ToString();

        // Act
        var result = FollowCache.Instance.IsFollowing(follower, "charlie");

        // Assert
        Assert.False(result);
    }
}
