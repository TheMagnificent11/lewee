namespace Lewee.Domain.Tests.Unit;

public class Address : ValueObject<Address>
{
    public Address(int streetNumber, string streetName, string suburb, string state, string postcode)
    {
        this.StreetNumber = streetNumber;
        this.StreetName = streetName;
        this.Suburb = suburb;
        this.State = state;
        this.Postcode = postcode;
    }

    public int StreetNumber { get; }
    public string StreetName { get; }
    public string Suburb { get; }
    public string State { get; }
    public string Postcode { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.StreetNumber;
        yield return this.StreetName.ToLowerInvariant();
        yield return this.Suburb.ToLowerInvariant();
        yield return this.State.ToLowerInvariant();
        yield return this.Postcode.ToLowerInvariant();
    }
}
