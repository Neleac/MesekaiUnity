using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	private ConnectionManager cManager;

	public string playerName;		// name of this client
	public string[] otherPlayers;	// name of players joined BEFORE client
	public bool otherJoined;		// flag for client to send transform to new joined player

	void Awake()
	{
		DontDestroyOnLoad(gameObject);

		gameObject.AddComponent<MessageQueue>();
		gameObject.AddComponent<ConnectionManager>();

		NetworkRequestTable.init();
		NetworkResponseTable.init();
	}

	void Start()
	{
		cManager = GetComponent<ConnectionManager>();
		if (cManager)
		{
			cManager.setupSocket();
			StartCoroutine(RequestHeartbeat(0.1f));
		}

		otherJoined = false;
	}

	public bool SendJoinRequest(string playerName)
	{
		if (cManager && cManager.IsConnected())
		{
			RequestJoin request = new RequestJoin();
			request.send(playerName);
			cManager.send(request);
			return true;
		}
		return false;
	}

	public bool SendLeaveRequest()
	{
		if (cManager && cManager.IsConnected())
		{
			RequestLeave request = new RequestLeave();
			request.send();
			cManager.send(request);
			return true;
		}
		return false;
	}

	public bool SendMoveRequest(string playerName, Transform avatarTF, bool motionTracking)
	{
		if (cManager && cManager.IsConnected())
		{
			RequestMove request = new RequestMove();
			request.send(playerName, avatarTF, motionTracking);
			cManager.send(request);
			return true;
		}
		return false;
	}

	public bool SendAnimateRequest(string playerName, Dictionary<string, bool> animStateDict, Dictionary<string, float> animValDict)
	{
		if (cManager && cManager.IsConnected())
		{
			RequestAnimate request = new RequestAnimate();
			request.send(playerName, animStateDict, animValDict);
			cManager.send(request);
			return true;
		}
		return false;
	}

    public IEnumerator RequestHeartbeat(float time)
	{
		yield return new WaitForSeconds(time);

		if (cManager)
		{
			RequestHeartbeat request = new RequestHeartbeat();
			request.send();
			cManager.send(request);
		}

		StartCoroutine(RequestHeartbeat(time));
	}
}
