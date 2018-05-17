using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadQueue : MonoBehaviour {

    private List<Action> FunctionsNeedRunInQueue  = new List<Action>();
	
	void Update ()
    {
        lock(FunctionsNeedRunInQueue)
        {
            if (FunctionsNeedRunInQueue.Count > 0)
            {
                Action action = FunctionsNeedRunInQueue[0];
                FunctionsNeedRunInQueue.RemoveAt(0);
                action();
            }
        }
	}

    public void StartThreadFunction(Action action)
    {
        Thread thread = new Thread(() => action());
        thread.Start();
    }

    public void StartFunctionInMainThread(Action action)
    {
        lock(FunctionsNeedRunInQueue)
        {
            FunctionsNeedRunInQueue.Add(action);
        }
    }
}
