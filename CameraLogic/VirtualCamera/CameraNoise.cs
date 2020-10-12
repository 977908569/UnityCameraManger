using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class CameraNoise : CameraComponentBase
{
	public float shakeOnceTime = 0.05f;//一次时间
	public float startSpeed = 1;//起始速度
	public float acceleratedSpeed = 0;//加速度

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

		Vector3 localPos = transform.InverseTransformPoint(curInfo.Position);
		localPos += GetCombinedFilterResults();
		curInfo.Position = transform.TransformPoint(localPos);
	}
	protected override void Update()
	{
		base.Update();
		if (enabled)
		{
			if (!shakeEnabled)
			{
				DOTween.To((args) => { tempTime = args; }, 0, shakeOnceTime, shakeOnceTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
				shakeEnabled = true;
			}
		}
		else
		{
			shakeEnabled = false;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		shakeEnabled = false;
	}

	bool shakeEnabled = false;
	private float tempTime=0;

	private Vector3 GetCombinedFilterResults()
	{
		float xPos = 0f;
		float yPos = 0f;
		float zPos = 0f;

		zPos += startSpeed * tempTime + 0.5f * acceleratedSpeed * tempTime * tempTime;

		return new Vector3(xPos, yPos, zPos);
	}
}
