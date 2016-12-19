using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algorithm_FF_maxFlow
{

    public enum Vertices
    {
        WHITE = 0,
        GREY = 1,
        BLACK = 2
    }

    public class Matrix
    {
        private int n, e;
        public int[,] capacity = new int[100, 100]; // Матрица пропускных способнотей
        public int[,] flow = new int[100,100];  // Матрица потока
        private int[] color = new int[100];      // Цвета для вершин
        private int[] pred = new int[100];       // Массив пути
        private int head, tail;  // Начало, Конец
        private int[] q = new int [102];      // Очередь, хранящая номера вершин входящих в неё

        //Сравнение двух целых значений
        private int Min(int x, int y)
        {
            return x < y ? x : y;
        }

        //Добавить в очередь(мы ступили на вершину)
        private void Enque(int x)
        {
            q[tail] = x;     // записать х в хвост
            tail++;          // хвостом становиться следующий элемент
            color[x] = (int)Vertices.GREY; // Цвет серый (из алгоритма поиска)
        }

        //Убрать из очереди(Вершина чёрная, на неё не ходить)
        private int Deque()
        {
            var x = q[head];  // Записать в х значение головы
            head++;           // Соответственно номер начала очереди увеличивается
            color[x] = (int)Vertices.BLACK; // Вершина х - отмечается как прочитанная
            return x;         // Возвращается номер х прочитанной вершины
        }

        //Поиск в ширину
        private int Bfs(int start, int end)
        {
            for (var u = 0; u < n; u++) // Сначала отмечаем все вершины не пройденными
                color[u] = (int)Vertices.WHITE;

            head = 0;   // Начало очереди 0
            tail = 0;   // Хвост 0
            Enque(start);      // Вступили на первую вершину
            pred[start] = -1;   // Специальная метка для начала пути
            while (head != tail)  // Пока хвост не совпадёт с головой
            {
                var u = Deque();       // вершина u пройдена
                for (var v = 0; v < n; v++) // Смотрим смежные вершины
                {
                    // Если не пройдена и не заполнена
                    if (color[v] == (int)Vertices.WHITE && (capacity[u,v] - flow[u,v]) > 0)
                    {
                        Enque(v);  // Вступаем на вершину v
                        pred[v] = u; // Путь обновляем
                    }
                }
            }
            if ( color[end] == (int)Vertices.BLACK ) // Если конечная вершина, дошли - возвращаем 0
                return 0;
            else
                return 1;
        }

        //Максимальный поток из истока в сток
        public int Max_flow(int source, int stock)
        {
            var maxflow = 0;            // Изначально нулевой
            for (var i = 0; i < n; i++)  // Зануляем матрицу потока
            {
                for (var j = 0; j < n; j++)
                    flow[i,j] = 0;
            }
            while (Bfs(source, stock) == 0)             // Пока сеществует путь
            {
                var delta = 10000;
                for (var u = n - 1; pred[u] >= 0; u = pred[u]) // Найти минимальный поток в сети
                {
                    delta = Min(delta, (capacity[pred[u],u] - flow[pred[u],u]));
                }
                for (var u = n - 1; pred[u] >= 0; u = pred[u]) // По алгоритму Форда-Фалкерсона 
                {
                    flow[pred[u],u] += delta;
                    flow[u,pred[u]] -= delta;
                }
                maxflow += delta;                       // Повышаем максимальный поток
            }
            return maxflow;
        }

    }

    class Program
    {
        
        static void Main(string[] args)
        {
            // Чтение из файла
            //freopen("data.txt", "r", stdin);  // аргумент 1 - путь к файлу, 2 - режим ("r" || "w"), 3 - stdin || stdout
            int n;
            int.TryParse(Console.ReadLine(), out n);
            if(n != 0)
                Console.WriteLine("Количество вершин = " + n);
            else
                Console.WriteLine("Значение 'n' не корректно");

            var matrix = new Matrix();

            int i, j;

            int[,] matrixCusial = {
                {0, 10, 5, 0, 0, 8, 0},
                {0,  0, 0, 5, 3, 0, 4},
                {0,  0, 0, 4, 5, 10,0},
                {0,  0, 0, 0, 4, 0, 9},
                {0,  0, 0, 0, 0, 5, 6},
                {0,  0, 0, 0, 0, 0, 7},
                {0,  0, 0, 0, 0, 0, 0}
            };

            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++)
                    matrix.capacity[i, j] = matrixCusial[i,j];
            }

            Console.WriteLine("Максимальный поток: " + matrix.Max_flow(0, n-1));
            Console.WriteLine("Матрица чего-то там: ");
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++)
                    Console.Write(matrix.flow[i,j] + " ");
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
