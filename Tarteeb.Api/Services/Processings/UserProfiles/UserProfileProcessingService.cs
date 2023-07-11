//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Models.Foundations.Emails;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Services.Foundations.Users;

namespace Tarteeb.Api.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingService : IUserProfileProcessingService
    {
        private readonly IUserService userService;
        private readonly ILoggingBroker loggingBroker;

        public UserProfileProcessingService(
            IUserService userService,
            ILoggingBroker loggingBroker)
        {
            this.userService = userService;
            this.loggingBroker = loggingBroker;
        }

        public IQueryable<UserProfile> RetrieveAllUserProfiles() =>
        TryCatch(() =>
        {
            IQueryable<User> users =
                this.userService.RetrieveAllUsers();

            IQueryable<UserProfile> populatedUserProfiles =
                users.Select(user => PopulateUserProfile(user));

            return populatedUserProfiles;
        });

        public ValueTask<UserProfile> RetrieveUserProfileByIdAsync(Guid userProfileId) =>
        TryCatch(async () =>
        {
            ValidateUserProfileId(userProfileId);
            var maybeUser = await this.userService.RetrieveUserByIdAsync(userProfileId);
            ValidateStorageUser(userProfileId, maybeUser);
            UserProfile populatedUserProfile = PopulateUserProfile(maybeUser);

            return populatedUserProfile;
        });

        private UserProfile PopulateUserProfile(User user)
        {
            return new UserProfile
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
            };
        }
    }
}
