//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Linq;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllUserProfiles()
        {
            // given
            dynamic[] randomUsersProperties = CreateRandomUsersProfileProperties();

            IQueryable<User> mappedUsers = MapToUser(randomUsersProperties);
            IQueryable<User> returnedUsers = mappedUsers;

            IQueryable<UserProfile> mappedUsersProfile = MapToUsersProfile(randomUsersProperties);
            IQueryable<UserProfile> expectedUsersProfiles = mappedUsersProfile;

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsers())
                    .Returns(returnedUsers);

            // when
            IQueryable<UserProfile> actualUserProfiles =
                this.userProfileProcessingService.RetrieveAllUserProfiles();

            // then 
            actualUserProfiles.Should().BeEquivalentTo(expectedUsersProfiles);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsers(), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
