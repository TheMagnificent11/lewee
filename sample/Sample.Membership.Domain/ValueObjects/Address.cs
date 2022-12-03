using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Membership.Domain.ValueObjects;

public class Address
{
    public Address(
        int streetNumber,
        string streetName,
        string streetType,
        string locality,
        string stateProvince,
        string country,
        string postcode)
    {
        this.StreetNumber = streetNumber;
        this.StreetName = streetName;
        this.StreetType = streetType;
        this.Locality = locality;
        this.StateProvince = stateProvince;
        this.Country = country;
        this.Postcode = postcode;
    }

    public Address(
        string unit,
        int streetNumber,
        string streetName,
        string streetType,
        string locality,
        string stateProvince,
        string country,
        string postcode)
        : this(streetNumber, streetName, streetType, locality, stateProvince, country, postcode)
    {
        this.Unit = unit;
    }

    public int StreetNumber { get; }
    public string StreetName { get; }
    public string StreetType { get; }
    public string Locality { get; }
    public string StateProvince { get; }
    public string Country { get; }
    public string Postcode { get; }
    public string? Unit { get; }
}
