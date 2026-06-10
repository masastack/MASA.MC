// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services;

public abstract class ServiceBase
{
    protected ICaller Caller { get; init; }

    protected abstract string BaseUrl { get; set; }

    protected ServiceBase(ICaller caller)
    {
        Caller = caller;
    }

    protected async Task<TResponse> GetAsync<TResponse>(string methodName, Dictionary<string, string>? paramters = null)
    {
        return await Caller.GetAsync<TResponse>(BuildAdress(methodName), paramters ?? new()) ?? throw new UserFriendlyException("The service is abnormal, please contact the administrator!");
    }

    protected async Task<TResponse> GetAsync<TRequest, TResponse>(string methodName, TRequest data) where TRequest : class
    {
        return await GetByQueryAsync<TRequest, TResponse>(methodName, data);
    }

    protected async Task PutAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await Caller.PutAsync(BuildAdress(methodName), data);
    }

    protected async Task PostAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await Caller.PostAsync(BuildAdress(methodName), data);
    }

    protected async Task PostAsync(string methodName)
    {
        var response = await Caller.PostAsync(BuildAdress(methodName), null);
    }

    protected async Task<TResponse> PostAsync<TResponse>(string methodName, HttpContent content)
    {
        var response = await Caller.PostAsync(BuildAdress(methodName), content);
        var json = await response.Content.ReadAsStringAsync();
        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true
        };
        return JsonSerializer.Deserialize<TResponse>(json, serializeOptions) ?? throw new UserFriendlyException("Internal error, please contact administrator");
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string methodName, TRequest data)
    {
        return await Caller.PostAsync<TRequest, TResponse>(BuildAdress(methodName), data) ?? throw new UserFriendlyException("The service is abnormal, please contact the administrator!");
    }

    protected async Task DeleteAsync<TRequest>(string methodName, TRequest? data = default)
    {
        var response = await Caller.DeleteAsync(BuildAdress(methodName), data);
    }

    protected async Task DeleteAsync(string methodName)
    {
        var response = await Caller.DeleteAsync(BuildAdress(methodName), null);
    }

    protected async Task SendAsync<TRequest>(string methodName, TRequest? data = default)
    {
        if (methodName.StartsWith("Add")) await PostAsync(methodName, data);
        else if (methodName.StartsWith("Update")) await PutAsync(methodName, data);
        else if (methodName.StartsWith("Remove")) await DeleteAsync(methodName, data);
    }

    protected async Task<TResponse> SendAsync<TRequest, TResponse>(string methodName, TRequest data) where TRequest : class
    {
        return await GetByQueryAsync<TRequest, TResponse>(methodName, data);
    }

    private async Task<TResponse> GetByQueryAsync<TRequest, TResponse>(string methodName, TRequest data) where TRequest : class
    {
        return await GetAsync<TResponse>(methodName, BuildQueryParameters(data));
    }

    private static Dictionary<string, string> BuildQueryParameters<TRequest>(TRequest data) where TRequest : class
    {
        var parameters = new Dictionary<string, string>();
        var properties = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var property in properties)
        {
            if (!property.CanRead)
            {
                continue;
            }

            var value = property.GetValue(data);
            if (value == null)
            {
                continue;
            }

            parameters[property.Name] = SerializeQueryValue(value);
        }

        return parameters;
    }

    private static string SerializeQueryValue(object value)
    {
        return value switch
        {
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToUniversalTime().UtcDateTime.ToString("O"),
            DateTime dateTime => (dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime()).ToString("O"),
            _ => value.ToString() ?? string.Empty
        };
    }

    string BuildAdress(string methodName)
    {
        return Path.Combine(BaseUrl, methodName.Replace("Async", ""));
    }
}