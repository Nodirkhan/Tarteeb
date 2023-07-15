//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using Xeptions;

namespace Tarteeb.Api.Models.Processings.Users
{
    public class FailedUserProcessingServiceException : Xeption
    {
        public FailedUserProcessingServiceException(Exception innerException)
            : base (message: "Failed user error occurred, please contact support", innerException)
        { }
    }
}
