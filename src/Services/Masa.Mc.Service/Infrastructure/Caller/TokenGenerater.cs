﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Caller;

public class TokenGenerater : ITokenGenerater
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenGenerater(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public TokenProvider Generater()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

        string scheme = "Bearer ";

        if (!string.IsNullOrEmpty(token) && token.IndexOf(scheme) > -1)
        {
            token = token.Replace(scheme, "");
        }

        return new TokenProvider { AccessToken = token };
    }
}

