//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Tarteeb.Api.Brokers.DateTimes;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Services.Foundations.Users;

namespace Tarteeb.Api.Services.Processings.Users
{
    public partial class UserProcessingService : IUserProcessingService
    {
        private readonly IUserService userService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserProcessingService(
            IUserService userService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.userService = userService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public User RetrieveUserByCredentails(string email, string password) =>
        TryCatch(() =>
        {
            ValidateEmailAndPassword(email, password);
            IQueryable<User> allUser = this.userService.RetrieveAllUsers();

            return allUser.FirstOrDefault(retrievedUser => retrievedUser.Email.Equals(email)
                    && retrievedUser.Password.Equals(password));
        });

        public ValueTask<Guid> VerifyUserByIdAsync(Guid userId) =>
        TryCatch(async () =>
        {
            User maybeUser = await this.userService.RetrieveUserByIdAsync(userId);
            maybeUser.IsVerified = true;
            maybeUser.UpdatedDate = this.dateTimeBroker.GetCurrentDateTime();
            await this.userService.ModifyUserAsync(maybeUser);

            return maybeUser.Id;
        });


        public async ValueTask<Guid> ActivateUserByIdAsync(Guid userId)
        {
            User maybeUser = await this.userService.RetrieveUserByIdAsync(userId);
            maybeUser.IsActive = true;
            maybeUser.UpdatedDate = this.dateTimeBroker.GetCurrentDateTime();
            await this.userService.ModifyUserAsync(maybeUser);

            return maybeUser.Id;
        }
    }
}
