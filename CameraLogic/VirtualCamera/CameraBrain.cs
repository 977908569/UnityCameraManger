using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CameraTargetType
{
	Player_Target,
	TT_Target,
	None
}
public class CameraBrain : MonoBehaviour
{
	#region 相机目标
	[HideInInspector]
	[SerializeField]
	public CameraTargetType[] Keys;
	[HideInInspector]
	[SerializeField]
	public Transform[] Values;
	public Dictionary<CameraTargetType, Transform> targetDic;
	#endregion
	//
	public Vector3 DefaultWorldUp { get { return Vector3.up; } }
	//[HideInInspector]
	public VirtualCameraBase activeCamera;
	//unity camera
	private Camera m_OutputCamera = null;
	[HideInInspector]
	public Camera OutputCamera
	{
		get
		{
			if (m_OutputCamera == null)
				m_OutputCamera = GetComponent<Camera>();
			return m_OutputCamera;
		}
	}

	public Action blendComplete;

	private void Awake()
	{
		InitDic();
	}
	private void InitDic()
	{
		targetDic = new Dictionary<CameraTargetType, Transform>();
		for (int i = 0; i < Keys.Length; i++)
		{
			if (!targetDic.ContainsKey(Keys[i]))
				targetDic.Add(Keys[i], Values[i]);
		}
	}
	public Transform TargetDicGet(CameraTargetType key)
	{
		Transform targetTrans = null;
		if (targetDic == null)
			InitDic();
		targetDic.TryGetValue(key, out targetTrans);
		return targetTrans;
	}
	public bool IsLive(VirtualCameraBase vcam)
	{
		if (activeCamera == vcam)
			return true;
		if (IsBlendUse(vcam))
			return true;
		return false;
	}

	public bool IsBlendUse(VirtualCameraBase vcam)
	{
		if (activeBlend != null && activeBlend.IsUse(vcam))
			return true;
		return false;
	}

	public bool IsBlend()
	{
		if (activeBlend != null && activeBlend.IsValid)
			return true;
		return false;
	}

	private void OnEnable()
	{
		//找出大脑的所有虚拟相机
		VirtualCameraBase[] _allVC = transform.parent.GetComponentsInChildren<VirtualCameraBase>();
		mActiveCameras.Clear();
		for (int i = 0; i < _allVC.Length; i++)
		{
			_allVC[i].Brain = this;
			mActiveCameras.Add(_allVC[i]);
		}
		CameraCore.Instance.AddActiveBrain(this);
	}

	private void LateUpdate()
	{
		ProcessActiveCamera();
	}

	#region 相机融合
	private CameraBlend CreateBlend(VirtualCameraBase camA, VirtualCameraBase camB, AnimationCurve blendCurve, float duration)
	{
		if (blendCurve == null || duration <= 0 || (camA == null && camB == null))
			return null;

		return new CameraBlend(camA, camB, blendCurve, duration, 0);
	}

	private AnimationCurve LookupBlendCurve(VirtualCameraBase fromKey, VirtualCameraBase toKey, out float duration)
	{
		AnimationCurve blendCurve = toKey.defaultBlendDefinition.BlendCurve;
		if (toKey.customBlends != null)
		{
			blendCurve = toKey.customBlends.GetBlendCurveForVirtualCameras(fromKey, blendCurve);
		}

		var keys = blendCurve.keys;
		duration = (keys == null || keys.Length == 0) ? 0 : keys[keys.Length - 1].time;
		return blendCurve;
	}
	#endregion

	private CameraBlend activeBlend;

	//选择一个合适的虚拟相机并把信息应用于unity 相机
	private void ProcessActiveCamera()
	{
		if (!isActiveAndEnabled)
		{
			return;
		}

		VirtualCameraBase previousCam = activeCamera;
		activeCamera = ActiveVirtualCamera;
		if (activeCamera != null)
		{
			if (previousCam != null && previousCam.VirtualCameraGameObject == null)
				return;

			//进行转移
			if (previousCam != null && previousCam != activeCamera)
			{
				float duration = 0;
				AnimationCurve curve = LookupBlendCurve(previousCam, activeCamera, out duration);
				activeBlend = CreateBlend(previousCam, activeCamera, curve, duration);
				//
				activeCamera.OnTransitionFromCamera(previousCam, DefaultWorldUp);
			}

			activeCamera.UpdateCameraInfo(DefaultWorldUp);
			CameraInfo info = activeCamera.CInfo;

			if (activeBlend != null)
			{
				if (activeBlend.IsComplete)
				{
					activeBlend = null;
					if (blendComplete != null)
					{
						blendComplete();
					}
					
				}
				else
				{
					activeBlend.UpdateCameraState(DefaultWorldUp);
					info = activeBlend.CInfo;
				}
			}

			PushInfoToUnityCamera(info);
		}
	}

	//相机的信息赋给unity 相机
	private void PushInfoToUnityCamera(CameraInfo info)
	{
		//Debug.Log("current set camera info:"+info.ToString());

		transform.position = info.Position;
		transform.rotation = info.Rotation;

		if (OutputCamera != null)
		{
			OutputCamera.fieldOfView = info.FieldOfView;
			OutputCamera.nearClipPlane = info.ClippingNear;
			OutputCamera.farClipPlane = info.ClippingFar;
		}
	}
	#region 获取优先级高的相机

	//当前激活的优先级高的虚拟相机
	public VirtualCameraBase ActiveVirtualCamera
	{
		get
		{
			int numCameras = VirtualCameraCount;
			if (numCameras > 0)
			{
				return GetVirtualCamera(0);
			}
			return null;
		}
	}
	#endregion

	// 激活的虚拟相机列表
	private List<VirtualCameraBase> mActiveCameras = new List<VirtualCameraBase>();

	//激活虚拟相机个数
	public int VirtualCameraCount { get { return mActiveCameras.Count; } }

	//获取激活虚拟相机
	public VirtualCameraBase GetVirtualCamera(int index)
	{
		return mActiveCameras[index];
	}

	//添加相机
	public void AddActiveCamera(VirtualCameraBase vcam)
	{
		// Bring it to the top of the list
		RemoveActiveCamera(vcam);

		// Keep list sorted by priority
		int insertIndex;
		for (insertIndex = 0; insertIndex < mActiveCameras.Count; ++insertIndex)
			if (vcam.Priority >= mActiveCameras[insertIndex].Priority)
				break;

		mActiveCameras.Insert(insertIndex, vcam);
	}

	//移除
	public void RemoveActiveCamera(VirtualCameraBase vcam)
	{
		mActiveCameras.Remove(vcam);
	}
}
