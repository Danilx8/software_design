namespace Software_Design._2._Bank_Account;

public class BankAccount(double initialValue)
{
    private double _balance = initialValue;

    public void deposit(double depositValue)
    {
        _balance += depositValue;
    }

    public void withdraw(double withdrawValue)
    {
        _balance -= withdrawValue;
    }

    public double getBalance()
    {
        return _balance;
    }
}