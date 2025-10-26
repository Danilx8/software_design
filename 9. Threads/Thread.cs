namespace Software_Design._9._Threads;

//public class ThreadExample {
//     private static int counter = 0;
// 
//     public static void main(String[] args) {
//         Runnable task = () -> {
//             for (int i = 0; i < 1000; i++) {
//                 counter++;
//             }
//         };
// 
//         Thread thread1 = new Thread(task);
//         Thread thread2 = new Thread(task);
// 
//         thread1.start();
//         thread2.start();
// 
//         try {
//             thread1.join();
//             thread2.join();
//         } catch (InterruptedException e) {
//             e.printStackTrace();
//         }
// 
//         System.out.println("Counter: " + counter);
//     }
// }
// 

// counter++ - по факту атомарная операция. можно сделать её такой и в коде 

public class ThreadExample
{
    private static int _counter = 0;
    
    public static void Run()
    {
        Task task1 = Task.Run(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                Interlocked.Increment(ref _counter);                
            }
        });
        Task task2 = Task.Run(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                Interlocked.Increment(ref _counter);                
            }
        });
        
        try
        {
            task1.Wait();
            task2.Wait();
        }
        catch (ThreadInterruptedException exception)
        {
            Console.WriteLine(exception.StackTrace);
        }
        
        Console.WriteLine($"Counter: {_counter}");
    }
}