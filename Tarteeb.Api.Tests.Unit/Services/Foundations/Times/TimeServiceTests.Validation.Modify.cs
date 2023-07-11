//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tarteeb.Api.Models.Foundations.Tickets;
using Tarteeb.Api.Models.Foundations.Times;
using Tarteeb.Api.Models.Foundations.Times.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.TimeSlots
{
    public partial class TimeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfTimeIsNullAndLogItAsync()
        {
            // given
            Time nullTime = null;
            var nullTimeException = new NullTimeException();

            var expectedTimeValidationException =
                new TimeValidationException(nullTimeException);

            // when
            ValueTask<Time> modifyTimeTask =
                this.timeService.ModifyTimeAsync(nullTime);

            TimeValidationException actualTimeValidationException =
                 await Assert.ThrowsAsync<TimeValidationException>(modifyTimeTask.AsTask);

            // then
            actualTimeValidationException.Should()
                .BeEquivalentTo(expectedTimeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTimeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTimeAsync(It.IsAny<Time>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfTimeIsInvalidAndLogItAsync(string invalidString)
        {
            // given 
            var invalidTime = new Time()
            {
                Comment = invalidString
            };

            var invalidTimeException = new InvalidTimeException();

            invalidTimeException.AddData(
                key: nameof(Time.Id),
                values: "Id is required");

            invalidTimeException.AddData(
               key: nameof(Time.HoursWorked),
               values: "Value is required");

            invalidTimeException.AddData(
               key: nameof(Time.DateWorked),
               values: "Date is required");

            invalidTimeException.AddData(
               key: nameof(Time.Comment),
               values: "Comment is required");

            invalidTimeException.AddData(
               key: nameof(Time.UserId),
               values: "Id is required");

            invalidTimeException.AddData(
               key: nameof(Time.TicketId),
               values: "Id is required");

            invalidTimeException.AddData(
               key: nameof(Time.CreatedDate),
               values: "Date is required");

            invalidTimeException.AddData(
                key: nameof(Ticket.UpdatedDate),
                 "Date is required",
                "Date is not recent",
                $"Date is the same as {nameof(Time.CreatedDate)}");

            var expectedTimeValidationException =
                new TimeValidationException(invalidTimeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(GetRandomDateTime);

            // when
            ValueTask<Time> modifyTimeTask =
                this.timeService.ModifyTimeAsync(invalidTime);

            TimeValidationException actualTimeValidationException =
                await Assert.ThrowsAsync<TimeValidationException>(modifyTimeTask.AsTask);

            // then
            actualTimeValidationException.Should()
                .BeEquivalentTo(expectedTimeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTimeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                 broker.UpdateTimeAsync(It.IsAny<Time>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given 
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Time randomTime = CreateRandomTime(randomDateTime);
            Time invalidTime = randomTime;
            var invalidTimeException = new InvalidTimeException();

            invalidTimeException.AddData(
                key: nameof(Time.UpdatedDate),
                values: $"Date is the same as {nameof(Time.CreatedDate)}");

            var expectedTimeValidationException =
                new TimeValidationException(invalidTimeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Time> modifyTimeTask =
                this.timeService.ModifyTimeAsync(invalidTime);

            TimeValidationException actualTimeValidationException =
                await Assert.ThrowsAsync<TimeValidationException>(
                    modifyTimeTask.AsTask);

            // then
            actualTimeValidationException.Should()
                .BeEquivalentTo(expectedTimeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTimeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTimeByIdAsync(invalidTime.Id), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateNotRecentAddLogItAsync(int seconds)
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Time randomTime = CreateRandomTime(randomDateTime);
            Time inputTime = randomTime;
            inputTime.UpdatedDate = randomDateTime.AddMinutes(seconds);
            var invalidTimeException = new InvalidTimeException();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            invalidTimeException.AddData(
                key: nameof(Time.UpdatedDate),
                values: "Date is not recent");

            var expectedTimeValidationException =
                new TimeValidationException(invalidTimeException);

            // when
            ValueTask<Time> modifyTimeTask =
                this.timeService.ModifyTimeAsync(inputTime);

            TimeValidationException actualTimeValidationException =
               await Assert.ThrowsAsync<TimeValidationException>(
                   modifyTimeTask.AsTask);

            // then
            actualTimeValidationException.Should()
                .BeEquivalentTo(expectedTimeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTimeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTimeByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfTimeDoesNotExistAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Time randomTime = CreateRandomTime(randomDateTime);
            Time nonExistTime = randomTime;
            Time nullTime = null;

            nonExistTime.CreatedDate =
                randomDateTime.AddMinutes(randomMinutes);

            var notFoundTimeException =
                new NotFoundTimeException(nonExistTime.Id);

            var expectedTimeValidationException =
                new TimeValidationException(notFoundTimeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTimeByIdAsync(nonExistTime.Id))
                    .ReturnsAsync(nullTime);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Time> modifyTimeTask =
                this.timeService.ModifyTimeAsync(nonExistTime);

            TimeValidationException actualTimeValidationException =
                await Assert.ThrowsAsync<TimeValidationException>(
                    modifyTimeTask.AsTask);

            // then
            actualTimeValidationException.Should()
                .BeEquivalentTo(expectedTimeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTimeByIdAsync(nonExistTime.Id), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTimeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Time randomTime = CreateRandomModifyTime(randomDateTime);
            Time invalidTime = randomTime.DeepClone();
            Time storageTime = invalidTime.DeepClone();
            storageTime.CreatedDate = storageTime.CreatedDate.AddMinutes(randomMinutes);
            storageTime.UpdatedDate = storageTime.UpdatedDate.AddMinutes(randomMinutes);
            var invalidTimeException = new InvalidTimeException();
            Guid TimeId = invalidTime.Id;

            invalidTimeException.AddData(
                key: nameof(Time.CreatedDate),
                values: $"Date is not the same as {nameof(Time.CreatedDate)}");

            var expectedTimeValidationException =
                new TimeValidationException(invalidTimeException);

            this.storageBrokerMock.Setup(broker =>
               broker.SelectTimeByIdAsync(TimeId)).ReturnsAsync(storageTime);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when 
            ValueTask<Time> modifyTimeTask =
                this.timeService.ModifyTimeAsync(invalidTime);

            TimeValidationException actualTimeValidationException =
                await Assert.ThrowsAsync<TimeValidationException>(modifyTimeTask.AsTask);

            // then
            actualTimeValidationException.Should().
                BeEquivalentTo(expectedTimeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTimeByIdAsync(invalidTime.Id), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTimeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Time randomTime = CreateRandomModifyTime(randomDateTime);
            Time invalidTime = randomTime;
            Time storageTime = randomTime.DeepClone();
            invalidTime.UpdatedDate = storageTime.UpdatedDate;
            var invalidTimeException = new InvalidTimeException();

            invalidTimeException.AddData(
                key: nameof(Time.UpdatedDate),
                values: $"Date is the same as {nameof(Time.UpdatedDate)}");

            var expectedTimeValidationException =
                new TimeValidationException(invalidTimeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTimeByIdAsync(invalidTime.Id)).
                    ReturnsAsync(storageTime);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Time> modifyTimeTask =
                this.timeService.ModifyTimeAsync(invalidTime);

            TimeValidationException actualTimeValidationException =
                await Assert.ThrowsAsync<TimeValidationException>(
                    modifyTimeTask.AsTask);

            // then
            actualTimeValidationException.Should()
                .BeEquivalentTo(expectedTimeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTimeByIdAsync(invalidTime.Id), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTimeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
