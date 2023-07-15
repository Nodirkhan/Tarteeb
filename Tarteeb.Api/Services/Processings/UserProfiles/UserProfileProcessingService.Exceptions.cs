//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Xeptions;

namespace Tarteeb.Api.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingService
    {
        private delegate ValueTask<UserProfile> ReturningUserProfileFunction();
        private delegate IQueryable<UserProfile> ReturningUserProfilesFunction();

        private async ValueTask<UserProfile> TryCatch(ReturningUserProfileFunction returningFunction)
        {
            try
            {
                return await returningFunction();
            }
            catch(InvalidUserProfileProcessingException invalidUserProfileException)
            {
                throw CreateAndLogValidationException(invalidUserProfileException);
            }
            catch(NotFoundUserException notFoundUserException)
            {
                throw CreateAndLogValidationException(notFoundUserException);
            }
            catch (UserValidationException userValidationException)
            {
                throw CreateAndLogDependencyValidationException(userValidationException);
            }
            catch(UserDependencyException userDependencyException)
            {
                throw CreateAndLogDependencyException(userDependencyException);
            }
            catch (UserServiceException userServiceException)
            {
                throw CreateAndLogDependencyException(userServiceException);
            }
            catch(Exception serviceException)
            {
                var failedUserProfileProcessingServiceException =
                    new FailedUserProfileProcessingServiceException(serviceException);

                throw CreateAndLogServiceException(failedUserProfileProcessingServiceException);
            }
        }

        private IQueryable<UserProfile> TryCatch(ReturningUserProfilesFunction returningFunction)
        {
            try
            {
                return returningFunction();
            }
            catch(UserProfileProcessingDependencyException userProfileProcessingDependencyException)
            {
                throw CreateAndLogDependencyException(userProfileProcessingDependencyException);
            }
            catch (UserProfileProcessingServiceException userProfileProcessingServiceException)
            {
                throw CreateAndLogDependencyException(userProfileProcessingServiceException);
            }
        }

        private UserProfileProcessingValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userProfileValidationException = new UserProfileProcessingValidationException(exception);
            this.loggingBroker.LogError(userProfileValidationException);

            return userProfileValidationException;
        }
       
        private UserProfileProcessingDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var userProfileProcessingDependencyValidationException =
                new UserProfileProcessingDependencyValidationException(exception.InnerException as Xeption);

            this.loggingBroker.LogError(userProfileProcessingDependencyValidationException);

            return userProfileProcessingDependencyValidationException;
        }

        private UserProfileProcessingDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var userProfileProcessingDependencyException =
                new UserProfileProcessingDependencyException(exception.InnerException as Xeption);

            this.loggingBroker.LogError(userProfileProcessingDependencyException);

            return userProfileProcessingDependencyException;
        }


        private UserProfileProcessingServiceException CreateAndLogServiceException(Xeption exception)
        {
            var userProfileProcessingServiceException =
                new UserProfileProcessingServiceException(exception);

            this.loggingBroker.LogError(userProfileProcessingServiceException);

            return userProfileProcessingServiceException;
        }
    }
}
