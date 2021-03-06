// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services;

public abstract class ServiceBase
{
    protected ICallerProvider CallerProvider { get; init; }

    protected abstract string BaseUrl { get; set; }

    protected ServiceBase(ICallerProvider callerProvider)
    {
        CallerProvider = callerProvider;
    }

    protected async Task<TResponse> GetAsync<TRequest, TResponse>(string methodName, TRequest data) where TRequest : class
    {
        return await CallerProvider.GetAsync<TRequest, TResponse>(BuildAdress(methodName), data) ?? throw new Exception("The service is abnormal, please contact the administrator!");
    }

    protected async Task<TResponse> GetAsync<TResponse>(string methodName)
    {
        return await CallerProvider.GetAsync<TResponse>(BuildAdress(methodName)) ?? throw new Exception("The service is abnormal, please contact the administrator!");
    }

    protected async Task PutAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await CallerProvider.PutAsync(BuildAdress(methodName), data);
        await CheckResponse(response);
    }

    protected async Task PostAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await CallerProvider.PostAsync(BuildAdress(methodName), data);
        await CheckResponse(response);
    }

    protected async Task PostAsync(string methodName)
    {
        var response = await CallerProvider.PostAsync(BuildAdress(methodName), null);
        await CheckResponse(response);
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string methodName, TRequest data)
    {
        var response = await CallerProvider.PostAsync<TRequest, TResponse>(BuildAdress(methodName), data);
        return response;
    }

    protected async Task DeleteAsync<TRequest>(string methodName, TRequest? data = default)
    {
        var response = await CallerProvider.DeleteAsync(BuildAdress(methodName), data);
        await CheckResponse(response);
    }

    protected async Task DeleteAsync(string methodName)
    {
        var response = await CallerProvider.DeleteAsync(BuildAdress(methodName), null);
        await CheckResponse(response);
    }

    protected string BuildAdress(string methodName)
    {
        return Path.Combine(BaseUrl, methodName.Replace("Async", ""));
    }

    protected async ValueTask CheckResponse(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK or HttpStatusCode.Accepted or HttpStatusCode.NoContent: break;
            case (HttpStatusCode)MasaHttpStatusCode.UserFriendlyException: throw new Exception(await response.Content.ReadAsStringAsync());
            default: throw new Exception("The service is abnormal, please contact the administrator!");
        }
    }
}
