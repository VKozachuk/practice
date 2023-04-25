using System;

internal class Program
{
    private static void Main(string[] args)
    {
        // розмір поля
        Console.Write("Enter field size: ");
        int fieldSize = Convert.ToInt32(Console.ReadLine());

        // кількість мін на полі
        Console.Write("Enter the number of mines: ");
        int numMines = Convert.ToInt32(Console.ReadLine());

        // створення і заповнення поля мінами
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

        // відображення поля
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
                Console.Write(field[i, j] ? "* " : "-" + " ");
            }
            Console.WriteLine("|");
        }

        Console.Write("  ");
        for (int j = 0; j < cols; j++)
        {
            Console.Write("--");
        }
        Console.WriteLine();

        Console.WriteLine("Press any key to exit");
        Console.ReadLine();
    }
}