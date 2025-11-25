namespace Chirp.Infrastructure.Services
{
    public class CheepDataCache
    {
        public static CheepDataCache Instance { get; } = new CheepDataCache();
        private readonly Dictionary<string, HashSet<string>> _followCache = new();
        private readonly Dictionary<string, HashSet<int>> _likeCache = new();

        private CheepDataCache() { }

        public void SetFollowedUsers(string username, IEnumerable<string> followedUsers)
        {
            _followCache[username] = followedUsers.ToHashSet();
        }

        public bool UserIsFollowing(string user, string target)
        {
            return _followCache.TryGetValue(user, out var follows)
                && follows.Contains(target);
        }

        public void SetLikedCheeps(string username, IEnumerable<int> likedCheepIds)
        {
            _likeCache[username] = likedCheepIds.ToHashSet();
        }

        public bool UserHasLiked(string username, int cheepId)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            return _likeCache.TryGetValue(username, out var likedCheeps)
                && likedCheeps.Contains(cheepId);
        }
    }
}