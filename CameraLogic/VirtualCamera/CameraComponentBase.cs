using System;
using System.Collections.Generic;
using UnityEngine;
public class CameraComponentBase : MonoBehaviour
{
	public VirtualCameraBase VirtualCamera
	{
		get
		{
			if (m_vcamOwner == null)
				m_vcamOwner = gameObject.GetComponent<VirtualCameraBase>();
			return m_vcamOwner;
		}
	}
	VirtualCameraBase m_vcamOwner;

	public virtual bool IsValid { get; set; }

	public virtual void MutateCameraInfo(ref CameraInfo curInfo)
	{

	}

	protected virtual void Start()
	{

	}

	protected virtual void Update()
	{
		
	}

	protected virtual void OnDisable()
	{
		
	}
}