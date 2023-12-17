using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace SimpleAPI.Domain.Base;

public class SearchCriteria<T>
{
    public Specification<T> Specification { get; }
    public IEnumerable<string> Include { get; }
    public IEnumerable<Sorting<T>> Sorting { get; }

    public SearchCriteria(Specification<T>? specification = null,
        IEnumerable<string>? include = null,
        IEnumerable<Sorting<T>>? sorting = null)
    {
        Specification = specification ?? Specification<T>.True;
        Include       = include ?? Enumerable.Empty<string>();
        Sorting       = sorting ?? Enumerable.Empty<Sorting<T>>();
    }

    public SearchCriteria(Specification<T>? specification = null,
        IEnumerable<string>? include = null,
        Sorting<T>? sorting = null) : this(
        specification,
        include,
        sorting is null ? null : new[] { sorting }) { }
}

public static class SearchCriteria
{
    private sealed record Condition(string Field, string Operator, string Value);

    private static readonly HashSet<string> KnownOperators = new HashSet<string>
    {
        "eq", // Equals
        "neq", // Not equals
        "lt", // Less than
        "lte", // Less than or equal
        "gt", // Greater than
        "gte", // Greater than or equal
        "like", // Like
        "starts", // Starts with
        "ends", // Ends with
        "in", // In
        "nin" // Not In
    };

    private static readonly MethodInfo StartsWithMethod
        = typeof(string).GetMethod("StartsWith", new[] { typeof(string) }) ??
        throw new InvalidOperationException("Could not get StartsWith method for String");

    private static readonly MethodInfo EndsWithMethod =
        typeof(string).GetMethod("EndsWith", new[] { typeof(string) }) ??
        throw new InvalidOperationException("Could not get EndWith method for String");

    private static readonly MethodInfo ContainsMethod =
        typeof(string).GetMethod("Contains", new[] { typeof(string) }) ??
        throw new InvalidOperationException("Could not get Contains method for String");


    /// <summary>
    /// Parse given query parameters string to a valid <see cref="SearchCriteria{T}"/>
    /// </summary>
    /// <param name="queryParams"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static SearchCriteria<T> Parse<T>(string queryParams)
    {
        var tokens = Tokenize(queryParams);

        var specs = new SearchCriteria<T>(
            specification: ExtractSpecification<T>(tokens),
            include: ExtractIncludes<T>(tokens),
            sorting: ExtractSorting<T>(tokens)
        );
        return specs;
    }

    private static List<KeyValuePair<string, string>> Tokenize(string queryParams)
    {
        return queryParams
            .Split("&", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => s.Contains('=', StringComparison.Ordinal))
            .Select(t =>
            {
                var equalsIndex = t.IndexOf('=', StringComparison.Ordinal);
                return new KeyValuePair<string, string>(t[..equalsIndex], t[(equalsIndex + 1)..]);
            })
            .ToList();
    }

    private static List<string> ExtractIncludes<T>(IEnumerable<KeyValuePair<string, string>> tokens)
    {
        var type = typeof(T);
        var typeProperties = type.GetProperties();

        return tokens
            .Where(p => p.Key.Equals("include", StringComparison.OrdinalIgnoreCase))
            .Select(p =>
            {
                var prop = Array.Find(typeProperties, prop => prop.Name.Equals(p.Value, StringComparison.OrdinalIgnoreCase));

                return prop?.Name ?? String.Empty;
            })
            .Where(s => !String.IsNullOrWhiteSpace(s))
            .ToList();
    }

    private static List<Sorting<T>> ExtractSorting<T>(IEnumerable<KeyValuePair<string, string>> tokens)
    {
        var toReturn = new List<Sorting<T>>();
        var type = typeof(T);
        var typeProperties = type.GetProperties();
        var sortingTerms = tokens
            .Where(p => p.Key.Equals("sort", StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Value);

        foreach (var sorting in sortingTerms)
        {
            var direction = sorting[0] == '-'
                ? Sorting.SortDirection.Descending
                : Sorting.SortDirection.Ascending;
            var sortingProperty = sorting.TrimStart(new char[] { '-', '+' });
            var propertyName = sortingProperty.Split('.').First();
            var prop = Array.Find(typeProperties, p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (prop == null) continue;

            var expParam = Expression.Parameter(type, "p");
            var expProp = propertyName == sortingProperty
                ? Expression.Property(expParam, prop.Name)
                : GetNestedProperty(expParam, sortingProperty);

            // https://stackoverflow.com/a/8974880/3410871
            if (expProp.Type.IsValueType)
                expProp = Expression.Convert(expProp, typeof(object));
            var expression = Expression.Lambda<Func<T, object>>(expProp, expParam);

            toReturn.Add(new Sorting<T>(expression, direction));
        }
        return toReturn;
    }

    private static Specification<T> ExtractSpecification<T>(List<KeyValuePair<string, string>> tokens)
    {
        var type = typeof(T);
        var typeProperties = type.GetProperties();
        var specifications = new List<Specification<T>>();

        var conditions = tokens
            .Where(p => !p.Key.Equals("include", StringComparison.OrdinalIgnoreCase) &&
                !p.Key.Equals("sort", StringComparison.OrdinalIgnoreCase))
            .Select(p =>
            {
                var separatorIndex = p.Key.IndexOf(':', StringComparison.Ordinal);
                var field = separatorIndex == -1
                    ? p.Key
                    : p.Key[..(separatorIndex)];
                var @operator = separatorIndex == -1
                    ? "eq"
                    : p.Key[(separatorIndex + 1)..];
                var value = p.Value;

                return new Condition(field, @operator, value);
            })
            .ToList();

        if (conditions.Count == 0)
            return Specification<T>.True;

        foreach (var condition in conditions)
        {
            var propertyName = condition.Field.Split('.').First();
            var prop = Array.Find(typeProperties,
                p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (prop == null) continue;

            if (!KnownOperators.Contains(condition.Operator))
                throw new ArgumentException($"Unsupported operator: {condition.Operator}");

            var result = condition.Operator == "in" || condition.Operator == "nin"
                ? MultiExpressionForCondition<T>(condition, prop)
                : SingleExpressionForCondition<T>(condition, prop);
            specifications.Add(new Specification<T>(result));
        }

        return specifications.Count == 0
            ? Specification<T>.True
            : Specification.CombineAnd(specifications.ToArray());
    }

    private static Expression GetNestedProperty(ParameterExpression param, string propertyName)
    {
        // TODO We should support nested collections
        Expression toReturn = param;
        foreach (var member in propertyName.Split('.'))
        {
            toReturn = Expression.PropertyOrField(toReturn, member);
        }
        return toReturn;
    }

    private static Expression<Func<T, bool>> SingleExpressionForCondition<T>(Condition condition, PropertyInfo prop)
    {
        var propertyName = condition.Field.Split('.').First();
        var expParam = Expression.Parameter(typeof(T), "p");
        var expMember = propertyName == condition.Field
            ? Expression.Property(expParam, prop.Name)
            : GetNestedProperty(expParam, condition.Field);
        var expValue = Expression.Constant(Cast(expMember.Type, condition.Value));
        Expression expBody = condition.Operator switch
        {
            "eq" => Expression.Equal(expMember, expValue),
            "neq" => Expression.NotEqual(expMember, expValue),
            "lt" => Expression.LessThan(expMember, expValue),
            "lte" => Expression.LessThanOrEqual(expMember, expValue),
            "gt" => Expression.GreaterThan(expMember, expValue),
            "gte" => Expression.GreaterThanOrEqual(expMember, expValue),
            "like" => Expression.Call(expMember, ContainsMethod, expValue),
            "starts" => Expression.Call(expMember, StartsWithMethod, expValue),
            "ends" => Expression.Call(expMember, EndsWithMethod, expValue),
            _ => throw new ArgumentException($"Unknown operator: {condition.Operator}")
        };
        var result = Expression.Lambda<Func<T, bool>>(expBody, expParam);
        return result;
    }

    private static Expression<Func<T, bool>> MultiExpressionForCondition<T>(Condition condition, PropertyInfo prop)
    {
        var propertyName = condition.Field.Split('.').First();
        var expParam = Expression.Parameter(typeof(T), "p");
        var expMember = propertyName == condition.Field
            ? Expression.Property(expParam, prop.Name)
            : GetNestedProperty(expParam, condition.Field);
        var conditionValues = Regex.Split(condition.Value, @"(?<!\?),");
        if (conditionValues.Length == 0)
            throw new ArgumentException("Condition value is empty!");

        // TODO Revisit
        // if (typeof(EntityID).IsAssignableFrom(expMember.Type))
        // {
        //     // Due to EF Core shortcomings on using ValueObject as keys we cannot use 'Contains', thus
        //     // we shall rewrite in as OR clause!
        //     var expValue = Expression.Constant(
        //         InstanceFactory.CreateInstance(expMember.Type, Int64.Parse(conditionValues[0])), expMember.Type);
        //     var expBodyOr = Expression.Equal(expMember, expValue);
        //     for (var i = 1; i < conditionValues.Length; i++)
        //     {
        //         expValue = Expression.Constant(InstanceFactory
        //             .CreateInstance(expMember.Type, Int64.Parse(conditionValues[i])), expMember.Type);
        //         expBodyOr = Expression.OrElse(expBodyOr, Expression.Equal(expMember, expValue));
        //     }
        //
        //     return condition.Operator == "nin"
        //         ? Expression.Lambda<Func<T, bool>>(Expression.Not(expBodyOr), expParam)
        //         : Expression.Lambda<Func<T, bool>>(expBodyOr, expParam);
        // }

        var containsMethod = typeof(Enumerable)
            .GetMethods()
            .Where(m => m.Name == "Contains")
            .Single(m => m.GetParameters().Length == 2)
            .MakeGenericMethod(expMember.Type);
        var values = conditionValues.Select(v => Expression.Constant(SafeConvert(v, expMember.Type)));
        var expValueArray = Expression.NewArrayInit(expMember.Type, values);
        var expBody = Expression.Call(null, containsMethod, expValueArray, expMember);

        return condition.Operator == "nin"
            ? Expression.Lambda<Func<T, bool>>(Expression.Not(expBody), expParam)
            : Expression.Lambda<Func<T, bool>>(expBody, expParam);
    }

    private static object SafeConvert(string value, Type type)
    {
        var converter = TypeDescriptor.GetConverter(type);
        if (converter.CanConvertFrom(typeof(String)))
            return (string)converter.ConvertFrom(value)!;

        if (type.IsEnum)
            return Enum.Parse(type, value);
        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }

    private static object? Cast(this Type type, object value)
    {
        if (typeof(IEntityID).IsAssignableFrom(type))
            return Activator.CreateInstance(type, Convert.ToInt32(value, CultureInfo.InvariantCulture));
        
        var param = Expression.Parameter(typeof(string), "d");
        var body = Expression.Block(Expression.Convert(Expression.Convert(param, value.GetType()), type));

        var exp = Expression.Lambda(body, param).Compile();
        return exp.DynamicInvoke(value);
    }
}
