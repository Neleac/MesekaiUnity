using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestMove : NetworkRequest
{
	public RequestMove()
	{
		request_id = Constants.CMSG_MOVE;
	}

	public void send(string playerName, Vector3 transform)
	{
		packet = new GamePacket(request_id);
        packet.addString(playerName);
        packet.addFloat32(transform.x);
        packet.addFloat32(transform.y);
        packet.addFloat32(transform.z);
	}
}