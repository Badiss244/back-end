using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

public static class QueryableExtensions
{
    public static IQueryable<T> IncludeAll<T>(this IQueryable<T> query, DbContext context) where T : class
    {
        var entityType = context.Model.FindEntityType(typeof(T));
        var navigations = entityType.GetNavigations();
        foreach (var navigation in navigations)
        {
            query = query.Include(navigation.Name);
        }
        return query;
    }
}
