using Server.Models;

namespace Server.Services;

public static class Checker
{
    public static void CheckUser(this UserModel input)
    {
        if (string.IsNullOrWhiteSpace(input.Id))
        {
            throw new ArgumentException("The id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(input.GivenName))
        {
            throw new ArgumentException("The given name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(input.FamilyName))
        {
            throw new ArgumentException("The family name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(input.Email))
        {
            throw new ArgumentException("The email cannot be empty.");
        }
    }

    public static void CheckDatum(this DatumModel input)
    {
        if (string.IsNullOrWhiteSpace(input.Label))
        {
            throw new ArgumentException("The label cannot be empty.");
        }
    }
}
