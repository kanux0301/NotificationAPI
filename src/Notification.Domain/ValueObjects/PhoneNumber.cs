using System.Text.RegularExpressions;
using Notification.Domain.Common;

namespace Notification.Domain.ValueObjects;

public partial class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));

        var normalized = NormalizePhoneNumber(phoneNumber);

        if (!PhoneRegex().IsMatch(normalized))
            throw new ArgumentException("Invalid phone number format.", nameof(phoneNumber));

        return new PhoneNumber(normalized);
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        return new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;

    [GeneratedRegex(@"^\+?[1-9]\d{6,14}$", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();
}
