using System;

internal class Program
{
    private static void Main(string[] args)
    {
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
        CheckingValues(fieldSize,numMines);
        bool[,] field = CreateField(fieldSize, numMines);
        DisplayField(field);
        EndGame();
    }

    // перевірка введених значень
    private static void CheckingValues(int fieldSize, int numMines)
    {
        if(fieldSize <= 0 || numMines <= 0)
        {
            Console.Clear();
            Console.WriteLine("Invalid input. Please enter a positive number" + "\n");
            Console.ReadLine();
            Console.Clear();
            Main(new string[] { });
            return;
        }
            
        else if(numMines >= fieldSize * fieldSize)
        {
            Console.Clear();
            Console.WriteLine("Invalid input. The number of mines cannot be greater than the total number of cells on the field" + "\n");
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
        int rows = fieldSize;
        int cols = fieldSize;
        bool[,] field = new bool[rows, cols];
        Random rand = new Random();

        while (numMines > 0)
        {
            int row = rand.Next(0, rows);
            int col = rand.Next(0, cols);

            if (!field[row, col])
            {
                field[row, col] = true;
                numMines--;
            }
        }

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
                Console.Write(field[i, j] ? "*" + " " : "-" + " ");
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