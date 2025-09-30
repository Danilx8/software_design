namespace Software_Design._2._Bank_Account;

public class Tests
{
    private BankAccount _account;
    
    public void Run()
    {
        _account = new BankAccount(100.00);
        
        // Нормальные операции (добавление и снятие положительных значений, которые не превышают текущего баланса)
        _account.deposit(25.00);
        _account.withdraw(75.00);
        _account.getBalance();
        // 50
        
        // Теперь "недопустимые" операции
        // Здесь мы "кладём" -50 денег
        _account.deposit(-50.00);
        
        // Далее снимаем больше чем есть на счёте
        _account.withdraw(10.00);
        
        // В конце "снимем" со счёта негативное число, чтобы выйти в плюс
        _account.withdraw(-10.00);

        // Остаёмся с нулём
        _account.getBalance();
    }
}