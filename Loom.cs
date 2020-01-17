using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

[ExecuteInEditMode]
/// <summary>
/// Multithreading support
/// </summary>
public class Loom : MonoBehaviour
{
	private static Loom _current;

	/// <summary>
	/// Return the current instance
	/// </summary>
	/// <value>
	/// 
	/// </value>
	public static Loom Current
	{
		get
		{
			if(!_initialized) Initialize();
			return _current;
		}
	}
	
	static bool _initialized;
	static int _threadId=-1;
	
	public static void Initialize()
	{
		
		var go = !_initialized;
		
		if(go && _threadId != -1 && _threadId != Thread.CurrentThread.ManagedThreadId)
			return;
		
		if (go)
		{
			var g = new GameObject("Loom");
			g.hideFlags = HideFlags.HideAndDontSave;
			GameObject.DontDestroyOnLoad(g);
			_current = g.AddComponent<Loom>();
			Component.DontDestroyOnLoad(_current);
			_initialized = true;
			_threadId = Thread.CurrentThread.ManagedThreadId;
		}
			
	}
	
	void OnDestroy()
	{
		_initialized = false;
	}
	
	private List<Action> _actions = new List<Action>();
	
	/// <summary>
	/// Queues an action on the main thread
	/// </summary>
	/// <param name='action'>
	/// The action to execute
	/// </param>
	public static void QueueOnMainThread(Action action)
	{
		lock (Current._actions)
		{
			Current._actions.Add(action);
		}
	}
	
	/// <summary>
	/// Runs an action on another thread
	/// </summary>
	/// <param name='action'>
	/// The action to execute on another thread
	/// </param>
	public static void RunAsync(Action action)
	{
		var t = new Thread(RunAction);
		t.Priority = System.Threading.ThreadPriority.Normal;
		t.Start(action);
	}
	
	private static void RunAction(object action)
	{
		((Action)action	)();
	}
	

	// Use this for initialization
	void Start()
	{
	
	}
	
	List<Action> toBeRun = new List<Action>();
	
	// Update is called once per frame
	void Update()
	{
		lock (_actions)
		{
			if (0 == _actions.Count)
			{
				return;
			}
			
			toBeRun.AddRange(_actions);
			_actions.Clear();
		}
		
		foreach(var a in toBeRun)
		{
			try {
				a();
			}
			catch (Exception e)
			{
				Debug.LogError("Queued Exception: " + e.ToString());
			}
		}
		
		toBeRun.Clear();
	}
}
