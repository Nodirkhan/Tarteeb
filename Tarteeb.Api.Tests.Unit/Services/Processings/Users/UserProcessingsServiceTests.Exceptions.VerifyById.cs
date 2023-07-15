//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
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
                service.RetrieveUserByIdAsync(inputUserId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProcessingDependencyValidationException))),Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
