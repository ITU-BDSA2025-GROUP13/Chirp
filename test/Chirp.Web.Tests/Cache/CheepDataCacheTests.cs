using Chirp.Web.Cache;

namespace Chirp.Web.Tests.Cache
{
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

        [Fact]
        public void SetLikedCheeps_WhenCalled_CachesLikedCheeps()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var likedCheepIds = new[] { 1, 2, 3 };

            // Act
            CheepDataCache.Instance.SetLikedCheeps(username, likedCheepIds);

            // Assert
            Assert.True(CheepDataCache.Instance.UserHasLiked(username, 1));
            Assert.True(CheepDataCache.Instance.UserHasLiked(username, 2));
            Assert.True(CheepDataCache.Instance.UserHasLiked(username, 3));
            Assert.False(CheepDataCache.Instance.UserHasLiked(username, 4));
        }

        [Fact]
        public void UserHasLiked_WhenUsernameIsNull_ReturnsFalse()
        {
            // Act
            var result = CheepDataCache.Instance.UserHasLiked(null!, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UserHasLiked_WhenUsernameIsEmpty_ReturnsFalse()
        {
            // Act
            var result = CheepDataCache.Instance.UserHasLiked(string.Empty, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UserHasLiked_WhenUserNotCached_ReturnsFalse()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();

            // Act
            var result = CheepDataCache.Instance.UserHasLiked(username, 1);

            // Assert
            Assert.False(result);
        }
    }
}