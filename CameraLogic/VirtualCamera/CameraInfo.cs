using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraInfo
{
	public Vector3 Position;
	public Quaternion Rotation;
	public float FieldOfView;
	public float ClippingNear;
	public float ClippingFar;

	public override string ToString()
	{
		return "pos" + Position + "  rotation:" + Rotation + "  field" + FieldOfView;
	}

	public static CameraInfo Default
	{
		get
		{
			CameraInfo info = new CameraInfo();
			info.FieldOfView = 45;
			info.ClippingFar = 1000;
			info.ClippingNear = 0.01f;
			return info;
		}
	}

	public static CameraInfo Lerp(CameraInfo infoA, CameraInfo infoB, float t)
	{
		t = Mathf.Clamp01(t);

		CameraInfo temp = new CameraInfo();
		temp.Position = Vector3.Lerp(infoA.Position, infoB.Position, t);
		temp.Rotation = Quaternion.Lerp(infoA.Rotation, infoB.Rotation, t);
		temp.FieldOfView = Mathf.Lerp(infoA.FieldOfView, infoB.FieldOfView, t);
		temp.ClippingNear= Mathf.Lerp(infoA.ClippingNear, infoB.ClippingNear, t);
		temp.ClippingFar = Mathf.Lerp(infoA.ClippingFar, infoB.ClippingFar, t);
		return temp;
	}
}
