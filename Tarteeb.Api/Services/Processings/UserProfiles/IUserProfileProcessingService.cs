//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Processings.UserProfiles;

namespace Tarteeb.Api.Services.Processings.UserProfiles
{
    public interface IUserProfileProcessingService
    {
        ValueTask<UserProfile> RetrieveUserProfileByIdAsync(Guid userProfileId);
        IQueryable<UserProfile> RetrieveAllUserProfiles();
    }
}
