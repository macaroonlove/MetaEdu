using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputPress : MonoBehaviour
{
	[Header("플레이어 이동")]
	public bool eye;
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool run;
    public bool interact;

	public bool analogMovement;

	[Header("마우스 잠금")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
	public static CursorLockMode CLM;

    #region 키가 눌렸는가?
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

	public void OnInteract(InputValue value)
    {
		InteractInput(value.isPressed);
    }
	#endregion

	#region 키에대한 로직
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

	public void InteractInput(bool newInteractState)
    {
		interact = newInteractState;
		Invoke(nameof(initButton), 0.5f);
	}

	void initButton()
    {
		interact = false;
	}
	#endregion

	#region 마우스 OnOff (준비중)
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
