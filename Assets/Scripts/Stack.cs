using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    public Stack<int> numberStack = new Stack<int>();
    public Queue<int> numberQueue = new Queue<int>();
    int n = 5;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= n; i++)
        {
            numberStack.Push(i);
            numberQueue.Enqueue(i);
        }

        for (int i = 1; i <= n; i++)
        {
            int num = numberStack.Pop();
            Debug.Log(num);
        }

        for (int i = 1; i <= n; i++)
        {
            int num = numberQueue.Dequeue();
            Debug.Log(num);
        }
    }

}
