using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestMove : NetworkRequest
{
	public RequestMove()
	{
		request_id = Constants.CMSG_MOVE;
	}

	public void send(string playerName, Transform avatarTf)
	{
		packet = new GamePacket(request_id);
        packet.addString(playerName);

		// avatar root transform
        packet.addFloat32(avatarTf.position.x);
        packet.addFloat32(avatarTf.position.y);
        packet.addFloat32(avatarTf.position.z);
		packet.addFloat32(avatarTf.eulerAngles.x);
        packet.addFloat32(avatarTf.eulerAngles.y);
        packet.addFloat32(avatarTf.eulerAngles.z);

		// // armature joints rotations
		// ArrayList jointRots = new ArrayList();
		// Transform spine = avatarTf.Find("Armature/Hips");
		// getJointAngles(spine, jointRots);

		// packet.addInt32(jointRots.Count * 3);
		// foreach (Vector3 rot in jointRots)
		// {
		// 	packet.addFloat32(rot.x);
		// 	packet.addFloat32(rot.y);
		// 	packet.addFloat32(rot.z);
		// }
	}

	private void getJointAngles(Transform jointTf, ArrayList jointRots)
	{
		jointRots.Add(jointTf.eulerAngles);

		// DFS
		foreach (Transform child in jointTf)
        {
            getJointAngles(child, jointRots);
        }
	}
}