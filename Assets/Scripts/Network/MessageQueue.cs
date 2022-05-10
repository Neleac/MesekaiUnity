using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MessageQueue : MonoBehaviour
{

	public delegate void Callback(ExtendedEventArgs eventArgs);
	public Dictionary<int, Callback> callbackList { get; set; }
	public Queue<ExtendedEventArgs> msgQueue { get; set; }

	void Awake()
	{
		callbackList = new Dictionary<int, Callback>();
		msgQueue = new Queue<ExtendedEventArgs>();
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		while (msgQueue != null && msgQueue.Count > 0)
		{
			ExtendedEventArgs args = msgQueue.Dequeue();
			if (callbackList.ContainsKey(args.event_id))
			{
				callbackList[args.event_id](args);
				Debug.Log("Processed Event No. " + args.event_id + " [" + args.GetType() + "]");
				Debug.Log("Processed Event No. " + args.event_id + " [" + args.ToString() + "]");
			}
			else
			{
				Debug.Log("Missing Event No. " + args.event_id + " [" + args.GetType() + "]");
			}
		}
	}

	public void AddCallback(int event_id, Callback callback)
	{
		callbackList.Add(event_id, callback);
	}

	public void RemoveCallback(int event_id)
	{
		callbackList.Remove(event_id);
	}

	public void AddMessage(int event_id, ExtendedEventArgs args)
	{
		msgQueue.Enqueue(args);
	}
}
