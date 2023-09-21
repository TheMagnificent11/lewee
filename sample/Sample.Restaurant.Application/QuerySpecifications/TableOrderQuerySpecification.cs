using Ardalis.Specification;
using Lewee.Domain;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application.QuerySpecifications;

internal class TableOrderQuerySpecification : QuerySpecification<Table>
{
    public TableOrderQuerySpecification(int tableNumber)
    {
        this.Query
            .Where(x => x.TableNumber == tableNumber)
            .Include(x => x.Orders)
                .ThenInclude(x => x.Items);
    }
}
