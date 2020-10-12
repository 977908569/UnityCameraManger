using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCore
{
	private static CameraCore sInstance = null;

	/// <summary>Get the singleton instance</summary>
	public static CameraCore Instance
	{
		get
		{
			if (sInstance == null)
				sInstance = new CameraCore();
			return sInstance;
		}
	}
	//相机大脑列表
	private List<CameraBrain> mActiveBrains = new List<CameraBrain>();

	public CameraBrain GetActiveBrain()
	{
		if (mActiveBrains.Count > 0)
		{
			return mActiveBrains[0];
		}
		return null;
	}

	public void EnableShakeCamera()
	{
		CameraBrain _ab = GetActiveBrain();
		if (_ab != null)
		{
			if (_ab.activeCamera != null)
			{
				_ab.activeCamera.EnableNNoise();
			}
		}
	}

	public void DisableShakeCamera()
	{
		CameraBrain _ab = GetActiveBrain();
		if (_ab != null)
		{
			if (_ab.activeCamera != null)
			{
				_ab.activeCamera.DisableNNoise();
			}
		}
	}

	//添加大脑
	public void AddActiveBrain(CameraBrain brain)
	{
		RemoveActiveBrain(brain);
		mActiveBrains.Insert(0, brain);
	}

	public void RemoveActiveBrain(CameraBrain brain)
	{
		mActiveBrains.Remove(brain);
	}
}
