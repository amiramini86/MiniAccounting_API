namespace MiniAccounting.Domain.Entities;

public class Account
{
    public int Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }

    public Account(string code, string name)
    {
        Code = code;
        Name = name;
    }
}