namespace Software_Design;

/** Корректность алгоритма быстрой сортировки
 * Условия для основного вызова:
 * {P: arr.length > 0; low < arr.length; high < arr.length; low < high} quickSort(arr, low, high) {Q: ∀(xi+1, xi) ∈ arr(xi+1 ≥ xi)}
 * (для любых последовательных х в массиве arr с индексами i и i+1 хi+1 должен быть больше или равен, чем x i)
 */
public class Quick_Sort_Hoare
{
    /** Корректность функции разделения массива
     * {P: arr.length > 0; low < arr.length; high < arr.length; low < high} partition(arr, low, high)
     * {Q: pi = i + 1, ∀(x) ∈ arr[0, i](arr[x] < arr[pi]}i; ∀(x) ∈ arr[low..high](x < x[pi])}
     * На выходе этой фукнции мы получаем массив, где все элементы в промежутке i до pi меньше arr[pi], а pi
     */
    static int partition(int[] arr, int low, int high)
    {
        // choose the pivot
        int pivot = arr[high];

        // index of smaller element and indicates 
        // the right position of pivot found so far
        int i = low - 1;

        // traverse arr[low..high] and move all smaller
        // elements to the left side. Elements from low to 
        // i are smaller after every iteration
        for (int j = low; j <= high - 1; j++)
        {
            if (arr[j] < pivot)
            {
                i++;
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }

        // move pivot after smaller elements and
        // return its position
        (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
        return i + 1;
    }

    /** Инвариант цикла:
     * I: ∀(x) ∈ arr[0, pi](arr[pi] ≥ x), ∀(y) arr[pi, n - 1](arr[pi] < y) 
     * На каждой итерации цикла quickSort выбирается индекс pi в массиве arr так, что все элементы слева от него меньше
     * или равны arr[pi], а справа - больше, где n - длина массива
     * 
     * Доказательство.
     * Инициализация:
     * На первой итерации мы получаем элемент pi из функции partition, где low = 0, а high = n - 1
     * low и high соответствуют предусловию.
     * По постусловию pi равняется концу массива
     * Также по постусловию partition в массиве все элементы слева от pi меньше элемента с индексом pi
     * Поскольку справа от pi нет других элементов,условие того, что справа от pi нет элементов больше, чем arr[pi], выполняется 
     * Выходит, на первой итерации постусловие partition подтвердило правильность инварианта цикла
     *
     * Процесс:
     * Допустим, что для какого-то pi выполняется условие, что все элементы массива, находящиеся слева от него, - меньше,
     * а справа - больше
     * На каждом шаге вызывается две проверки: слева и справа от pi
     * Первая проверка - слева. По постусловию функции partition все элементы от 0 до i меньше чем pi
     * Вторая проверка. В первой проверке мы убедились, что все элементы от 0 до pi - 1 меньше, чем arr[pi].
     * Поскольку массив был отсортирован так, что слева от pi оказались все элементы, меньше чем arr[pi] выходит, что
     * справа оказались только те элементы, которые больше или равны arr[pi], что подтверждает вторую часть инварианта
     *
     * Завершение:
     * Цикл завершается, когда индексы low и high в обеих половинах массива встретятся.
     * По постусловию partition, который, как доказано выше, рекурсивно разбил каждую половину на другие половины,
     * где был найден такой индекс pi, чтобы слева были элементы слева от индекса pi, а справа - элементы, больше или
     * равные элементу по индексу pi
     * В последнем цикле индекс low больше или равен, чем high, что означает, что каждый элемент первого массива
     * (до первого pi) меньше или равен следующему. То же верно и для второго массива (от первого pi до конца массива)
     * По постусловию первого цикла, все элементы второго массива больше элементов первого массива
     * В итоге получаем отсортированный масси
     */
    static void quickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            // pi is the partition return index of pivot
            int pi = partition(arr, low, high);

            // recursion calls for smaller elements
            // and greater or equals elements
            quickSort(arr, low, pi - 1);
            quickSort(arr, pi + 1, high);
        }
    }

    static void Main(string[] args)
    {
        int[] arr = { 10, 7, 8, 9, 1, 5 };
        int n = arr.Length;

        quickSort(arr, 0, n - 1);
        foreach (int val in arr)
        {
            Console.Write(val + " ");
        }
    }
}