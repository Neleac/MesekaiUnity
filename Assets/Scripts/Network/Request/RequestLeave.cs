using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestLeave : NetworkRequest
{
	public RequestLeave()
	{
		request_id = Constants.CMSG_LEAVE;
	}

	public void send()
	{
		packet = new GamePacket(request_id);
	}
}
