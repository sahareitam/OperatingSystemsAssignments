using System;
using System.Collections.Concurrent;
using System.Threading;

public class Simulator
{
    private SharableSpreadsheet spreadsheet;
    private int nThreads;
    private int nOperations;
    private int msSleep;
    private Random random = new Random();
    private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
    private int remainingOperations;

    public Simulator(int rows, int cols, int nThreads, int nOperations, int msSleep)
    {
        // Create the spreadsheet with the correct number of rows, columns, and logging enabled.
        spreadsheet = new SharableSpreadsheet(rows, cols, nUser: -1, LogOperation);
        this.nThreads = nThreads;
        this.nOperations = nOperations;
        this.msSleep = msSleep;
        this.remainingOperations = nThreads * nOperations;

        InitializeSpreadsheet(rows, cols);
    }

    // Fill the spreadsheet with initial data (e.g., "cellXY" format).
    private void InitializeSpreadsheet(int rows, int cols)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                spreadsheet.SetCell(row, col, $"cell{row}{col}");
            }
        }
    }

    // Main function to run the simulation with the specified number of threads.
    public void Run()
    {
        Thread[] threads = new Thread[nThreads];

        for (int i = 0; i < nThreads; i++)
        {
            threads[i] = new Thread(UserThread);
            threads[i].Start(i);
        }

        foreach (var thread in threads)
        {
            thread.Join();  // Wait for all threads to finish their work.
        }

        spreadsheet.schedulerRun = false;  // Stop the scheduler after all operations are done.

        PrintLogs();  // Print all the logs at the end of the simulation.
    }

    // The function each thread (user) will run.
    private void UserThread(object obj)
    {
        int userId = (int)obj;

        for (int i = 0; i < nOperations; i++)
        {
            PerformRandomOperation(userId);  // Perform a random operation on the spreadsheet.
            Thread.Sleep(msSleep);  // Sleep for the specified amount of time.
        }
    }

    // Perform a random operation on the spreadsheet for the given user.
    private void PerformRandomOperation(int userId)
    {
        int row = random.Next(spreadsheet.nRows);
        int col = random.Next(spreadsheet.nCols);
        int operation = random.Next(13);  // Randomly choose between 13 different operations.

        switch (operation)
        {
            case 0:
                spreadsheet.EnqueueGetCell(row, col);
                break;
            case 1:
                spreadsheet.EnqueueSetCell(row, col, $"rand{row}{col}");
                break;
            case 2:
                spreadsheet.EnqueueExchangeRows(row, random.Next(spreadsheet.nRows));
                break;
            case 3:
                spreadsheet.EnqueueExchangeCols(col, random.Next(spreadsheet.nCols));
                break;
            case 4:
                spreadsheet.EnqueueSearchInRow(row, $"rand{row}{col}");
                break;
            case 5:
                spreadsheet.EnqueueSearchInCol(col, $"rand{row}{col}");
                break;
            case 6:
                spreadsheet.EnqueueSearchInRange(row, random.Next(spreadsheet.nRows), col, random.Next(spreadsheet.nCols), $"rand{row}{col}");
                break;
            case 7:
                spreadsheet.EnqueueAddRow(row);
                break;
            case 8:
                spreadsheet.EnqueueAddCol(col);
                break;
            case 9:
                spreadsheet.EnqueueFindAll($"rand{row}{col}");
                break;
            case 10:
                spreadsheet.EnqueueSetAll($"rand{row}{col}", $"new{row}{col}");
                break;
            case 11:
                spreadsheet.EnqueueGetSize();
                break;
            case 12:
                spreadsheet.EnqueueSearchString($"rand{row}{col}");
                break;
        }

        // Update the remaining operations and stop the scheduler when done.
        lock (this)
        {
            remainingOperations--;
            if (remainingOperations == 0)
            {
                spreadsheet.schedulerRun = false;  // Stop the scheduler once all operations are finished.
            }
        }
    }

    // Function to log the operations and add them to the log queue.
    private void LogOperation(string operation)
    {
        logQueue.Enqueue($"[{DateTime.Now:HH:mm:ss}] {operation}");
    }

    // Print the logged operations after all threads complete their work.
    private void PrintLogs()
    {
        while (logQueue.TryDequeue(out var logEntry))
        {
            Console.WriteLine(logEntry);
        }
    }
}

// Main entry point for the console application.
class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 5)
        {
            Console.WriteLine("Usage: Simulator <rows> <cols> <nThreads> <nOperations> <msSleep>");
            return;
        }

        // Parse the arguments from the command line.
        int rows = int.Parse(args[0]);
        int cols = int.Parse(args[1]);
        int nThreads = int.Parse(args[2]);
        int nOperations = int.Parse(args[3]);
        int msSleep = int.Parse(args[4]);

        // Create and run the simulator.
        var simulator = new Simulator(rows, cols, nThreads, nOperations, msSleep);
        simulator.Run();
    }
}
