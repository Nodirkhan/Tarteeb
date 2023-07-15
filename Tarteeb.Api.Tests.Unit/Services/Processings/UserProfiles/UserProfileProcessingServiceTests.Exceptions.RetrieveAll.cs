//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(UserDependencyExceptions))]
        public void ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            IQueryable<User> someUsers = CreateRandomUsers();
            IQueryable<UserProfile> someUserProfiles =
                someUsers.Select(AsUserProfile).AsQueryable();

            var expectedUserProfileProcessingDependencyException =
                new UserProfileProcessingDependencyException(
                    dependencyException.InnerException as Xeption);

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsers()).Throws(dependencyException);

            // when
            Action retrieveAllUserProfiles = () =>
                this.userProfileProcessingService.RetrieveAllUserProfiles();

            UserProfileProcessingDependencyException actualUserProfileProcessingDependencyException =
                Assert.Throws<UserProfileProcessingDependencyException>(retrieveAllUserProfiles);

            // then 
            actualUserProfileProcessingDependencyException.Should().BeEquivalentTo(
                expectedUserProfileProcessingDependencyException);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsers(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProfileProcessingDependencyException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}