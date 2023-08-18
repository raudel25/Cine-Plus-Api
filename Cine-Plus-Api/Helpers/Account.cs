namespace Cine_Plus_Api.Helpers;

public abstract class Account
{
    public abstract int Level();

    public static Account StringToAccount(string account)
    {
        var accounts = new Account[]
            { new AdminAccount(), new EmployAccount(), new AdminAccount(), new ManagerAccount() };
        var result = accounts.FirstOrDefault(a => a.ToString() == account);

        if (result is null)
            throw new ArgumentOutOfRangeException(nameof(Account), account, null);

        return result;
    }

    public abstract override string ToString();
}

public class AdminAccount : Account
{
    public override int Level() => 4;
    public override string ToString() => "admin";
}

public class ManagerAccount : Account
{
    public override int Level() => 3;
    public override string ToString() => "admin";
}

public class EmployAccount : Account
{
    public override int Level() => 2;
    public override string ToString() => "employ";
}

public class UserAccount : Account
{
    public override int Level() => 1;
    public override string ToString() => "user";
}