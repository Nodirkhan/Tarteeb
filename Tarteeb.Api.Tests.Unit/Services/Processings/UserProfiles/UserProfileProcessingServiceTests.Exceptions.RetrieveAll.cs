//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(UserProfileDependencyExceptions))]
        public void ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
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

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();

            var failedPostImpressionProcessingServiceException =
                new FailedUserProfileProcessingServiceException(serviceException);

            var expectedUserProfileProcessingServiceException =
                new UserProfileProcessingServiceException(failedPostImpressionProcessingServiceException);

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsers()).Throws(serviceException);

            //when
            Action retrieveAllUserProfilesAction = () =>
                this.userProfileProcessingService.RetrieveAllUserProfiles();

            UserProfileProcessingServiceException actualUserProfilesActionProcessingDependencyException =
                Assert.Throws<UserProfileProcessingServiceException>(retrieveAllUserProfilesAction);

            //then
            actualUserProfilesActionProcessingDependencyException.Should().BeEquivalentTo(
                expectedUserProfileProcessingServiceException);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsers(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProfileProcessingServiceException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}