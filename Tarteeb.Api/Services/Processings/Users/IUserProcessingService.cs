﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Threading.Tasks;
using Tarteeb.Api.Models;

namespace Tarteeb.Api.Services.Processings.Users
{
    public interface IUserProcessingService
    {
        public ValueTask<User> RetrieveUserByCredentails(string email, string password);
    }
}