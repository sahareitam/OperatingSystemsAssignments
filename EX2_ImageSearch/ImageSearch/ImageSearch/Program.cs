using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class ImageSearch
{
    public static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: ImageSearch <image1> <image2> <nThreads> <algorithm>");
            return;
        }

        string image1_path = args[0];
        string image2_path = args[1];

        if (!int.TryParse(args[2], out int nThreads) || (args[3] != "exact" && args[3] != "euclidian") || nThreads < 1)
        {
            Console.WriteLine("Error:Invalid parameters.");
            return;
        }
        string The_algorithm = args[3];

        SixLabors.ImageSharp.Image<Rgba32> Large_img;
        SixLabors.ImageSharp.Image<Rgba32> small_img;

        try
        {
            Large_img = SixLabors.ImageSharp.Image.Load<Rgba32>(image1_path);
            small_img = SixLabors.ImageSharp.Image.Load<Rgba32>(image2_path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading images: {ex.Message}");
            return;
        }



        Rgba32[][] large_Arr = Convert_2D_Array(Large_img);
        Rgba32[][] small_Arr = Convert_2D_Array(small_img);

        List<(int, int)> results = new List<(int, int)>();
        int L_height = Large_img.Height;
        int L_width = Large_img.Width;
        int S_Height = small_img.Height;
        int S_Width = small_img.Width;

        Thread[] threads = new Thread[nThreads];
        int slice_Height = L_height / nThreads;

        for (int i = 0; i < nThreads; i++)
        {
            int start_Row = i * slice_Height;
            int end_Row = (i == nThreads - 1) ? L_height : start_Row + slice_Height + S_Height - 1;
            threads[i] = new Thread(() => Search(large_Arr, small_Arr, start_Row, end_Row, L_width, S_Height, S_Width, The_algorithm, results));
            threads[i].Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        foreach (var result in results)
        {
            Console.WriteLine($"{result.Item1},{result.Item2}");
        }
    }

    private static Rgba32[][] Convert_2D_Array(SixLabors.ImageSharp.Image<Rgba32> image)
    {
        int width = image.Width;
        int height = image.Height;
        Rgba32[][] array = new Rgba32[height][];
        for (int y = 0; y < height; y++)
        {
            array[y] = new Rgba32[width];
            for (int x = 0; x < width; x++)
            {
                array[y][x] = image[x, y];
            }
        }
        return array;
    }


    private static void Search(Rgba32[][] large_Arr, Rgba32[][] small_Arr, int start_Row, int end_Row, int width, int s_Height, int s_Width, string algorithm, List<(int, int)> results)
    {
        for (int y = start_Row; y <= Math.Min(end_Row - s_Height, large_Arr.Length - s_Height); y++)
        {
            for (int x = 0; x <= width - s_Width; x++)
            {
                bool match = false;
                if (algorithm == "exact")
                {
                    match = Exact_algo(large_Arr, small_Arr, y, x, s_Height, s_Width);
                }
                else if (algorithm == "euclidian")
                {
                    match = Euclidian_algo(large_Arr, small_Arr, y, x, s_Height, s_Width);
                }
                if (match)
                {
                    lock (results)
                    {
                        results.Add((y, x));
                    }
                }
            }
        }
    }




    private static bool Exact_algo(Rgba32[][] large_Arr, Rgba32[][] small_Arr, int start_Y, int start_X, int s_Height, int s_Width)
    {
        for (int y = 0; y < s_Height; y++)
        {
            for (int x = 0; x < s_Width; x++)
            {
                if (!large_Arr[start_Y + y][start_X + x].Equals(small_Arr[y][x]))
                {
                    return false;
                }
            }
        }
        return true;
    }



    private static bool Euclidian_algo(Rgba32[][] large_Arr, Rgba32[][] small_Arr, int start_Y, int start_X, int s_Height, int s_Width)
    {
        for (int y = 0; y < s_Height; y++)
        {
            for (int x = 0; x < s_Width; x++)
            {
                Rgba32 c1 = large_Arr[start_Y + y][start_X + x];
                Rgba32 c2 = small_Arr[y][x];
                double dist = (c1.R - c2.R) * (c1.R - c2.R) +
                              (c1.G - c2.G) * (c1.G - c2.G) +
                              (c1.B - c2.B) * (c1.B - c2.B);

                if (dist > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
