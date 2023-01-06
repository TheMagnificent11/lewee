using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lewee.Domain;

namespace Sample.Orders.Domain;

public class MenuItem : BaseEntity, IAggregateRoot
{
    public MenuItem(string name, decimal price)
    {
        this.Name = name;
        this.Price = price;
    }

    public string Name { get; protected set; }
    public decimal Price { get; protected set; }
}
