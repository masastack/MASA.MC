// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.OptionsResolve;

public static class PushOptionsMapper
{
    private static readonly ConcurrentDictionary<Type, Delegate> _mappers = new();

    public static T Map<T>(ConcurrentDictionary<string, object> source) where T : class, IOptions
    {
        var mapper = (Func<ConcurrentDictionary<string, object>, T>)_mappers.GetOrAdd(
            typeof(T),
            t => BuildMapper<T>()
        );

        return mapper(source);
    }

    private static Func<ConcurrentDictionary<string, object>, T> BuildMapper<T>() where T : class, IOptions
    {
        var sourceParam = Expression.Parameter(typeof(ConcurrentDictionary<string, object>), "source");
        var targetVar = Expression.Variable(typeof(T), "target");

        var variables = new List<ParameterExpression> { targetVar };
        var expressions = new List<Expression>
    {
        // target = new T()
        Expression.Assign(targetVar, Expression.New(typeof(T)))
    };

        foreach (var p in typeof(T).GetProperties().Where(p => p.CanWrite && p.PropertyType == typeof(string)))
        {
            var valueVar = Expression.Variable(typeof(object), "value");
            variables.Add(valueVar);

            var tryGetValue = Expression.Call(
                sourceParam,
                typeof(ConcurrentDictionary<string, object>).GetMethod("TryGetValue"),
                Expression.Constant(p.Name),
                valueVar
            );

            var toStringExpr = Expression.Condition(
                Expression.NotEqual(valueVar, Expression.Constant(null)),
                Expression.Call(valueVar, typeof(object).GetMethod("ToString")!),
                Expression.Constant(string.Empty)
            );

            var assignProp = Expression.IfThen(
                tryGetValue,
                Expression.Assign(
                    Expression.Property(targetVar, p),
                    toStringExpr
                )
            );

            expressions.Add(assignProp);
        }

        expressions.Add(targetVar); // return target

        var body = Expression.Block(variables, expressions);
        var lambda = Expression.Lambda<Func<ConcurrentDictionary<string, object>, T>>(body, sourceParam);
        return lambda.Compile();
    }
}