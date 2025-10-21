namespace Software_Design._7._Synchronisation_methods;

public class Methods
{
    private static readonly Random Random = new();
    private static readonly Barrier Barrier = new(3);
    private static readonly Semaphore Pool = new(0, 3);
    private volatile int _version;

    // 1. Барьер хорошо подходит для многоступенчатых операций. Самый понятный вариант - 2 этапа.
    // Метод ниже демонстрирует модуль, который возвращает результаты квиза сначала для одного участника, а когда
    // результаты вычислены для всех, то отображаются общие результаты
    public static void RunBarrier()
    {
        Console.WriteLine("Опрос начался!");

        Task resultsRequest1 = Task.Run(() => DemonstrateResults("Первый участник"));
        Task resultsRequest2 = Task.Run(() => DemonstrateResults("Второй участник"));
        Task resultsRequest3 = Task.Run(() => DemonstrateResults("Третий участник"));

        Task.WaitAll(resultsRequest1, resultsRequest2, resultsRequest3);

        Console.WriteLine("Опрос окончен");
    }

    private static void DemonstrateResults(string username)
    {
        Console.WriteLine($"{username} завершил опрос и ждёт результатов");
        Thread.Sleep(Random.Next(1000, 3000));
        Console.WriteLine($"{username}: {Random.Next(10)}");

        Barrier.SignalAndWait();

        Console.WriteLine($"Вывод результатов всех участников для {username}");
    }

    // 2. Семафор можно использовать при ограниченных ресурсах при запросе на бэкенде
    // Ситуация: приватное апи не может обрабатывать больше 3 запроса сразу и возвращает ошибку. При ограниченном числе
    // пользователей приемлемым решением будет блокировка метода, который обращается к такому АПИ
    // Сперва 3 запроса сразу забивают доступный пул, после чего по одному освобождают места для следующих запросов
    public static void Semaphore()
    {
        for (int i = 0; i < 5; ++i)
        {
            Thread t = new Thread(CallLocalBusyApi);
            t.Start(i);
        }

        Pool.Release(3);
    }

    private static void CallLocalBusyApi(object? requestIndex)
    {
        Pool.WaitOne();
        Console.WriteLine($"Запрос {requestIndex} пытается достучаться до апи");
        Thread.Sleep(Random.Next(1000, 3000));
        Console.WriteLine($"Запрос {requestIndex} дошёл до апи!");
        Pool.Release();
    }

    // 3. Если пользователь хочет иметь возможность отменять какие-то тяжёлые запросы, то можно в API предусмотреть
    // два запроса: один на запуск долгого процесса, а другой - на отмену. Процессом будет условный анализ данных
    // При завершении процесса его нужно обработать и выдать какой-то результат. 
    public static async Task Future()
    {
        var source = new CancellationTokenSource();

        Console.WriteLine("Первый запрос не будет отменяться");
        var firstTask = StartProcess(CancellationToken.None); // без токена
        Console.WriteLine(await firstTask);

        source.CancelAfter(20000);
        Console.WriteLine("Второй запрос отменится спустя 20 сек");
        var secondTask = StartProcess(source.Token);
        Console.WriteLine(await secondTask);
    }

    private static async Task<string> StartProcess(CancellationToken token)
    {
        await Task.Delay(30000, token); // передаём токен в Delay
        return "Success";
    }

    // 4. Фабрику web-crawler-ов можно держать в следующей архитектуре:
    // При пользовательском запросе запускается соответствующий crawler
    // На бэкенде запускается джоба, собирающая ресурсы для начала работы и затем поэтапно выполняющая определённые действия
    // При смене статусов джобы клиенту, запустившемуся этого crawler-а приходит уведомление, где далее выполняется 
    // какая-то логика, в зависимости от статуса
    public static void Event()
    {
        var farm = new CrawlerFarm();

        farm.CrawlerStatusChanged += FirstCrawlerUser;
        farm.CrawlerStatusChanged += SecondCrawlerUser;

        farm.Status = "Started";
        farm.Status = "Finished";
    }

    private class CrawlerFarm
    {
        public delegate void CrawlerStatusChangedHandler(object sender, CrawlerEventArgs e);

        // Event for notifying stock price changes
        public event CrawlerStatusChangedHandler CrawlerStatusChanged;

        private string status;

        public string Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    OnCrawlerStatusChanged(new CrawlerEventArgs(value));
                }
            }
        }

        private void OnCrawlerStatusChanged(CrawlerEventArgs e)
        {
            CrawlerStatusChanged.Invoke(this, e);
        }
    }

    private class CrawlerEventArgs : EventArgs
    {
        public string Status { get; }

        public CrawlerEventArgs(string status)
        {
            Status = status;
        }
    }

    private static void FirstCrawlerUser(object sender, CrawlerEventArgs args)
    {
        Console.WriteLine($"Статус кролера первого пользователя сменился на {args.Status}");
    }

    private static void SecondCrawlerUser(object sender, CrawlerEventArgs args)
    {
        Console.WriteLine($"Статус кролера второго пользователя сменился на {args.Status}");
    }

    // 5. Атомарные операции можно использовать в обращении с SQLite, потому что у самой СУБД нет такого механизма
    // Актуальной такая задача становится, например, в случае, если несколько процессов имеют в одной базе общий ресурс
    // (например, чёрный список), который они читают и обновляют. Для того, чтобы в чёрный список не попадало дубликатов
    // (что решается на уровне СУБД) или новые ресурсы не противоречили чёрному списку (что не решается на уровне этой
    // СУБД), можно не делать локи, а воспользоваться атомиками
    public static void Atomics()
    {
        var blacklist = new BlackList();
        long readOps = 0;
        long blockedCount = 0;

        // 10 читателей - проверяют userId (имитация входящих запросов)
        var readers = Parallel.For(0, 10, _ =>
        {
            for (int i = 0; i < 100_000; i++)
            {
                int userId = Random.Shared.Next(0, 200);
                if (blacklist.Contains(userId))
                    Interlocked.Increment(ref blockedCount);
                Interlocked.Increment(ref readOps);
            }
        });

        // 3 писателя - добавляют в чёрный список (имитация модераторов)
        var writers = Parallel.For(0, 3, _ =>
        {
            for (int i = 0; i < 100; i++)
            {
                blacklist.Add(Random.Shared.Next(0, 200));
                Thread.Sleep(10);
            }
        });

        Console.WriteLine($"Операций чтения: {readOps:N0}");
        Console.WriteLine($"Заблокировано: {blockedCount:N0}");
        Console.WriteLine($"Размер ЧС: {blacklist.Count}");
    }

    public class BlackList
    {
        private volatile int[] _blacklist = [];
        private int _count = 0;
        private int _writerLock = 0;
        // некоторая логика подключения к бд            

        public bool Contains(int key)
        {
            var snapshot = _blacklist;
            int count = Volatile.Read(ref _count);

            for (int i = 0; i < count; i++)
            {
                if (snapshot[i] == key)
                    return true;
            }

            return false;
        }

        // Добавление со spinlock
        public void Add(int key)
        {
            if (Contains(key)) return;

            while (Interlocked.CompareExchange(ref _writerLock, 1, 0) != 0)
                Thread.SpinWait(20);

            try
            {
                if (Contains(key)) return;

                var newArray = new int[_count + 1];
                Array.Copy(_blacklist, newArray, _count);
                newArray[_count] = key;

                Interlocked.Increment(ref _count);
                _blacklist = newArray;
            }
            finally
            {
                Volatile.Write(ref _writerLock, 0);
            }
        }
        
        public int Count => Volatile.Read(ref _count);
    }
}