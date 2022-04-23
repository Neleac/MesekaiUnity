using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestMove : NetworkRequest
{
	public RequestMove()
	{
		request_id = Constants.CMSG_MOVE;
	}

	public void send(string playerName, Vector3 position, Vector3 rotation)
	{
		packet = new GamePacket(request_id);
        packet.addString(playerName);
        packet.addFloat32(position.x);
        packet.addFloat32(position.y);
        packet.addFloat32(position.z);
		packet.addFloat32(rotation.x);
        packet.addFloat32(rotation.y);
        packet.addFloat32(rotation.z);
	}
}