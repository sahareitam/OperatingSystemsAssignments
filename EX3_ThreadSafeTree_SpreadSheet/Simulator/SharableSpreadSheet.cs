using System.Collections.Concurrent;
public class SharableSpreadsheet
{
    public int nRows;
    public int nCols;
    public bool schedulerRun;
    private string[,] spreadsheet;

    private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
    private SemaphoreSlim userSemaphore;
    private Action<string> logOperation;

    public ConcurrentQueue<Action> readQueue = new ConcurrentQueue<Action>();
    public ConcurrentQueue<Action> writeQueue = new ConcurrentQueue<Action>();

    public SharableSpreadsheet(int nRows, int nCols, int nUser = -1, Action<string> logOperation = null)
    {
        this.nRows = nRows;
        this.nCols = nCols;
        this.schedulerRun = true;
        this.spreadsheet = new string[nRows, nCols];
        this.logOperation = logOperation;

        if (nUser > 0)
            this.userSemaphore = new SemaphoreSlim(nUser);
        else
            this.userSemaphore = new SemaphoreSlim(int.MaxValue);

        // Start the scheduler thread
        Thread schedulerThread = new Thread(Scheduler);
        schedulerThread.IsBackground = true;
        schedulerThread.Start();
    }

    public void EnqueueGetCell(int row, int col)
    {
        EnqueueRead(() =>
        {
            var value = GetCell(row, col);
            logOperation?.Invoke($"Read '{value}' from cell ({row},{col})");
        });
    }

    public void EnqueueSetCell(int row, int col, string str)
    {
        EnqueueWrite(() =>
        {
            SetCell(row, col, str);
            logOperation?.Invoke($"Set '{str}' in cell ({row},{col})");
        });
    }

    public void EnqueueExchangeRows(int row1, int row2)
    {
        EnqueueWrite(() =>
        {
            ExchangeRows(row1, row2);
            logOperation?.Invoke($"Exchanged rows {row1} and {row2}");
        });
    }

    public void EnqueueExchangeCols(int col1, int col2)
    {
        EnqueueWrite(() =>
        {
            ExchangeCols(col1, col2);
            logOperation?.Invoke($"Exchanged columns {col1} and {col2}");
        });
    }

    public void EnqueueSearchInRow(int row, string str)
    {
        EnqueueRead(() =>
        {
            int foundCol = SearchInRow(row, str);
            if (foundCol != -1)
                logOperation?.Invoke($"Searched '{str}' in row {row} and found at column {foundCol}");
            else
                logOperation?.Invoke($"Searched '{str}' in row {row} and not found");
        });
    }

    public void EnqueueSearchInCol(int col, string str)
    {
        EnqueueRead(() =>
        {
            int foundRow = SearchInCol(col, str);
            if (foundRow != -1)
                logOperation?.Invoke($"Searched '{str}' in column {col} and found at row {foundRow}");
            else
                logOperation?.Invoke($"Searched '{str}' in column {col} and not found");
        });
    }

    public void EnqueueSearchInRange(int row1, int row2, int col1, int col2, string str)
    {
        EnqueueRead(() =>
        {
            var position = SearchInRange(Math.Min(row1, row2), Math.Max(row1, row2), Math.Min(col1, col2), Math.Max(col1, col2), str);
            logOperation?.Invoke($"Searched '{str}' in range ({Math.Min(row1, row2)},{Math.Min(col1, col2)}) to ({Math.Max(row1, row2)},{Math.Max(col1, col2)}) and found at {position}");
        });
    }

    public void EnqueueAddRow(int row)
    {
        EnqueueWrite(() =>
        {
            AddRow(row);
            logOperation?.Invoke($"Added row after {row}");
        });
    }

    public void EnqueueAddCol(int col)
    {
        EnqueueWrite(() =>
        {
            AddCol(col);
            logOperation?.Invoke($"Added column after {col}");
        });
    }

    public void EnqueueFindAll(string str)
    {
        EnqueueRead(() =>
        {
            var positions = FindAll(str, true);
            string positionsStr = string.Join(", ", positions.Select(pos => $"({pos.Item1}, {pos.Item2})"));
            logOperation?.Invoke($"Found all occurrences of '{str}' at positions {positionsStr}");
        });
    }

    public void EnqueueSetAll(string oldStr, string newStr)
    {
        EnqueueWrite(() =>
        {
            SetAll(oldStr, newStr, true);
            logOperation?.Invoke($"Replaced all occurrences of '{oldStr}' with '{newStr}'");
        });
    }

    public void EnqueueGetSize()
    {
        EnqueueRead(() =>
        {
            var size = GetSize();
            logOperation?.Invoke($"Size of spreadsheet is {size.Item1} rows and {size.Item2} columns");
        });
    }

    public void EnqueuePrint()
    {
        EnqueueRead(() =>
        {
            Print();
            logOperation?.Invoke($"Printed the entire spreadsheet");
        });
    }

    public void EnqueueSearchString(string str)
    {
        EnqueueRead(() =>
        {
            var position = SearchString(str);
            logOperation?.Invoke($"Searched '{str}' and found at {position}");
        });
    }

    private void EnqueueRead(Action operation) => readQueue.Enqueue(operation);
    private void EnqueueWrite(Action operation) => writeQueue.Enqueue(operation);

    public void Scheduler()
    {
        const int readOps = 512;
        const int writeOps = 192;

        while (schedulerRun)
        {
            ProcessQueue(readQueue, readOps, rwLock.EnterReadLock, rwLock.ExitReadLock);
            ProcessQueue(writeQueue, writeOps, rwLock.EnterWriteLock, rwLock.ExitWriteLock);
        }
    }

    private void ProcessQueue(ConcurrentQueue<Action> queue, int operationsPerQueue, Action enterLock, Action exitLock)
    {
        for (int i = 0; i < operationsPerQueue; i++)
        {
            if (queue.TryDequeue(out var operation))
            {
                userSemaphore.Wait();
                enterLock();
                try
                {
                    operation();
                }
                finally
                {
                    exitLock();
                    userSemaphore.Release();
                }
            }
            else
            {
                break;
            }
        }
    }

    // Methods to demonstrate read/write operations

    public string GetCell(int row, int col)
    {
        return spreadsheet[row, col];
    }

    public void SetCell(int row, int col, string str)
    {
        spreadsheet[row, col] = str;
    }

    public Tuple<int, int> SearchString(string str)
    {
        for (int row = 0; row < nRows; row++)
        {
            for (int col = 0; col < nCols; col++)
            {
                if (spreadsheet[row, col] == str)
                {
                    return Tuple.Create(row, col);
                }
            }
        }
        return null;
    }

    public void ExchangeRows(int row1, int row2)
    {
        for (int col = 0; col < nCols; col++)
        {
            string temp = spreadsheet[row1, col];
            spreadsheet[row1, col] = spreadsheet[row2, col];
            spreadsheet[row2, col] = temp;
        }
    }

    public void ExchangeCols(int col1, int col2)
    {
        for (int row = 0; row < nRows; row++)
        {
            string temp = spreadsheet[row, col1];
            spreadsheet[row, col1] = spreadsheet[row, col2];
            spreadsheet[row, col2] = temp;
        }
    }

    public int SearchInRow(int row, string str)
    {
        for (int col = 0; col < nCols; col++)
        {
            if (spreadsheet[row, col] == str)
            {
                return col;
            }
        }
        return -1;
    }

    public int SearchInCol(int col, string str)
    {
        for (int row = 0; row < nRows; row++)
        {
            if (spreadsheet[row, col] == str)
            {
                return row;
            }
        }
        return -1;
    }

    public Tuple<int, int> SearchInRange(int col1, int col2, int row1, int row2, string str)
    {
        col1 = Math.Max(0, Math.Min(col1, nCols - 1));
        col2 = Math.Max(0, Math.Min(col2, nCols - 1));
        row1 = Math.Max(0, Math.Min(row1, nRows - 1));
        row2 = Math.Max(0, Math.Min(row2, nRows - 1));

        for (int row = row1; row <= row2; row++)
        {
            for (int col = col1; col <= col2; col++)
            {
                if (spreadsheet[row, col] == str)
                {
                    return Tuple.Create(row, col);
                }
            }
        }
        return null;
    }

    public void AddRow(int row)
    {
        string[,] newSpreadsheet = new string[nRows + 1, nCols];
        for (int i = 0, newRow = 0; i < nRows; i++, newRow++)
        {
            if (i == row + 1)
            {
                newRow++;
            }
            for (int col = 0; col < nCols; col++)
            {
                newSpreadsheet[newRow, col] = spreadsheet[i, col];
            }
        }
        nRows++;
        spreadsheet = newSpreadsheet;
    }

    public void AddCol(int col)
    {
        string[,] newSpreadsheet = new string[nRows, nCols + 1];
        for (int row = 0; row < nRows; row++)
        {
            for (int i = 0, newCol = 0; i < nCols; i++, newCol++)
            {
                if (i == col + 1)
                {
                    newCol++;
                }
                newSpreadsheet[row, newCol] = spreadsheet[row, i];
            }
        }
        nCols++;
        spreadsheet = newSpreadsheet;
    }

    public Tuple<int, int>[] FindAll(string str, bool caseSensitive)
    {
        var positions = new List<Tuple<int, int>>();
        for (int row = 0; row < nRows; row++)
        {
            for (int col = 0; col < nCols; col++)
            {
                string cellValue = spreadsheet[row, col];
                if (!caseSensitive)
                {
                    cellValue = cellValue.ToLower();
                    str = str.ToLower();
                }
                if (cellValue == str)
                {
                    positions.Add(Tuple.Create(row, col));
                }
            }
        }
        return positions.ToArray();
    }

    public void SetAll(string oldStr, string newStr, bool caseSensitive)
    {
        for (int row = 0; row < nRows; row++)
        {
            for (int col = 0; col < nCols; col++)
            {
                string cellValue = spreadsheet[row, col];
                if (!caseSensitive)
                {
                    cellValue = cellValue.ToLower();
                    oldStr = oldStr.ToLower();
                }
                if (cellValue == oldStr)
                {
                    spreadsheet[row, col] = newStr;
                }
            }
        }
    }

    public Tuple<int, int> GetSize()
    {
        return Tuple.Create(nRows, nCols);
    }

    public void Print()
    {
        int cellWidth = 5; // Define the width of each cell for consistent formatting

        for (int row = 0; row < nRows; row++)
        {
            for (int col = 0; col < nCols; col++)
            {
                string cellValue = spreadsheet[row, col] ?? string.Empty;
                Console.Write(cellValue.PadRight(cellWidth));
            }
            Console.WriteLine();
        }
    }
}
