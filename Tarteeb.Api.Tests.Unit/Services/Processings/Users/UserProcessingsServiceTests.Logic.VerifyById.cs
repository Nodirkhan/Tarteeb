//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using System;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Users;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.Users
{
    public partial class UserProcessingsServiceTests
    {
        [Fact]
        public async Task ShouldVerifyUserByIdAsync()
        {
            // given
            DateTimeOffset dateTimeOffset = GetRandomDate();
            Guid randomUserId = Guid.NewGuid();
            Guid inputUserId = randomUserId;
            User randomUser = CreateRandomUser();
            randomUser.Id = inputUserId;
            User returnedUser = randomUser;
            returnedUser.IsActive = true;
            returnedUser.UpdatedDate = dateTimeOffset;
            User inputUser = returnedUser;
            User modifiedUser = inputUser.DeepClone();
            Guid expectedUserId = modifiedUser.Id;

            this.userServiceMock.Setup(service => 
                service.RetrieveUserByIdAsync(inputUserId))
                    .ReturnsAsync(returnedUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(dateTimeOffset);

            this.userServiceMock.Setup(service =>
                service.ModifyUserAsync(inputUser))
                    .ReturnsAsync(modifiedUser);

            // when
            Guid actualUserId = 
                await this.userProcessingsService.VerifyUserByIdAsync(inputUserId);

            // then
            actualUserId.Should().Be(inputUserId);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.userServiceMock.Verify(service =>
                service.ModifyUserAsync(inputUser), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
