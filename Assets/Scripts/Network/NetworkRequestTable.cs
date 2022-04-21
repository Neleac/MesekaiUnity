using UnityEngine;

using System;
using System.Collections.Generic;

public class NetworkRequestTable
{
	public static Dictionary<short, Type> requestTable { get; set; }

	public static void init()
	{
		requestTable = new Dictionary<short, Type>();
		add(Constants.CMSG_JOIN, "RequestJoin");
		add(Constants.CMSG_LEAVE, "RequestLeave");
	}

	public static void add(short request_id, string name)
	{
		requestTable.Add(request_id, Type.GetType(name));
	}

	public static NetworkRequest get(short request_id)
	{
		NetworkRequest request = null;

		if (requestTable.ContainsKey(request_id))
		{
			request = (NetworkRequest)Activator.CreateInstance(requestTable[request_id]);
			request.request_id = request_id;
		}
		else
		{
			Debug.Log("Request [" + request_id + "] Not Found");
		}

		return request;
	}
}
