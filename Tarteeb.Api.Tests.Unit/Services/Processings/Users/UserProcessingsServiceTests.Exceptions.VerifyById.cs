//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Processings.Users;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.Users
{
    public partial class UserProcessingsServiceTests
    {
        [Theory]
        [MemberData(nameof(UserDependencyValidationExceptions))]
        public async Task ShouldThrowUserProcessingDependencyValidationExceptionIfUserValidationExceptionOccursAndLogItAsync(
            Xeption dependencyValidationExceptions)
        {
            // given
            Guid randomUserId = Guid.NewGuid();
            Guid inputUserId = randomUserId;

            var expectedUserProcessingDependencyValidationException =
                new UserProcessingDependencyValidationException(dependencyValidationExceptions.InnerException as Xeption);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserId))
                    .ThrowsAsync(dependencyValidationExceptions);

            // when
            ValueTask<Guid> verifyUserByIdTask = this.userProcessingsService.VerifyUserByIdAsync(inputUserId);

            UserProcessingDependencyValidationException actualUserProcessingDependencyValidationException =
                await Assert.ThrowsAsync<UserProcessingDependencyValidationException>(verifyUserByIdTask.AsTask);

            // then
            actualUserProcessingDependencyValidationException.Should().BeEquivalentTo(expectedUserProcessingDependencyValidationException);
            
            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProcessingDependencyValidationException))),Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(UserDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionIfDependencyErrorOccurrsAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid randomUserId = Guid.NewGuid();
            Guid inputUserId = randomUserId;

            var expectedUserProcessingDependencyException =
                new UserProcessingDependencyException(dependencyException.InnerException as Xeption);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserId))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<Guid> verifyUserByIdTask = this.userProcessingsService.VerifyUserByIdAsync(inputUserId);

            UserProcessingDependencyException actualUserProcessingDependencyException =
                await Assert.ThrowsAsync<UserProcessingDependencyException>(verifyUserByIdTask.AsTask);

            // then
            actualUserProcessingDependencyException.Should().BeEquivalentTo(expectedUserProcessingDependencyException);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProcessingDependencyException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnVerifyUserByIdIfExceptionErrorOccurrsAndLogItAsync()
        {
            // given
            Guid randomUserId = Guid.NewGuid();
            Guid inputUserId = randomUserId;
            var serviceException = new Exception();

            var failedUserProcessingServiceException =
                new FailedUserProcessingServiceException(serviceException);

            var expectedUserProcessingServiceException =
                new UserProcessingServiceException(failedUserProcessingServiceException);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserId))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Guid> verifyUserByIdTask = this.userProcessingsService.VerifyUserByIdAsync(inputUserId);

            UserProcessingServiceException actualUserProcessingServiceException =
                await Assert.ThrowsAsync<UserProcessingServiceException>(
                    verifyUserByIdTask.AsTask);

            // then
            actualUserProcessingServiceException.Should().BeEquivalentTo(expectedUserProcessingServiceException);

            this.userServiceMock.Verify(service =>
               service.RetrieveUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProcessingServiceException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
