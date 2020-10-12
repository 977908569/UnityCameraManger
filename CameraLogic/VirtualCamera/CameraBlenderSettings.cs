using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraBlenderSettings
{
	public CustomBlend[] m_CustomBlends = null;

	public AnimationCurve GetBlendCurveForVirtualCameras(VirtualCameraBase fromCamera, AnimationCurve defaultCurve)
	{
		if (m_CustomBlends != null)
		{
			for (int i = 0; i < m_CustomBlends.Length; ++i)
			{
				CustomBlend blendParams = m_CustomBlends[i];

				if (blendParams.m_FromCamera == null)
					continue;

				if (blendParams.m_FromCamera == fromCamera)
				{
					return blendParams.BlendCurve;
				}
			}
		}

		return defaultCurve;
	}

	[Serializable]
	public struct CustomBlend
	{
		public string name;
		public VirtualCameraBase m_FromCamera;
		public AnimationCurve BlendCurve;
	}
}

