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
    public bool interact;
	public bool phone;
	public bool esc = false;
	public bool analogMovement;

	[Header("���콺 ���")]
    public bool cursorLocked = true;
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
		Debug.Log(value.Get<Vector2>());
		LookInput(value.Get<Vector2>());
	}

	public void OnJump(InputValue value)
	{
		JumpInput(value.isPressed);
	}

	public void OnRun(InputValue value)
	{
        if (value.isPressed)
        {
			RunInput();
		}
	}

	public void OnInteract(InputValue value)
    {
		InteractInput(value.isPressed);
    }

	public void OnPhone(InputValue value)
    {
		PhoneInput();
    }

	public void OnSetting(InputValue value)
    {
		EscInput();
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

	public void RunInput()
	{
		run = run ? false : true;
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

	public void PhoneInput()
    {
		phone = phone ? false : true;
	}

	public void EscInput()
    {
		esc = esc ? false : true;
    }
	#endregion
}