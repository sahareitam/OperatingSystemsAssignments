using System;
using System.Diagnostics;
using System.Threading;

class MatrixMultiplier
{
    public static void MultiplyMatricesConcurrently(int[,] matrixA, int[,] matrixB, int[,] resultMatrix, int rowsA, int colsA, int colsB, int numThreads)
    {
        int rows_Thread = rowsA / numThreads;
        int remaining_Rows = rowsA % numThreads;

        Thread[] threads = new Thread[numThreads];

        for (int i = 0; i < numThreads; i++)
        {
            int startRow = i * rows_Thread;
            int endRow = (i == numThreads - 1) ? (i + 1) * rows_Thread + remaining_Rows : (i + 1) * rows_Thread;

            threads[i] = new Thread(() => ENV_rMultiply(matrixA, matrixB, resultMatrix, colsA, colsB, startRow, endRow));
            threads[i].Start();
        }

        for (int i = 0; i < numThreads; i++)
        {
            threads[i].Join();
        }
    }

    public static void ENV_rMultiply(int[,] matrixA, int[,] matrixB, int[,] resultMatrix, int colsA, int colsB, int startI, int endI)
    {
        for (int i = startI; i < endI; i++)
        {
            for (int j = 0; j < colsB; j++)
            {
                resultMatrix[i, j] = 0;
                for (int k = 0; k < colsA; k++)
                {
                    resultMatrix[i, j] += matrixA[i, k] * matrixB[k, j];
                }
            }
        }
    }
}


//    // Print matrix for checking
//    public static void PrintMatrix(int[,] matrix)
//    {
//        int rows = matrix.GetLength(0);
//        int cols = matrix.GetLength(1);

//        for (int i = 0; i < rows; i++)
//        {
//            for (int j = 0; j < cols; j++)
//            {
//                Console.Write(matrix[i, j] + "\t");
//            }
//            Console.WriteLine();
//        }
//    }
//    // Initialize random matrix by range of values
//    private static Random random = new Random();


//    // Regular matrix multiplication without threads (for comparison)
//    public static void MultiplyRegular(int[,] matrixA, int[,] matrixB, int[,] resultMatrix, int rowsA, int colsA, int colsB)
//    {
//        for (int i = 0; i < rowsA; i++)
//        {
//            for (int j = 0; j < colsB; j++)
//            {
//                resultMatrix[i, j] = 0;
//                for (int k = 0; k < colsA; k++)
//                {
//                    resultMatrix[i, j] += matrixA[i, k] * matrixB[k, j];
//                }
//            }
//        }
//    }

//    public static void FillMatrixWithRandomValues(int[,] matrix, int minValue, int maxValue)
//    {
//        int rows = matrix.GetLength(0);
//        int cols = matrix.GetLength(1);

//        for (int i = 0; i < rows; i++)
//        {
//            for (int j = 0; j < cols; j++)
//            {
//                matrix[i, j] = random.Next(minValue, maxValue);
//            }
//        }
//    }

//    static void Main(string[] args)
//    {
//        int rowsA = 1000; // הגדלת גודל המטריצה
//        int colsA = 2000;
//        int colsB = 5000;
//        int numThreads = 20; // הגדלת מספר החוטים

//        int[,] matrixA = new int[rowsA, colsA];
//        int[,] matrixB = new int[colsA, colsB];
//        int[,] resultMatrix = new int[rowsA, colsB];
//        int[,] resultMatrixRegular = new int[rowsA, colsB];

//        // Fill matrices with random values
//        MatrixMultiplier.FillMatrixWithRandomValues(matrixA, 1, 10);
//        MatrixMultiplier.FillMatrixWithRandomValues(matrixB, 1, 10);

//        // Test threaded multiplication
//        Stopwatch stopwatch = new Stopwatch();
//        stopwatch.Start();
//        MatrixMultiplier.MultiplyMatricesConcurrently(matrixA, matrixB, resultMatrix, rowsA, colsA, colsB, numThreads);
//        stopwatch.Stop();
//        Console.WriteLine($"Time with {numThreads} threads: {stopwatch.ElapsedMilliseconds} ms");

//        // Test regular multiplication
//        stopwatch.Restart();
//        MatrixMultiplier.MultiplyRegular(matrixA, matrixB, resultMatrixRegular, rowsA, colsA, colsB);
//        stopwatch.Stop();
//        Console.WriteLine($"Time without threads: {stopwatch.ElapsedMilliseconds} ms");

//        // Compare the results to ensure correctness
//        bool isEqual = true;
//        for (int i = 0; i < rowsA; i++)
//        {
//            for (int j = 0; j < colsB; j++)
//            {
//                if (resultMatrix[i, j] != resultMatrixRegular[i, j])
//                {
//                    isEqual = false;
//                    break;
//                }
//            }
//        }

//        if (isEqual)
//        {
//            Console.WriteLine("The results are correct.");
//        }
//        else
//        {
//            Console.WriteLine("The results are incorrect.");
//        }
//    }
//}


