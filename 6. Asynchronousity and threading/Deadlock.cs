namespace Software_Design._6._Asynchronousity_and_threading;

//public class DeadlockExample {
// 
//     private static final Object lock1 = new Object();
//     private static final Object lock2 = new Object();
// 
//     public static void main(String[] args) {
//         Thread thread1 = new Thread(() -> {
//             synchronized (lock1) {
//                 System.out.println("Thread 1 acquired lock1");
// 
//                 try { Thread.sleep(50); } 
//                 catch (InterruptedException e) { e.printStackTrace(); }
// 
//                 synchronized (lock2) {
//                     System.out.println("Thread 1 acquired lock2");
//                 }
//             }
//         });
// 
//         Thread thread2 = new Thread(() -> {
//             synchronized (lock2) {
//                 System.out.println("Thread 2 acquired lock2");
// 
//                 try { Thread.sleep(50); } 
//                 catch (InterruptedException e) { e.printStackTrace(); }
// 
//                 synchronized (lock1) {
//                     System.out.println("Thread 2 acquired lock1");
//                 }
//             }
//         });
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
//         System.out.println("Finished");
//     }
// }
// 

// Если разбирать код пошагово, то вторая таска захватила сразу второй лок, после чего первый таск - первый лок
// В результате второй таск ждёт первого лока, а первый - второго
// Решить такую ситуацию можно, вынеся взаимозависимые локи друг из друга, и исполняя их последовательно 

public class Deadlock
{
    public static readonly ReaderWriterLock lock1 = new();
    public static readonly ReaderWriterLock lock2 = new();

    public static void Run()
    {
        Task task1 = new Task(() =>
        {
            lock (lock1)
            {
                Console.WriteLine("Task 1 acquired first lock");

                try
                {
                    Task.Delay(50);
                }
                catch (ThreadInterruptedException error)
                {
                    Console.WriteLine(error.StackTrace);
                }
            }
            lock (lock2)
            {
                Console.WriteLine("Task 2 acquired second lock");
            }
        });

        Task task2 = new Task(() =>
        {
            lock (lock2)
            {
                Console.WriteLine("Task 2 acquired second lock");

                Task.Delay(50);
            }
            lock (lock1)
            {
                Console.WriteLine("Task 2 acquired first lock");
            }
        });
        
        task1.Start();
        task2.Start();

        try
        {
            task1.Wait();
            task2.Wait();
        }
        catch (ThreadInterruptedException error)
        {
            Console.WriteLine(error.StackTrace);
        }
        
        Console.WriteLine("Finished");
    }
}