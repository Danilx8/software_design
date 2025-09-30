namespace Software_Design._3._Fixed_Bank_Account;

public class BankAccount(double initialValue)
{
    private double _balance = initialValue;

    public void deposit(double depositValue)
    {
        if (depositValue < 0) throw new Exception("Can't deposit negative value");
        if (depositValue > _balance) throw new Exception($"Can't deposit more than current balance: {_balance}");

        _balance += depositValue;
    }

    public void withdraw(double withdrawValue)
    {
        if (withdrawValue < 0) throw new Exception("Can't withdraw negative value");
        if (withdrawValue > _balance) throw new Exception($"Can't withdraw more than current balance: {_balance}");
        _balance -= withdrawValue;
    }

    public double getBalance()
    {
        return _balance;
    }
}