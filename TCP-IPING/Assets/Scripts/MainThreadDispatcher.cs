using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadDispatcher : ISingleton<MainThreadDispatcher>
{
    /*
    UnityEngine.UnityException: get_isActiveAndEnabled can only be called from the main thread.
    Constructors and field initializers will be executed from the loading thread when loading a scene.
    Don't use this function in the constructor or field initializers, instead move initialization code to the Awake or Start function.
    at (wrapper managed-to-native)
    */
    //Unity 메인쓰레드가 아닌 곳에서 호출되어 오류 발생. 문제 해결 (241209)
    private static readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    public static void ExecuteOnMainThread(Action action)
    {
        mainThreadActions.Enqueue(action);
    }

    private void Update()
    {
        while (mainThreadActions.TryDequeue(out var action))
        {
            action.Invoke();
        }
    }
}