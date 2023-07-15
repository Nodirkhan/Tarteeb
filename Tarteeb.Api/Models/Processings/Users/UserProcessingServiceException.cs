//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Xeptions;

namespace Tarteeb.Api.Models.Processings.Users
{
    public class UserProcessingServiceException : Xeption
    {
        public UserProcessingServiceException(Xeption innerException) 
            : base (message: "User processing service error occured, contact support", innerException)
        { }
    }
}
