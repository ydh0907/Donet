using System;
using System.Collections.Generic;
using UnityEngine;

public class JobQueue : MonoBehaviour
{
    private static Queue<Action> jobs = new Queue<Action>();

    private static object locker = new object();

    public static void AddJop(Action action)
    {
        lock (locker)
            jobs.Enqueue(action);
    }

    private void Update()
    {
        lock (locker)
        {
            while (jobs.Count > 0)
                jobs.Dequeue()?.Invoke();
        }
    }
}
