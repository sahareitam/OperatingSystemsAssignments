using System;
using System.Collections.Generic;
using System.Threading;



public interface IBinaryTree
{
    void Add(string value);
    bool Delete(string value);
    int Search(string value);
    void PrintSorted();
}



public class ThreadSafeBinaryTree : IBinaryTree
{
    private class Node
    {
        public string value;
        public int count;
        public Node left;
        public Node right;

        public Node(string value)
        {
            this.value = value;
            this.count = 1;
            this.left = null;
            this.right = null;
        }
    }

    private Node root;
    private ReaderWriterLockSlim readerwriter_lock = new ReaderWriterLockSlim();

    public void Add(string value)
    {
        readerwriter_lock.EnterWriteLock();
        try
        {
            root = Add(root, value);
        }
        finally
        {
            readerwriter_lock.ExitWriteLock();
        }
    }


    public bool Delete(string value)
    {
        readerwriter_lock.EnterWriteLock();
        try
        {
            bool deleted;
            root = Delete(root, value, out deleted);
            return deleted;
        }
        finally
        {
            readerwriter_lock.ExitWriteLock();
        }
    }


    private Node Add(Node node, string value)
    {
        if (node == null)
        {
            return new Node(value);
        }

        int comparing_test = string.Compare(value, node.value, StringComparison.Ordinal);
        if (comparing_test == 0)
        {
            node.count++;
        }
        else if (comparing_test < 0)
        {
            node.left = Add(node.left, value);
        }
        else
        {
            node.right = Add(node.right, value);
        }

        return node;
    }


    private Node Delete(Node node, string value, out bool deleted)
    {
        deleted = false;

        if (node == null)
        {
            return null;
        }

        int comparison = string.Compare(value, node.value, StringComparison.Ordinal);

        if (comparison == 0)
        {
            deleted = true;

            if (node.count > 1)
            {
                node.count--;
                return node;
            }


            if (node.left == null)
            {
                return node.right;
            }
            if (node.right == null)
            {
                return node.left;
            }

            Node successor = GetMin(node.right);
            node.value = successor.value;
            node.count = successor.count;
            successor.count = 1;
            node.right = Delete(node.right, successor.value, out _);
        }
        else if (comparison < 0)
        {
            node.left = Delete(node.left, value, out deleted);
        }
        else
        {
            node.right = Delete(node.right, value, out deleted);
        }

        return node;
    }



    private Node GetMin(Node node)
    {
        while (node.left != null)
        {
            node = node.left;
        }
        return node;
    }



    public int Search(string value)
    {
        readerwriter_lock.EnterReadLock();
        try
        {
            return GetNodeCount(root, value);
        }
        finally
        {
            readerwriter_lock.ExitReadLock();
        }
    }

    private int GetNodeCount(Node node, string value)
    {
        if (node == null)
        {
            return 0;
        }

        int comparisonResult = string.Compare(value, node.value, StringComparison.Ordinal);

        if (comparisonResult == 0)
        {
            return node.count;
        }
        else if (comparisonResult < 0)
        {
            return GetNodeCount(node.left, value);
        }
        else
        {
            return GetNodeCount(node.right, value);
        }
    }

    public void PrintSorted()
    {
        readerwriter_lock.EnterReadLock();
        try
        {
            PrintInOrder(root);
        }
        finally
        {
            readerwriter_lock.ExitReadLock();
        }
    }

    private void PrintInOrder(Node node)
    {
        if (node != null)
        {
          
            PrintInOrder(node.left);
            PrintNode(node);       
            PrintInOrder(node.right);
        }
    }

    private void PrintNode(Node node)
    {
        Console.WriteLine($"{node.value} ({node.count})");
    }
}
