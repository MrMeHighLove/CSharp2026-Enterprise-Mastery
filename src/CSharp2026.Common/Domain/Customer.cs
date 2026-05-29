// -----------------------------------------------------------------------
//   Customer.cs
//   Customer entity. Identity by CustomerId; name and email are mutable.
// -----------------------------------------------------------------------

using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Common.Domain;

public sealed class Customer
{
    public CustomerId Id    { get; }
    public string     Name  { get; private set; }
    public Email      Email { get; private set; }

    public Customer(CustomerId id, string name, Email email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
        Email = email;
    }

    public void Rename(string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        Name = newName;
    }

    public void ChangeEmail(Email newEmail) => Email = newEmail;
}
