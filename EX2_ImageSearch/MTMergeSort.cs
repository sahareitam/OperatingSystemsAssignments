using System;
using System.Collections.Generic;
using System.Threading;

public class MTMergeSort
{
    // This function initiates the multithreaded merge-sort process.
    // It takes a list of strings (strList) and a minimum threshold (nMin) for determining when to stop creating new threads.
    public List<string> MergeSort(string[] strList, int nMin = 2)
    {
        // If the input array is null or has one or fewer elements, return it as there's nothing to sort.
        if (strList == null || strList.Length <= 1)
            return new List<string>(strList);

        // Loop through each string in the array and sort the individual characters of each string.
        for (int i = 0; i < strList.Length; i++)
        {
            strList[i] = Sort_ENV(strList[i]);
        }

        // Start the recursive merge sort process for the entire array.
        return Merge_Sort_rec(strList, 0, strList.Length - 1, nMin);
    }

    // Helper function to sort the characters in a single string.
    // Converts the string into a char array, sorts it, and then converts it back to a string.
    private string Sort_ENV(string input)
    {
        char[] chars = input.ToCharArray();
        Array.Sort(chars);
        return new string(chars);
    }

    // This is the recursive function that splits the input array into smaller subarrays
    // and sorts each one, either using threads or sequentially depending on the size.
    private List<string> Merge_Sort_rec(string[] strList, int left, int right, int nMin)
    {
        // Base case: if there is only one element in the current subarray, return it as a sorted list.
        if (left >= right)
            return new List<string> { strList[left] };

        // Find the middle index of the current subarray to split it into two halves.
        int mid = (left + right) / 2;
        List<string> leftSorted = null;
        List<string> rightSorted = null;

        // If the current subarray size is smaller than or equal to the threshold, sort sequentially.
        if (right - left + 1 <= nMin)
        {
            // If the subarray size is smaller than or equal to the threshold, sort sequentially (without threads).
            leftSorted = Merge_Sort_rec(strList, left, mid, nMin);
            rightSorted = Merge_Sort_rec(strList, mid + 1, right, nMin);
        }
        else
        {
            // Create a new thread to sort the left half of the subarray.
            Thread left_Thread = new Thread(() => leftSorted = Merge_Sort_rec(strList, left, mid, nMin));

            // Create a new thread to sort the right half of the subarray.
            Thread right_Thread = new Thread(() => rightSorted = Merge_Sort_rec(strList, mid + 1, right, nMin));

            // Start both threads to execute concurrently.
            left_Thread.Start();
            right_Thread.Start();

            // Wait for both threads to finish before proceeding to the merge step.
            left_Thread.Join();
            right_Thread.Join();

        }

        // Merge the two sorted halves into a single sorted list.
        return Merge(leftSorted, rightSorted);
    }

    // This function merges two sorted lists into a single sorted list.
    // It compares elements from both lists and adds the smaller one to the result.
    private List<string> Merge(List<string> left, List<string> right)
    {
        List<string> merged = new List<string>(); // Initialize the merged list.
        int i = 0, j = 0;

        // Compare elements from both lists one by one and add the smaller element to the merged list.
        while (i < left.Count || j < right.Count)
        {
            // If all elements from the 'left' list have been added, add the remaining elements from 'right'.
            if (i == left.Count)
            {
                merged.Add(right[j]);
                j++;
            }
            // If all elements from the 'right' list have been added, add the remaining elements from 'left'.
            else if (j == right.Count)
            {
                merged.Add(left[i]);
                i++;
            }
            // Otherwise, compare elements from both lists and add the smaller one.
            else
            {
                if (string.Compare(left[i], right[j]) <= 0)
                {
                    merged.Add(left[i]);
                    i++;
                }
                else
                {
                    merged.Add(right[j]);
                    j++;
                }
            }
        }

        // Return the final merged and sorted list.
        return merged;
    }
}

