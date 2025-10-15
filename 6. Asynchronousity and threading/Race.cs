namespace Software_Design._6._Asynchronousity_and_threading;

//public class RaceConditionExample {
// 
//     private static int counter = 0;
// 
//     public static void main(String[] args) {
//         int numberOfThreads = 10;
//         Thread[] threads = new Thread[numberOfThreads];
// 
//         for (int i = 0; i < numberOfThreads; i++) {
//             threads[i] = new Thread(() -> {
//                 for (int j = 0; j < 100000; j++) {
//                     counter++;
//                 }
//             });
//             threads[i].start();
//         }
// 
//         for (int i = 0; i < numberOfThreads; i++) {
//             try {
//                 threads[i].join();
//             } catch (InterruptedException e) {
//                 e.printStackTrace();
//             }
//         }
// 
//         System.out.println("Final counter value: " + counter);
//     }
// }
// 

// Потоки в состоянии гонки обращаются к куче за переменной при вычислении, причём может произойти ситуация, когда
// много потока одновременно перезаписали значение переменной на 1 больше (например, с 9 до 10), и по итогу выполнения,
// скажем, 5 потоков переменная увеличится не на 5, а на 1, так как потоки не поделили переменную.  

// правильный вариант
public class Race
{
    private static int _counter = 0;
    private static readonly ReaderWriterLock CounterLock = new();

    public static void Run()
    {
        int numberOfThreads = 10;
        Task[] tasks = new Task[numberOfThreads];
        for (int i = 0; i < numberOfThreads; i++)
        {
            tasks[i] = new Task(() =>
            {
                for (int j = 0; j < 100000; j++)
                {
                    lock (CounterLock)
                    {
                        _counter++;
                    }
                }
            });
            tasks[i].Start();
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            try
            {
                tasks[i].Wait();
            }
            catch (ThreadInterruptedException error)
            {
                Console.WriteLine("Task was interrupted");
            }
        }

        Console.WriteLine($"Final value of counter: {_counter}");
    }
}