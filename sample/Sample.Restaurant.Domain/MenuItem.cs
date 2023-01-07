using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class MenuItem : BaseEntity, IAggregateRoot
{
    private readonly List<Order> orders;

    public MenuItem(string name, decimal price)
    {
        this.orders = new List<Order>();

        this.Name = name;
        this.Price = price;

        this.Orders = this.orders;
    }

    public string Name { get; protected set; }
    public decimal Price { get; protected set; }
    public IReadOnlyCollection<Order> Orders { get; protected set; }
}
