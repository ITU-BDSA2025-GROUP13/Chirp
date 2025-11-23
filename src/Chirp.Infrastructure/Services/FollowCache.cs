namespace Chirp.Infrastructure.Services
{
    public class FollowCache
    {
        public static FollowCache Instance { get; } = new FollowCache();
        private readonly Dictionary<string, HashSet<string>> _cache = new();

        private FollowCache() { }

        public void SetFollowedUsers(string username, IEnumerable<string> followedUsers)
        {
            _cache[username] = followedUsers.ToHashSet();
        }

        public bool IsFollowing(string follower, string target)
        {
            return _cache.TryGetValue(follower, out var follows)
                && follows.Contains(target);
        }
    }
}