using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
public class CameraPrioritySet : MonoBehaviour
{
	public static CameraPrioritySet Instance;

	public FreeLookCamera walkVC;
	public FreeLookCamera aimVC;
	public FreeLookCamera skillVC;
	public FreeLookCamera TTVC;
	public CGCamera cgVC;

	//
	VirtualCameraBase lastVC;
	private PlayerState _currentState= PlayerState.None;
	public PlayerState currentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			if (value != _currentState)
			{
				if (lastVC != null)
				{
					lastVC.Priority = 10;
				}
				switch (value)
				{
					case PlayerState.Walk:
						lastVC = walkVC;
						lastVC.Priority = 11;
						break;
					case PlayerState.Aim:
						lastVC = aimVC;
						lastVC.Priority = 11;
						break;
					case PlayerState.Skill:
						lastVC = skillVC;
						lastVC.Priority = 11;
						break;
					case PlayerState.TT:
						lastVC = TTVC;
						lastVC.Priority = 11;
						break;
					case PlayerState.CG:
						lastVC = cgVC;
						lastVC.Priority = 11;
						break;
					default:
						lastVC = walkVC;
						lastVC.Priority = 11;
						break;
				}
				_currentState = value;
			}
		}
	}

    private PlayerEntity self
    {
        get { return FightingRoom.Instance.SelfEntity; }
    }

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (self.isTT)
		{
			currentState = PlayerState.TT;
		}
		else if (self.plInput.isAim)
		{
			currentState = PlayerState.Aim;
		}
		else if (UI_TestFight.Instance != null && UI_TestFight.Instance.skillCtr.skillSearch)
		{
			currentState = PlayerState.Skill;
		}
		else if (self.isCG)
		{
			currentState = PlayerState.CG;
		}
		else
		{
			currentState = PlayerState.Walk;
		}
	}
}

public enum PlayerState
{
	None,
	Walk,
	Aim,
	Rush,
	Skill,
	TT,
	CG,
}

