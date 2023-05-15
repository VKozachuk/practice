using System;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Clear();

        // розмір поля
        Console.Write("Enter field size: ");
        int fieldSize;
        while (!int.TryParse(Console.ReadLine(), out fieldSize))
        {
            Console.Clear();
            Console.WriteLine("Invalid input. Please enter a positive integer.");
            Console.Write("Enter field size: ");
        }

        // кількість мін на полі
        Console.Write("Enter the number of mines: ");
        int numMines;
        while (!int.TryParse(Console.ReadLine(), out numMines))
        {
            Console.Clear();
            Console.WriteLine("Invalid input. Please enter a positive integer.");
            Console.Write("Enter the number of mines: ");
        }

        // виклик функції для перевірки значень, створення - відображення ігрового поля, а також виходу з гри або рестарт
        CheckingValues(fieldSize, numMines);
        bool[,] field = CreateField(fieldSize, numMines);
        DisplayField(field);
        EndGame();
    }

    // перевірка введених значень
    private static void CheckingValues(int fieldSize, int numMines)
    {
        if (fieldSize <= 0 || numMines <= 0)
        {
            Console.Clear();
            Console.WriteLine("Invalid input. Please enter a positive number." + "\n");
            Console.ReadLine();
            Console.Clear();
            Main(new string[] { });
            return;
        }

        else if (numMines >= fieldSize * fieldSize)
        {
            Console.Clear();
            Console.WriteLine("Invalid input. The number of mines cannot be greater than the total number of cells on the field.");
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
            Console.Clear();
            Main(new string[] { });
            return;
        }

        Console.Clear();
    }

    // створення поля і заповнення поля мінами
    private static bool[,] CreateField(int fieldSize, int numMines)
    {
        int iterations = 0;
        int rows = fieldSize;
        int cols = fieldSize;
        bool[,] field = new bool[rows, cols];
        Random rand = new Random();

        List<(int, int)> positions = new List<(int, int)>();

        // додавання усіх позицій у список
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                positions.Add((i, j));
            }
        }

        // розставлення мін на полі
        while (numMines > 0 && positions.Count > 0)
        {
            int index = rand.Next(positions.Count);
            int row = positions[index].Item1;
            int col = positions[index].Item2;
            iterations++;

            field[row, col] = true;
            positions.RemoveAt(index);
            numMines--;
        }

        Console.WriteLine("The number of iterations = " + iterations);

        return field;
    }



    // відображення поля
    private static void DisplayField(bool[,] field)
    {
        int rows = field.GetLength(0);
        int cols = field.GetLength(1);

        Console.Write("   ");
        for (int j = 0; j < cols; j++)
        {
            Console.Write(j + " ");
        }
        Console.WriteLine();

        Console.Write("  ");
        for (int j = 0; j < cols; j++)
        {
            Console.Write("--");
        }
        Console.WriteLine();

        for (int i = 0; i < rows; i++)
        {
            Console.Write(i + "| ");
            for (int j = 0; j < cols; j++)
            {
                Console.Write(field[i, j] ? "X" + " " : "-" + " ");
            }
            Console.WriteLine("|");
        }

        Console.Write("  ");
        for (int j = 0; j < cols; j++)
        {
            Console.Write("--");
        }
        Console.WriteLine();
    }

    // вихід з гри або рестарт
    private static void EndGame()
    {
        while (true)
        {
            Console.Write("Press q to exit or r to restart: ");
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.KeyChar == 'q')
            {
                Console.Clear();
                return;
            }

            else if (keyInfo.KeyChar == 'r')
            {
                Console.Clear();
                Main(new string[] { });
                return;
            }

            else
            {
                Console.Clear();
                Console.WriteLine("Invalid input. Please enter q or r.");
            }
        }
    }
}