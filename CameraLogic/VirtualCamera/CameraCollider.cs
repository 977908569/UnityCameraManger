using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : CameraComponentBase
{
	public override bool IsValid
	{
		get
		{
			return enabled;
		}

		set
		{
			base.IsValid = value;
		}
	}

	public override void MutateCameraInfo(ref CameraInfo curInfo)
	{
		if (!IsValid)
			return;
		if (VirtualCamera.Target == null)
			return;
		Vector3 pos = transform.position;
		CheckPos(VirtualCamera.Target.position, pos, out pos);
		curInfo.Position = pos;
	}

	private bool CheckPos(Vector3 start, Vector3 end, out Vector3 calPos)
	{
		calPos = end;
		RaycastHit hit;
		int mask = GameLayerMasks.Default & ~(1 << GameLayers.AirWall) & ~(1 << GameLayers.Bullet);
		if (Physics.Linecast(start, end, out hit, mask))
		{
			calPos = hit.point;
			//Debug.DrawLine(getV, end, Color.red);
			//Debug.DrawLine(start, getV, Color.yellow);
			return false;
		}
		return true;
	}
}
