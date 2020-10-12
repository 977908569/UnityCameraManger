using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraBase : MonoBehaviour
{
	public CameraBlendDefinition defaultBlendDefinition = new CameraBlendDefinition(CameraBlendDefinition.Style.EaseInOut, 0.5f);
	//进入相机的混合参数设置
	public CameraBlenderSettings customBlends = null;
	
	//相机通用参数
	public CameraParameters cameraParameters;

	//优先级
	public int m_Priority = 10;
	public int Priority { get { return m_Priority; } set { m_Priority = value; } }

	//gameobject
	public GameObject VirtualCameraGameObject
	{
		get
		{
			if (this == null)
				return null; // object deleted
			return gameObject;
		}
	}

	//相机信息
	public  CameraInfo CInfo { get; set; }

	//目标
	[HideInInspector]
	public Transform Target;
	public CameraTargetType targetType = CameraTargetType.Player_Target;
	//名字
	public string Name { get { return name; } set { name = value; } }

	public bool IsLive { get { return Brain.IsLive(this); }}

	private CameraBrain m_Brain;
	//相机的控制者
	public CameraBrain Brain {
		get
		{
			return m_Brain;
		}
		set
		{
			m_Brain = value;
			Target = m_Brain.TargetDicGet(targetType);
		}
	}

	public bool IsBlendOver()
	{
		return !Brain.IsBlend() && Brain.activeCamera == this;
	}

	public void EnableNNoise()
	{
		if (cameraNoise != null)
		{
			cameraNoise.enabled = true;
		}
	}

	public void DisableNNoise()
	{
		if (cameraNoise != null)
		{
			cameraNoise.enabled = false;
		}
	}

	//相机切换
	public virtual void OnTransitionFromCamera(VirtualCameraBase fromCam, Vector3 worldUp)
	{

	}

	//更新相机信息
	public virtual void UpdateCameraInfo(Vector3 worldUp)
	{
		if (IsLive)
		{
			CInfo = PullInfoFromVirtualCamera(worldUp);
			//
			CInfo = CalculateNewInfo(worldUp);
		}
	}

	private CameraNoise cameraNoise;
	private CameraCollider cameraCollider;
	private CameraInfo CalculateNewInfo(Vector3 worldUp)
	{
		CameraInfo info = CInfo;
		
		if (cameraCollider != null)
		{
			cameraCollider.MutateCameraInfo(ref info);
		}

		//放最后
		if (cameraNoise != null)
		{
			cameraNoise.MutateCameraInfo(ref info);
		}

		return info;
	}

	private CameraInfo PullInfoFromVirtualCamera(Vector3 worldUp)
	{
		CameraInfo state = CameraInfo.Default;
		//去取当前相机信息
		state.Position = transform.position;
		state.Rotation = transform.rotation;
		state.FieldOfView = cameraParameters.FieldOfView;
		state.ClippingNear = cameraParameters.ClippingNear;
		state.ClippingFar = cameraParameters.ClippingFar;
		return state;
	}

	private int m_QueuePriority = int.MaxValue;

	private void UpdateVcamPoolInfos()
	{
		m_QueuePriority = int.MaxValue;
		if (isActiveAndEnabled)
		{
			if (Brain != null)
			{
				Brain.AddActiveCamera(this);
				m_QueuePriority = m_Priority;
			}
		}
	}

	protected virtual void Start()
	{
		cameraNoise = GetComponent<CameraNoise>();
		cameraCollider = GetComponent<CameraCollider>();
	}

	protected virtual void Update()
	{
		if (m_Priority != m_QueuePriority)
			UpdateVcamPoolInfos();
	}

	protected virtual void OnEnable()
	{
		UpdateVcamPoolInfos();
	}

	protected virtual void OnDisable()
	{
		UpdateVcamPoolInfos();
	}
}

[Serializable]
public struct CameraParameters
{
	//视口大小
	public float FieldOfView;
	public float ClippingNear;
	public float ClippingFar;
}
