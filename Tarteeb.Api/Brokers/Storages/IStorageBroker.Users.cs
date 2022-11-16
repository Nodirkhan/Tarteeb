﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using System.Linq;
using Tarteeb.Api.Models;

namespace Tarteeb.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
       IQueryable<User> SelectAllUsers();
        ValueTask<User> SelectUserByIdAsync(Guid id);

        ValueTask<User> UpdateUserAsync(User user);
    }
}
