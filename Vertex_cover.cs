using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Vertex_cover
{
    class Graph // ГРАФ
    {
        public int N;                   // Длина графа
        public List<List<int>> graph;   // Матрица смежности графа

        public Graph()
        {
            // Выбор способа ввода
            Console.WriteLine("Выберете способ ввода:");
            Console.WriteLine("0.С клавиатуры\n1.Из файла\n2.Автоматически");
            int Ask = int.Parse(Console.ReadLine());
            switch (Ask)
            {
                case 0:
                    Print();
                    break;
                case 1:
                    File();
                    break;
                case 2:
                    Rand();
                    break;
            }
        }

        void Print() // Ввод данных вручную
        {
            // Ввод количества вершин
            Console.WriteLine("Введите количество вершин графа:");
            N = int.Parse(Console.ReadLine()); 

            // Инициализация и ввод графа
            graph = new List<List<int>>(N); 
            for (int i = 0; i < N; i++)
            {
                graph.Add(new List<int>(N));
                Console.WriteLine("Введите " + i + " строку графа");
                for (int j = 0; j < N; j++)
                    graph[i].Add(int.Parse(Console.ReadLine()));
            }
        }

        void Rand() // Ввод данных с помощью генератора 
        {
            Random rnd = new Random();

            Console.WriteLine("Выберете способ ввода длины массива:");
            Console.WriteLine("0.С клавиатуры\n1.Автоматически");

            // Ввод количества вершин
            if (int.Parse(Console.ReadLine()) == 0)
                N = int.Parse(Console.ReadLine());
            else
                N = rnd.Next() % 4 + 2;

            // Инициализация и заполнение графа
            graph = new List<List<int>>(N);
            for (int i = 0; i < N; i++)
            {
                graph.Add(new List<int>(N));
                for (int j = 0; j < i; j++)
                    graph[i].Add(rnd.Next() % 2);
                graph[i].Add(0);
            }
            for(int i = 0; i < N; i++)
                for (int j = i + 1; j < N; j++)
                    graph[i].Add(graph[j][i]);
                
        }

        void File() // Ввод данных через файл
        {
            String line;
            try
            {
                // Ввод количества вершин
                StreamReader sr = new StreamReader("Graph.txt");
                N = int.Parse(sr.ReadLine());

                // Инициализация и заполнение графа
                graph = new List<List<int>>(N);
                for (int i = 0; i < N; i++)
                {
                    line = sr.ReadLine();
                    String[] buf = line.Split(' ');
                    graph.Add(new List<int>(N));

                    for (int j = 0; j < N; j++)
                        graph[i].Add(int.Parse(buf[j]));
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public void Save() // Сохраняет исходный список в файл
        {
            try
            {
                // Запись количества вершин
                StreamWriter sw = new StreamWriter("Graph1.txt");
                sw.WriteLine(N);

                // Запись графа
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                        sw.Write(graph[i][j]+" ");
                    sw.WriteLine();
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public void Show() // Вывод матрицы смежности графа на экран
        {
            for(int i = 0; i < N; i++)
            {
                for(int j = 0; j < N; j++)
                    Console.Write(graph[i][j]+ " ");
            Console.WriteLine();
            }
        }
    }

    class Enumerative // ТОЧНЫЙ ПЕРЕБОРНЫЙ
    {
        int N;                                  // Количество вершин в графе
        public List<List<int>> G;               // Граф
        List<int> Cover;                        // Вершинное покрытие
        TimeSpan ts;                            // Время выполнения

        public Enumerative(Graph G)
        {
            this.G = G.graph;
            N = G.N;

            // Считаем время выполнения
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Ищем вершинное покрытие
            Search();

            // Записываем время выполнения
            stopWatch.Stop();
            ts = stopWatch.Elapsed; 
        }

        bool NextSet(List<int> a, int n, int m) // Перебор всех сочетаний из n по m без повторов
        {

            for (int i = m - 1; i >= 0; --i)
                if (a[i] < n - m + i + 1)
                {
                    ++a[i];
                    for (int j = i + 1; j < m; ++j)
                        a[j] = a[j - 1] + 1;

                    return true;
                }
            return false;
        }

        List<int> Search() // Поиск наименьшего вершинного покрытия перебором
        {
            Cover = new List<int>(0);

            // Проверка на наличие вершин в графе
            int CountG = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (G[i][j] == 1)
                        CountG++;
            if (CountG != 0)
            {

                // Проверяем все объем вершинного покрытия от 1 до N вершин
                for (int LengthCov = 1; LengthCov <= N; LengthCov++)
                {
                    // Создаем массив для вершинного покрытия
                    Cover = new List<int>(LengthCov);
                    for (int i = 0; i < LengthCov; i++)
                        Cover.Add(i + 1);


                    // Перебор всех возможных сочетаний вершин объёмом LengthCov
                    while (NextSet(Cover, N, LengthCov))
                        // Находим первое, являющиеся вершинным покрытием
                        if (Prov(Cover, LengthCov))
                            return Cover;
                }
            }

            // Если вершинного покрытия нет, возвращаем список из 0 элементов
            return Cover; 
        }

        bool Prov(List<int> Cover, int n) // Проверка является ли набор вершинным покрытием
        {
            // Создаем копию графа для проверки
            List<List<int>> GCopy = new List<List<int>>(N);
            for (int i = 0; i < N; i++)
            {
                GCopy.Add( new List<int>(N));
                for (int j = 0; j < N; j++)
                    GCopy[i].Add(G[i][j]);
            }

            // Удаляем из графа все ребра, инцидентные вершинам, входящим в покрытие
            for (int i = 0; i < n; i++)
                for (int j = 0; j < N; j++)
                    if (GCopy[j][Cover[i] - 1] == 1)
                    {
                        GCopy[j][Cover[i] - 1] = 0;
                        GCopy[Cover[i] - 1][j] = 0;
                    }

            // Если в графе остались ребра, это не вершинное покрытие
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (GCopy[i][j] == 1)
                        return false;

            return true;
        }

        public void Show() // Вывод результата
        {
            Console.WriteLine("Вершинное покрытие графа состоит из вершин: ");
            Cover.ForEach(v => Console.Write(v + " "));
            Console.WriteLine();
        }

        public void WatchTime() // Вывод времени выполнения алгоритма
        {
            Console.WriteLine(" Время выполнения алгоритма:");
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }

    class Application // ПРИБЛИЖЕННЫЙ АЛГОРИТМ
    {
        int N;                                  // Количество вершин в графе
        public List<List<int>> G;               // Граф
        List<int> Cover;                        // Вершинное покрытие
        TimeSpan ts;                            // Время выполнения

        public Application(Graph G)
        {
            N = G.N;
            Cover = new List<int>();

            // Создаем копию графа
            this.G = new List<List<int>>(N);
            for (int i = 0; i < N; i++)
            {
                this.G.Add(new List<int>(N));
                for (int j = 0; j < N; j++)
                    this.G[i].Add(G.graph[i][j]);
            }

            // Считаем время выполнения
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Ищем вершинное покрытие
            Search();

            // Записываем время выполнения
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
        }

        void Search() // алгоритм
        {
            for (int i = 0; i < N; i++)
                for (int j = 0; j < i; j++)
                    // для первого найденного ребра
                    if (G[i][j] == 1) 
                    {

                        // добавляем инцидентные вершины в покрытие
                        Cover.Add(i + 1);
                        Cover.Add(j + 1);


                        // удаляем все инцидентные этим вершинам ребра
                        for (int l = 0; l < N; l++)
                        {
                            if (G[i][l] == 1)
                            {
                                G[i][l] = 0;
                                G[l][i] = 0;
                            }
                        }

                        for (int l = 0; l < N; l++)
                            if (G[j][l] == 1)
                            {
                                G[j][l] = 0;
                                G[l][j] = 0;
                            }
                    }

        }

        public void Show() // Вывод результата
        {
            Console.WriteLine("Вершинное покрытие графа состоит из вершин: ");
            Cover.ForEach(v => Console.Write(v+" "));
            Console.WriteLine();
        }

        public void WatchTime() // Вывод времени выполнения алгоритма
        {
            Console.WriteLine(" Время выполнения алгоритма:");
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }

    class Greedy // ЖАДНЫЙ АЛГОРИТМ
    {
        int N;                                  // Количество вершин в графе
        public List<List<int>> G;               // Граф
        List<int> Cover;                        // Вершинное покрытие
        TimeSpan ts;                            // Время выполнения

        public Greedy(Graph G)
        {
            Cover = new List<int>();
            N = G.N;

            // Создаем копию графа
            this.G = new List<List<int>>(N);
            for (int i = 0; i < N; i++)
            {
                this.G.Add(new List<int>(N));
                for (int j = 0; j < N; j++)
                    this.G[i].Add(G.graph[i][j]);
            }

            // Считаем время выполнения
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Ищем вершинное покрытие
            Search();

            // Записываем время выполнения
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
        }

        void Search() // алгоритм
        {
            bool U;     // Наличие в графе непокрытых ребер
            do
            {
                U = false;
                int MaxCount = 0;       // максимальное количество ребер инцидентных одной вершины
                int MaxVal = 0;         // вершина, инцидентная максимальному количеству ребер

                // Для каждой вершины
                for (int i = 0; i < N; i++)
                {
                    int Count = 0;      // счетчик инцидентных вершине ребер 

                    // Считаем количество ребер у каждой вершины
                    for (int j = 0; j < N; j++)
                    {
                        if (G[i][j] == 1)
                            Count++;
                    }

                    // Находим вершину с намбольшим количеством ребер
                    if (MaxCount < Count)
                    {
                        MaxCount = Count;
                        MaxVal = i;
                    }
                }

                // сохраняем вершину с максимальным количеством смежных ребер в вершинное покрытие
                if (MaxCount != 0)
                    Cover.Add(MaxVal + 1);

                // удаляем смежные с этой вершиной ребра
                for (int i = 0; i < N; i++)
                    if (G[i][MaxVal] == 1)
                    {
                        G[i][MaxVal] = 0;
                        G[MaxVal][i] = 0;
                        U = true;           // считаем, что в графе еще есть непокрытые ребра
                    }
            } while (U);
        }

        public void Show() // Вывод результата
        {
            Console.WriteLine("Вершинное покрытие графа состоит из вершин: ");
            Cover.ForEach(v => Console.Write(v + " "));
            Console.WriteLine();
        }

        public void WatchTime() // Вывод времени выполнения алгоритма
        {
            Console.WriteLine(" Время выполнения алгоритма:");
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            do
            {
                // Создание графа
                Graph G = new Graph();

                // Вывод графа на экран
                G.Show();

                // Выбор алгоритма выполнения
                Console.WriteLine("Выберете алгоритм:");
                Console.WriteLine("0.Точный переборный\n1.Приближенный\n2.Жадный");

                int Ask = int.Parse(Console.ReadLine());
                switch (Ask)
                {
                    case 0:
                        Enumerative Enum = new Enumerative(G);
                        Enum.Show();
                        //Enum.WatchTime();
                        break;

                    case 1:
                        Application Applic = new Application(G);
                        Applic.Show();
                        //Applic.WatchTime();
                        break;

                    case 2:
                        Greedy Greed = new Greedy(G);
                        Greed.Show();
                        //Greed.WatchTime();
                        break;
                }

                //Console.WriteLine("Введите 1, если хотите сохранить список.");
                //if (int.TryParse(Console.ReadLine(), out int save)&&(save == 1))
                //    G.Save();

            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }
    }
}
