namespace Sample.Restaurant.Domain.Queries;

public static class FindTableByNumberQuery
{
    public static IQueryable<Table> FindByNumber(this IQueryable<Table> tables, int tableNumber)
    {
        return tables.Where(x => x.TableNumber == tableNumber && !x.IsDeleted);
    }
}
