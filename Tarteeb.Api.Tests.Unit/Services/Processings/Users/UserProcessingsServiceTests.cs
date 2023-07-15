//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using Tarteeb.Api.Brokers.DateTimes;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Services.Foundations.Users;
using Tarteeb.Api.Services.Processings.Users;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.Users
{
    public partial class UserProcessingsServiceTests
    {
        private readonly Mock<IUserService> userServiceMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserProcessingService userProcessingsService;

        public UserProcessingsServiceTests()
        {
            this.userServiceMock = new Mock<IUserService>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userProcessingsService = new UserProcessingService(
                userService: this.userServiceMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> UserDependencyExceptions()
        {
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new UserDependencyException(someInnerException),
                new UserServiceException(someInnerException)
            };
        }

        public static TheoryData<Xeption> UserDependencyValidationExceptions()
        {
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>()
            {
                new UserValidationException(someInnerException),
                new UserDependencyValidationException(someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
           actualException => actualException.SameExceptionAs(expectedException);

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDate() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private User CreateRandomUserWithCredentials(string email, string password)
        {
            User randomUser = CreateUserFiller().Create();

            randomUser.Email = email;
            randomUser.Password = password;

            return randomUser;
        }

        private IQueryable<User> CreateRandomUsersIncluding(User existingUser)
        {
            List<User> randomUsers = CreateUserFiller()
                .Create(count: GetRandomNumber())
                    .ToList();

            randomUsers.Add(existingUser);

            return randomUsers.AsQueryable();
        }

        private User CreateRandomUser() =>
            CreateUserFiller().Create();

        private Filler<User> CreateUserFiller()
        {
            DateTimeOffset dates = GetRandomDate();
            var filler = new Filler<User>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}
