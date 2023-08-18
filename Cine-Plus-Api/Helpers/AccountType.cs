namespace Cine_Plus_Api.Helpers;

public enum AccountType
{
    Admin,
    Manager,
    Employ,
    User,
}

public static class AccountTypeMethods
{
    public static string ToString(AccountType accountType)
    {
        return accountType switch
        {
            AccountType.Admin => "admin",
            AccountType.Employ => "employ",
            AccountType.Manager => "manager",
            AccountType.User => "user",
            _ => throw new ArgumentOutOfRangeException(nameof(accountType), accountType, null)
        };
    }

    public static AccountType ToAccountType(string accountType)
    {
        return accountType switch
        {
            "admin" => AccountType.Admin,
            "employ" => AccountType.Employ,
            "manager" => AccountType.Manager,
            "user" => AccountType.User,
            _ => throw new ArgumentOutOfRangeException(nameof(accountType), accountType, null)
        };
    }

    private static int ToNumber(AccountType accountType)
    {
        return accountType switch
        {
            AccountType.Admin => 4,
            AccountType.Employ => 2,
            AccountType.Manager => 3,
            AccountType.User => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(accountType), accountType, null)
        };
    }

    public static bool Authorize(AccountType current, AccountType accountType) =>
        ToNumber(current) >= ToNumber(accountType);
}