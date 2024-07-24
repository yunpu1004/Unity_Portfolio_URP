using UnityEngine;
using UnityEngine.InputSystem;

// 이 스크립트는 플레이어의 입력을 받습니다.
public class PlayerInput : MonoBehaviour
{
	public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool sprint;
	public bool attack;
	public bool interact;
	public bool tapMenu;
	public bool skillQ;
	public bool skillE;

	[Range(0.5f, 5f)] public float mouseSensitivity = 1f;

	public bool analogMovement;
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;


	public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		if(cursorInputForLook)
		{
			LookInput(value.Get<Vector2>());
		}
	}

	public void OnJump(InputValue value)
	{
		JumpInput(value.isPressed);
	}

	public void OnSprint(InputValue value)
	{
		SprintInput(value.isPressed);
	}

	public void OnAttack(InputValue value)
	{
		AttackInput(value.isPressed);
	}

	public void OnInteract(InputValue value)
	{
		InteractInput(value.isPressed);
	}

	public void OnTapMenu(InputValue value)
	{
		TapMenuInput(value.isPressed);
	}

	public void OnSkillQ(InputValue value)
	{
		skillQ = value.isPressed;
	}

	public void OnSkillE(InputValue value)
	{
		skillE = value.isPressed;
	}

	// 캐릭터의 이동, 시점, 점프, 달리기, 공격 입력을 무시합니다.
	public void IgnoreCharacterMovement()
	{
		move = Vector2.zero;
		look = Vector2.zero;
		jump = false;
		sprint = false;
		attack = false;
	}

	// 탭 메뉴 입력을 무시합니다.
	public void IgnoreTapMenu()
	{
		tapMenu = false;
	}

	// 상호작용 입력을 무시합니다.
	public void IgnoreInteract()
	{
		interact = false;
	}

	// Q 스킬 사용 입력을 무시합니다.
	public void IgnoreSkillQ()
	{
		skillQ = false;
	}

	// E 스킬 사용 입력을 무시합니다.
	public void IgnoreSkillE()
	{
		skillE = false;
	}

	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	} 

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection * mouseSensitivity;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}

	public void AttackInput(bool newAttackState)
	{
		attack = newAttackState;
	}

	public void InteractInput(bool newInteractState)
	{
		interact = newInteractState;
	}

	public void TapMenuInput(bool newTapMenuState)
	{
		tapMenu = newTapMenuState;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
	}

	public void SetCursorLock(bool newState)
	{
		if(cursorLocked == newState) return;
		cursorLocked = newState;
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}
