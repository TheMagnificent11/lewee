﻿namespace Sample.Restaurant.Domain;
public class MenuItemReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
