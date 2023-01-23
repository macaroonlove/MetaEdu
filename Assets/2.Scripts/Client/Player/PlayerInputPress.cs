using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputPress : MonoBehaviour
{
	[Header("�÷��̾� �̵�")]
	public bool eye;
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool run;

	public bool analogMovement;

	[Header("���콺 ���")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
	public static CursorLockMode CLM;

    #region Ű�� ���ȴ°�?
    public void OnEyesight(InputValue value)
    {
		EyeInput(value.isPressed);
    }

	public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		if (cursorInputForLook)
		{
			LookInput(value.Get<Vector2>());
		}
	}

	public void OnJump(InputValue value)
	{
		JumpInput(value.isPressed);
	}

	public void OnRun(InputValue value)
	{
		RunInput(value.isPressed);
	}
    #endregion

    #region Ű������ ����
	public void EyeInput(bool newEyeState)
    {
		eye = newEyeState;
	}

	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void RunInput(bool newRunState)
	{
		run = newRunState;
	}
	#endregion

	#region ���콺 OnOff (�غ���)
	private void OnCursor(InputValue value)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.None : CursorLockMode.Locked;
		CLM = Cursor.lockState;
		cursorLocked = !newState; 
	}
    #endregion
}
