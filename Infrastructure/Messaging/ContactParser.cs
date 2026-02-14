using System.Text.RegularExpressions;

namespace PayByLink.Api.Infrastructure.Messaging;

public static class ContactParser
{
    // Bem simples pro MVP: E.164 (ex: +4915212345678)
    private static readonly Regex E164 = new(@"^\+\d{8,15}$", RegexOptions.Compiled);

    public static bool IsPhoneE164(string contact) => E164.IsMatch(contact.Trim());
    public static bool IsEmail(string contact) => contact.Contains("@") && contact.Contains(".");
}
