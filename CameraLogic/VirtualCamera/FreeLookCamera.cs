using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : VirtualCameraBase
{
	#region 相机信息设置
	public Vector3 NormalOffset = Vector3.zero;
	public Vector3 NormalUpOffset = Vector3.zero;
	public Vector3 NormalDownOffset = Vector3.zero;

	public float xMinLimit = -20;
	public float xMaxLimit = 50;
	public float yMinLimit = -360;
	public float yMaxLimit = 360;
	public float angleX = 10;
	public float angleY = 180;

	public bool userSelfAngle = false;

	public float autoCalibrationLerpSpeed = 30;
	public float autoCalibrationTime = 1f;
	private Vector3 TargetOffset
	{
		get
		{
			if (angleX > 0)
			{
				return Vector3.Lerp(NormalOffset, NormalUpOffset, angleX / xMaxLimit);
			}
			else if (angleX < 0)
			{
				return Vector3.Lerp(NormalOffset, NormalDownOffset, angleX / xMinLimit);
			}
			else
			{
				return NormalOffset;
			}
		}
		set
		{

			NormalOffset = value;
		}
	}
	#endregion

	public void ResetAngel()
	{
		if (Target == null)
			return;
		Quaternion q = Quaternion.LookRotation(Target.transform.forward);
		angleX = q.eulerAngles.x;
		angleY = q.eulerAngles.y;
	}
	protected override void Start()
	{
		base.Start();

		//默认背朝
		if (Target != null)
		{
			angleY = Target.eulerAngles.y;
		}		
	}

	protected override void Update()
	{
		base.Update();
		if(InAutoCalibration)
		{
			if (tempAutoCalibrationTime > 0)
			{
				tempAutoCalibrationTime -= TimeMgr.unscaledDeltaTime;
			}
			else
			{
				InAutoCalibration = false;
				FightingRoom.SelfInput.fireAutoCalibration = false;
			}
		}
	}
	private void InputUpdate()
	{
		if (IsLive)
		{
			UpdateInput();
			UpdateGyroscope();
			UpdateTrans();
		}
	}

	public override void UpdateCameraInfo(Vector3 worldUp)
	{
		InputUpdate();
		base.UpdateCameraInfo(worldUp);
	}

	public override void OnTransitionFromCamera(VirtualCameraBase fromCam, Vector3 worldUp)
	{
		base.OnTransitionFromCamera(fromCam, worldUp);

		if(!userSelfAngle)
		{
			if (fromCam is FreeLookCamera)
			{
				Quaternion q = Quaternion.LookRotation(fromCam.transform.forward);
				angleX = q.eulerAngles.x;
				angleY = q.eulerAngles.y;
			}
		}

		UpdateCameraInfo(worldUp);
	}

	void UpdateInput()
	{
		#region 编辑器鼠标控制
#if UNITY_EDITOR
		if (!Cursor.visible)
		{
			angleY += Input.GetAxis("Mouse X") * ViewCtr.Instance.GetRotaSpeed();
			angleX = Mathf.Clamp(angleX - Input.GetAxis("Mouse Y") * ViewCtr.Instance.GetRotaSpeed(), xMinLimit, xMaxLimit);
		}

#endif
		#endregion

		#region 正式的UI触摸控制
		if (ViewCtr.IsMoving)
		{
			FightingRoom.SelfInput.fireAutoCalibration = false;
			nowSpeed = ViewCtr.Instance.GetRotaSpeed();
			angleX -= ViewCtr.Instance.moveDeltaY * nowSpeed;
			angleY += ViewCtr.Instance.moveDeltaX * nowSpeed;
		}

		//自动校准
		if (FightingRoom.SelfInput.fireAutoCalibration)
		{
			if (!InAutoCalibration)
				InAutoCalibration = true;

			if (UI_TestFight.Instance != null && UI_TestFight.Instance.lastAimTarget != null)
			{
				Transform target = UI_TestFight.Instance.lastAimTarget.transform;

				Quaternion tq = Quaternion.LookRotation(target.position - transform.position);

				Quaternion temp = Quaternion.Lerp(transform.rotation, tq, TimeMgr.unscaledDeltaTime * autoCalibrationLerpSpeed);

				angleX = temp.eulerAngles.x;
				angleY = temp.eulerAngles.y;
			}
		}
		//角度检查
		if (angleX > 180) angleX = angleX - 360;
		angleX = Mathf.Clamp(angleX, xMinLimit, xMaxLimit);
		angleY = Utils.CheckAngle(angleY);
		angleY = Mathf.Clamp(angleY, yMinLimit, yMaxLimit);
		#endregion
	}

	#region 自动校准时间
	float tempAutoCalibrationTime;
	bool _InAutoCalibration;
	bool InAutoCalibration
	{
		get
		{
			return _InAutoCalibration;
		}
		set
		{
			_InAutoCalibration = value;
			if (value)
			{
				tempAutoCalibrationTime = autoCalibrationTime;
			}
		}
	}
	#endregion

	float nowSpeed =1;
	void UpdateTrans()
	{
		if (Target == null)
			return;
		transform.rotation = Quaternion.Euler(angleX, angleY, 0);

		var targetPos = Target.position + TargetOffset.x * transform.right;

		targetPos = targetPos + TargetOffset.y * transform.up;

		var cameraPos = targetPos + TargetOffset.z * transform.forward;

		transform.position = cameraPos;
	}

	#region GyroCtr

	/*// 屏蔽调试信息
	protected void OnGUI()
	{
		if (!IsLive)
			return;
		GUILayout.Label("Flag: " + IsGtroCtr + " " + isSaveGyro);;
		GUILayout.Label("angleX: " + angleX);
		GUILayout.Label("angleY: " + angleY);;
		GUILayout.Label("rate"+Input.gyro.rotationRateUnbiased);
	}
	//*/

	bool IsGtroCtr
	{
		get
		{
			if (FightingRoom.SelfInput.isFire)
				return true;

			return false;
		}
	}

	void UpdateGyroscope()
	{
		if (!SystemInfo.supportsGyroscope)
			return;

		if (IsGtroCtr)
		{
			Vector3 offset = Input.gyro.rotationRateUnbiased * ViewCtr.Instance.GetGyroscopeSensitivity();

			angleX -= offset.x;
			angleY -= offset.y;

			//角度检查
			angleX = Mathf.Clamp(angleX, xMinLimit, xMaxLimit);
			angleY = Utils.CheckAngle(angleY);
		}
	}
	#endregion
}
