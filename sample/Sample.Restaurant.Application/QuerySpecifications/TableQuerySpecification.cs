using Ardalis.Specification;
using Lewee.Domain;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application.QuerySpecifications;

internal class TableQuerySpecification : QuerySpecification<Table>
{
    public TableQuerySpecification(int tableNumber)
    {
        this.Query.Where(x => x.TableNumber == tableNumber);
    }
}
