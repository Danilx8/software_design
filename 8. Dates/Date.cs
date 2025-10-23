namespace Software_Design._8._Dates;

//import java.text.ParseException;
// import java.text.SimpleDateFormat;
// import java.util.Date;
// 
// public class DateExample {
//     public static void main(String[] args) {
//         String dateString = "2024-05-13 14:30:00";
//         SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
//         try {
//             Date date = format.parse(dateString);
//             System.out.println("Date: " + date);
//         } catch (ParseException e) {
//             e.printStackTrace();
//         }
//     }
// }
// 

// в коде выше дана голая дата + время без указания формата, без указания часового пояса или чего-либо ещё
// нигде в задаче не уточнено, чего именно мы ожидаем от кода: локального времени или универсального
// для любого проекта актуально правило сохранять единый формат для подобных данных, поэтому на сервере я бы
// испольвзовал UTC, а на фронтенде уже переводил бы его в локальное для пользователя время (с ssr это, конечно,
// сложнее, но куда деваться). Также часто встречаются трудности с американским форматом времени, и это всё чинится 
// разделением строки на оотдельные части (год, месяц, день итд). в примере, по крайней мере, понятно, что дата
// европейская

public class Date
{
    public static void Run()
    {
        string dateString = "2024-05-13 14:30:00";
        try
        {
            int year = int.Parse(dateString.Split("-")[0]);
            int month = int.Parse(dateString.Split("-")[1]);
            int day = int.Parse(dateString.Split("-")[2].Split(" ")[0]);
            int hour = int.Parse(dateString.Split(" ")[1].Split(":")[0]);
            int minute = int.Parse(dateString.Split(" ")[1].Split(":")[1]);
            int second = int.Parse(dateString.Split(" ")[1].Split(":")[2]);
            DateTime date = new DateTime(year, month, day, hour, minute, second);
            
            Console.WriteLine($"Local time: {date.ToLocalTime()}");
            Console.WriteLine($"Universal time: {date.ToUniversalTime()}");
            Console.WriteLine($"Long date: {date.ToLongDateString()}");
            Console.WriteLine($"Short date: {date.ToShortDateString()}");
            Console.WriteLine($"Long time: {date.ToLongTimeString()}");
            Console.WriteLine($"Short time: {date.ToShortTimeString()}");
            
            // Local time: 13.05.2024 17:30:00 (за меня сделал вывод, что время было в UTC)
            // Universal time: 13.05.2024 11:30:00 (опять же за меня решил, что время было НЕ в UTC, а местным)
            // Long date: Monday, 13 May 2024 (к этим вариантам претензий нет)
            // Short date: 13.05.2024
            // Long time: 14:30:00
            // Short time: 14:30
            
            // За неимением параметров для этих функций, держим всё на сервере в UTC
        }
        catch (FormatException exception)
        {
            Console.WriteLine(exception.StackTrace);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.StackTrace);
        }
    }
}