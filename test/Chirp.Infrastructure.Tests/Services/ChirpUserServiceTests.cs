using System.Collections.Generic;
using System.Linq;
using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Chirp.Infrastructure.Tests.Services;

public class ChirpUserServiceTests
{
	[Fact]
	public void GetFollowedUsernames_UserNotFound_ReturnsEmptyList()
	{
		Mock<IChirpUserRepository> repoMock = new Mock<IChirpUserRepository>();
		Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();
		userManagerMock
			.Setup(m => m.FindByNameAsync("missing"))
			.ReturnsAsync((ChirpUser?)null);

		var service = new ChirpUserService(repoMock.Object, userManagerMock.Object);

		List<string> result = service.GetFollowedUsernames("missing");

		Assert.Empty(result);
		repoMock.Verify(r => r.GetFollowedUsers(It.IsAny<ChirpUser>()), Times.Never);
	}

	[Fact]
	public void GetFollowedUsernames_FiltersSelfAndNullEntries()
	{
		Mock<IChirpUserRepository> repoMock = new Mock<IChirpUserRepository>();
		Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

		var currentUser = new ChirpUser { UserName = "current", Email = "current@test.com", Id = "1" };
		userManagerMock
			.Setup(m => m.FindByNameAsync(currentUser.UserName!))
			.ReturnsAsync(currentUser);

		List<ChirpUser> followedUsers =
		[
			new ChirpUser { UserName = "friend", Email = "friend@test.com" },
			new ChirpUser { UserName = null },
			new ChirpUser { UserName = string.Empty },
			new ChirpUser { UserName = currentUser.UserName }
		];

		repoMock
			.Setup(r => r.GetFollowedUsers(currentUser))
			.Returns(followedUsers);

		var service = new ChirpUserService(repoMock.Object, userManagerMock.Object);

		List<string> result = service.GetFollowedUsernames(currentUser.UserName!);

		string friendName = Assert.Single(result);
		Assert.Equal("friend", friendName);
	}

	[Fact]
	public void ToggleUserFollowing_WhenRelationExists_RemovesFollower()
	{
		const string followerName = "follower";
		const string followedName = "followed";

		Mock<IChirpUserRepository> repoMock = new Mock<IChirpUserRepository>();
		Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

		var follower = new ChirpUser { UserName = followerName, Email = "follower@test.com", Id = "1" };
		var followed = new ChirpUser { UserName = followedName, Email = "followed@test.com", Id = "2" };

		userManagerMock.Setup(m => m.FindByNameAsync(followerName)).ReturnsAsync(follower);
		userManagerMock.Setup(m => m.FindByNameAsync(followedName)).ReturnsAsync(followed);

		repoMock.Setup(r => r.ContainsRelation(follower, followed)).Returns(true);
		repoMock.Setup(r => r.RemoveFromFollowerList(follower, followed)).Returns(Task.CompletedTask);

		var service = new ChirpUserService(repoMock.Object, userManagerMock.Object);

		service.ToggleUserFollowing(followerName, followedName);

		repoMock.Verify(r => r.ContainsRelation(follower, followed), Times.Once);
		repoMock.Verify(r => r.RemoveFromFollowerList(follower, followed), Times.Once);
		repoMock.Verify(r => r.AddToFollowerList(It.IsAny<ChirpUser>(), It.IsAny<ChirpUser>()), Times.Never);
	}

	[Fact]
	public void ToggleUserFollowing_WhenRelationMissing_AddsFollower()
	{
		const string followerName = "follower";
		const string followedName = "followed";

		Mock<IChirpUserRepository> repoMock = new Mock<IChirpUserRepository>();
		Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

		var follower = new ChirpUser { UserName = followerName, Email = "follower@test.com", Id = "1" };
		var followed = new ChirpUser { UserName = followedName, Email = "followed@test.com", Id = "2" };

		userManagerMock.Setup(m => m.FindByNameAsync(followerName)).ReturnsAsync(follower);
		userManagerMock.Setup(m => m.FindByNameAsync(followedName)).ReturnsAsync(followed);

		repoMock.Setup(r => r.ContainsRelation(follower, followed)).Returns(false);
		repoMock.Setup(r => r.AddToFollowerList(follower, followed)).Returns(Task.CompletedTask);

		var service = new ChirpUserService(repoMock.Object, userManagerMock.Object);

		service.ToggleUserFollowing(followerName, followedName);

		repoMock.Verify(r => r.ContainsRelation(follower, followed), Times.Once);
		repoMock.Verify(r => r.AddToFollowerList(follower, followed), Times.Once);
		repoMock.Verify(r => r.RemoveFromFollowerList(It.IsAny<ChirpUser>(), It.IsAny<ChirpUser>()), Times.Never);
	}

	[Fact]
	public void ToggleUserFollowing_WhenUserMissing_DoesNothing()
	{
		const string followerName = "follower";
		const string followedName = "followed";

		Mock<IChirpUserRepository> repoMock = new Mock<IChirpUserRepository>();
		Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

		var follower = new ChirpUser { UserName = followerName, Email = "follower@test.com", Id = "1" };

		userManagerMock.Setup(m => m.FindByNameAsync(followerName)).ReturnsAsync(follower);
		userManagerMock.Setup(m => m.FindByNameAsync(followedName)).ReturnsAsync((ChirpUser?)null);

		var service = new ChirpUserService(repoMock.Object, userManagerMock.Object);

		service.ToggleUserFollowing(followerName, followedName);

		repoMock.Verify(r => r.ContainsRelation(It.IsAny<ChirpUser>(), It.IsAny<ChirpUser>()), Times.Never);
		repoMock.Verify(r => r.AddToFollowerList(It.IsAny<ChirpUser>(), It.IsAny<ChirpUser>()), Times.Never);
		repoMock.Verify(r => r.RemoveFromFollowerList(It.IsAny<ChirpUser>(), It.IsAny<ChirpUser>()), Times.Never);
	}

	private static Mock<UserManager<ChirpUser>> CreateUserManagerMock()
	{
		var storeMock = new Mock<IUserStore<ChirpUser>>();
		return new Mock<UserManager<ChirpUser>>(
			storeMock.Object,
			null!,
			null!,
			Enumerable.Empty<IUserValidator<ChirpUser>>(),
			Enumerable.Empty<IPasswordValidator<ChirpUser>>(),
			null!,
			null!,
			null!,
			null!);
	}
}
