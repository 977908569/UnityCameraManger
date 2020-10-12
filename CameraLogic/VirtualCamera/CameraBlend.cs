using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraBlend
{
	public VirtualCameraBase camA;
	public VirtualCameraBase camB;
	public AnimationCurve blendCurve;

	public float Duration { get; set; }
	public float TimeInBlend { get; set; }

	public bool IsComplete { get { return TimeInBlend >= Duration; } }

	public CameraInfo CInfo { get { return CameraInfo.Lerp(camA.CInfo, camB.CInfo, BlendWeight); } }

	public float BlendWeight
	{
		get { return blendCurve != null ? blendCurve.Evaluate(TimeInBlend) : 0; }
	}

	public bool IsUse(VirtualCameraBase vcam)
	{
		if (vcam == camA || vcam == camB)
			return true;
		return false;
	}

	public bool IsValid
	{
		get { return (camA != null || camB != null); }
	}

	public CameraBlend(VirtualCameraBase a, VirtualCameraBase b, AnimationCurve curve, float duration, float t)
	{
		if (a == null || b == null)
			throw new ArgumentException("Blend cameras cannot be null");
		camA = a;
		camB = b;
		blendCurve = curve;
		TimeInBlend = t;
		Duration = duration;
	}

	public void UpdateCameraState(Vector3 worldUp)
	{
		TimeInBlend += TimeMgr.unscaledDeltaTime;
		camA.UpdateCameraInfo(worldUp);
		camB.UpdateCameraInfo(worldUp);
	}
}

[Serializable]
public struct CameraBlendDefinition
{
	public enum Style
	{
		/// <summary>Zero-length blend</summary>
		Cut,
		/// <summary>S-shaped curve, giving a gentle and smooth transition</summary>
		EaseInOut,
		/// <summary>Linear out of the outgoing shot, and easy into the incoming</summary>
		EaseIn,
		/// <summary>Easy out of the outgoing shot, and linear into the incoming</summary>
		EaseOut,
		/// <summary>Easy out of the outgoing, and hard into the incoming</summary>
		HardIn,
		/// <summary>Hard out of the outgoing, and easy into the incoming</summary>
		HardOut,
		/// <summary>Linear blend.  Mechanical-looking.</summary>
		Linear
	};

	public Style m_Style;

	public float m_Time;

	public CameraBlendDefinition(Style style, float time)
	{
		m_Style = style;
		m_Time = time;
	}

	public AnimationCurve BlendCurve
	{
		get
		{
			float time = Mathf.Max(0, m_Time);
			switch (m_Style)
			{
				default:
				case Style.Cut: return new AnimationCurve();
				case Style.EaseInOut: return AnimationCurve.EaseInOut(0f, 0f, time, 1f);
				case Style.EaseIn:
					{
						AnimationCurve curve = AnimationCurve.Linear(0f, 0f, time, 1f);
						Keyframe[] keys = curve.keys;
						keys[1].inTangent = 0;
						curve.keys = keys;
						return curve;
					}
				case Style.EaseOut:
					{
						AnimationCurve curve = AnimationCurve.Linear(0f, 0f, time, 1f);
						Keyframe[] keys = curve.keys;
						keys[0].outTangent = 0;
						curve.keys = keys;
						return curve;
					}
				case Style.HardIn:
					{
						AnimationCurve curve = AnimationCurve.Linear(0f, 0f, time, 1f);
						Keyframe[] keys = curve.keys;
						keys[0].outTangent = 0;
						keys[1].inTangent = 1.5708f; // pi/2 = up
						curve.keys = keys;
						return curve;
					}
				case Style.HardOut:
					{
						AnimationCurve curve = AnimationCurve.Linear(0f, 0f, time, 1f);
						Keyframe[] keys = curve.keys;
						keys[0].outTangent = 1.5708f; // pi/2 = up
						keys[1].inTangent = 0;
						curve.keys = keys;
						return curve;
					}
				case Style.Linear: return AnimationCurve.Linear(0f, 0f, time, 1f);
			}
		}
	}
}
