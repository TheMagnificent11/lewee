using Saji.Domain;

namespace Sample.Membership.Domain.ValueObjects;

public class Address : IValueObject<Address>
{
    public Address(
        string address1,
        string? address2,
        string? address3,
        string locality,
        string stateProvince,
        string country,
        string postcode,
        string? propertyIdentificationCode = null)
    {
        this.Address1 = address1;
        this.Address2 = address2;
        this.Address3 = address3;
        this.Locality = locality;
        this.StateProvince = stateProvince;
        this.Country = country;
        this.Postcode = postcode;
        this.PropertyIdentificationCode = propertyIdentificationCode;
    }

    public string Address1 { get; }
    public string? Address2 { get; }
    public string? Address3 { get; }
    public string Locality { get; }
    public string StateProvince { get; }
    public string Country { get; }
    public string Postcode { get; }
    public string? PropertyIdentificationCode { get; private set; }

    public bool Equals(Address? other)
    {
        if (other == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(this.PropertyIdentificationCode) && !string.IsNullOrWhiteSpace(other.PropertyIdentificationCode))
        {
            return this.PropertyIdentificationCode.Equals(other.PropertyIdentificationCode, StringComparison.OrdinalIgnoreCase);
        }

        if (!this.Address1.Equals(other.Address1, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!IsEqual(this.Address2, other.Address2))
        {
            return false;
        }

        if (!IsEqual(this.Address3, other.Address3))
        {
            return false;
        }

        if (!this.Locality.Equals(other.Locality, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!this.StateProvince.Equals(other.StateProvince, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!this.Country.Equals(other.Country, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!this.Postcode.Equals(other.Postcode, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return this.Equals(obj as Address);
    }

    public override int GetHashCode()
    {
        var hashCode =
            this.Address1.GetHashCode() +
            this.Locality.GetHashCode() +
            this.StateProvince.GetHashCode() +
            this.Country.GetHashCode() + this.Postcode.GetHashCode();

        if (this.Address2 != null)
        {
            hashCode += this.Address2.GetHashCode();
        }

        if (this.Address3 != null)
        {
            hashCode += this.Address3.GetHashCode();
        }

        if (this.PropertyIdentificationCode != null)
        {
            hashCode += this.PropertyIdentificationCode.GetHashCode();
        }

        return hashCode;
    }

    public void SetPropertyIdentificationCode(string propertyIdentificationCode)
    {
        if (propertyIdentificationCode.Equals(this.PropertyIdentificationCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        this.PropertyIdentificationCode = propertyIdentificationCode.ToUpperInvariant();
    }

    private static bool IsEqual(string? string1, string? string2)
    {
        if (string.IsNullOrWhiteSpace(string1))
        {
            if (!string.IsNullOrWhiteSpace(string2))
            {
                return false;
            }

            return true;
        }

        return string1.Equals(string2, StringComparison.OrdinalIgnoreCase);
    }
}
