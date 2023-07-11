//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Linq;
using FluentAssertions;
using Force.DeepCloner;
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
            IQueryable<User> randomUsers = CreateRandomUsers();
            IQueryable<UserProfile> randomUserProfiles = PopulateUserProfiles(randomUsers);

            IQueryable<User> returnedUsers = randomUsers;
            IQueryable<UserProfile> expectedUserProfiles = randomUserProfiles.DeepClone();

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsers());

            // when
            IQueryable<UserProfile> actualUserProfiles =
                this.userProfileProcessingService.RetrieveAllUserProfiles();

            // then 
            actualUserProfiles.Should().BeEquivalentTo(expectedUserProfiles);

            this.userServiceMock.Verify(service => 
                service.RetrieveAllUsers(), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private IQueryable<UserProfile> PopulateUserProfiles(IQueryable<User> users)
        {
            return users.Select(user => new UserProfile
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                BirthDate = user.BirthDate,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                GitHubUsername = user.GitHubUsername,
                TelegramUsername = user.TelegramUsername,
                TeamId = user.TeamId
            });
        }
    }
}
