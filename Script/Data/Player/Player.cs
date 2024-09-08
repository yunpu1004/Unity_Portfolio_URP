using UnityEngine;


// 이 스크립트는 입력으로부터 플레이어의 이동, 점프, 공격, 상호작용 등을 제어합니다.
public class Player : MonoBehaviour
{
    [Header("플레이어")]
    public float moveSpeed = 2.0f;
    public float sprintSpeed = 5.335f;

    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.12f;
    public float speedChangeRate = 10.0f;

    public AudioClip landingAudioClip;
    public AudioClip[] footstepAudioClips;
    public AudioClip[] slashAudioClips;

    [Range(0, 1)] 
    public float volume = 0.5f;

    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;
    public float jumpTimeout = 0.50f;
    public float fallTimeout = 0.15f;

    [Header("착지 판정")]
    public bool grounded = true;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;

    [Header("시네머신")]
    public GameObject cinemachineCameraTarget;
    public float topClamp = 70.0f;
    public float bottomClamp = -30.0f;
    public float cameraAngleOverride = 0.0f;
    public bool lockCameraPosition = false;

    // 시네머신 카메라
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    // 플레이어 이동
    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    // 타임아웃
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    // 애니메이션 ID
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    // 공격 애니메이션
    public bool onAttackAnimation { get; private set;} = false;

    // 플레이어 리스폰 지점 (초기 위치와 동일)
    private Vector3 respawnPoint;

    private Animator animator;
    private CharacterController controller;
    private PlayerInput input;
    private GameObject mainCamera;
    private PlayerStat playerStat;
    private MenuUI menuUI;
    private Weapon weapon;
    private Skill skill;
    private DialogueUI dialogueUI;
    private TradeUI tradeUI;
    private InventoryUI inventoryUI;

    private const float threshold = 0.01f;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        weapon = GetComponentInChildren<Weapon>();
        menuUI = FindObjectOfType<MenuUI>(true);
        playerStat = GetComponent<PlayerStat>();
        dialogueUI = FindObjectOfType<DialogueUI>(true);
        tradeUI = FindObjectOfType<TradeUI>(true);
        inventoryUI = FindObjectOfType<InventoryUI>(true);
        skill = GetComponent<Skill>();
        cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;
        respawnPoint = transform.position;
    }

    // 플레이어의 키보드 및 마우스 입력을 받아 적절한 행동을 수행합니다.
    private void FixedUpdate()
    {
        CheckInput();
        JumpAndGravity();
        GroundedCheck();
        Move();
        Attack();
        Interact();
        TapMenu();
        Skill();
        CameraRotation();
    }

    // 플레이어가 특정한 행동을 하는 중이면 일부 입력을 무시합니다.
    private void CheckInput()
    {
        if(dialogueUI.onDialogue || tradeUI.onInteraction || inventoryUI.isInventoryOpen || menuUI.onTapMenuProgress || menuUI.onTapMenu || playerStat.GetHP() <= 0)
        {
            input.IgnoreCharacterMovement();
            input.IgnoreInteract();
            input.IgnoreTapMenu();
            input.IgnoreSkillE();
            input.IgnoreSkillQ();
        }

        if(!skill.IsSkillAvailable() || input.jump || (input.skillE && input.skillQ))
        {
            input.IgnoreSkillE();
            input.IgnoreSkillQ();
        }

        if(skill.IsSkillEAnimationPlaying() || skill.IsSkillQAnimationPlaying())
        {
            input.IgnoreCharacterMovement();
            input.IgnoreInteract();
            input.IgnoreTapMenu();
        }

        if(!grounded)
        {
            input.IgnoreInteract();
            input.IgnoreTapMenu();
            input.IgnoreSkillE();
            input.IgnoreSkillQ();
        }

        if(input.interact && input.tapMenu) input.IgnoreInteract();
    }

    // 플레이어가 땅에 닿아있는지 확인합니다.
    private void GroundedCheck()
    {
        // GroundedOffset 위치를 중심으로 반지름 GroundedRadius 만큼의 구체 안에 GroundLayers에 해당하는 오브젝트가 있는지 확인합니다.
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        // 애니메이터의 Grounded를 설정합니다.
        animator.SetBool(animIDGrounded, grounded);
    }

    // 마우스 이동에 따라 카메라를 회전시킵니다.
    private void CameraRotation()
    {
        // 마우스 입력을 받아 시네머신 카메라의 회전값을 변경합니다.
        if (input.look.sqrMagnitude >= threshold && !lockCameraPosition)
        {
            cinemachineTargetYaw += input.look.x;
            cinemachineTargetPitch += input.look.y;
        }

        // 회전 각도를 정해진 범위 사이로 제한합니다.
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

        // 시네머신 카메라의 회전값을 적용합니다.
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    // 각도를 제한합니다.
    private float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360f) angle += 360f;
        while (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    // 플레이어를 이동시킵니다.
    private void Move()
    {
        if(onAttackAnimation) return;
        
        // 입력과 달리기 상태에 따라 목표 속도를 결정합니다.
        float targetSpeed = input.sprint ? sprintSpeed : moveSpeed;
        if (input.move == Vector2.zero) targetSpeed = 0.0f;

        // 목표 속도에 도달할 때까지 현재 속도를 부드럽게 변경합니다.
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        // 현재 속도가 목표 속도와 일치할 때까지 부드럽게 변경합니다.
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        // 입력 방향과 회전에 따라 플레이어를 이동시킵니다.
        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
        if (input.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        controller.Move((targetDirection.normalized * speed + new Vector3(0.0f, verticalVelocity, 0.0f)) * Time.deltaTime);

        // 애니메이터에 속도를 전달합니다.
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;
        animator.SetFloat(animIDSpeed, animationBlend);
        animator.SetFloat(animIDMotionSpeed, inputMagnitude);
    }

    // 점프와 중력을 적용합니다.
    private void JumpAndGravity()
    {
        if (grounded)
        {
            // 땅에 닿아있을 때, 낙하 타임아웃을 초기화합니다.
            fallTimeoutDelta = fallTimeout;

            // 애니메이터에 점프와 낙하상태를 false로 전달합니다.
            animator.SetBool(animIDJump, false);
            animator.SetBool(animIDFreeFall, false);

            // 캐릭터가 경사면에서 미끄러지는 것을 방지하기 위해, 작은 양의 중력을 적용합니다.
            if (verticalVelocity < 0.0f) verticalVelocity = -2f;

            // 땅에 닿아있는 상태에서 점프 입력이 들어오면 수직 속도에 점프력을 적용합니다.
            if (input.jump && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool(animIDJump, true);
            }

            if (jumpTimeoutDelta >= 0.0f) jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            // 땅에 닿아있지 않을 때, 점프 타임아웃을 초기화합니다.
            jumpTimeoutDelta = jumpTimeout;
            if (fallTimeoutDelta >= 0.0f) fallTimeoutDelta -= Time.deltaTime;
            else animator.SetBool(animIDFreeFall, true);

            // 공중에 떠있는 상태에서 점프 입력이 들어오면 이를 무시합니다.
            input.jump = false;
        }

        // 종단속도에 도달할 때까지 중력에 의한 낙하속도를 증가시킵니다. 
        if (verticalVelocity < terminalVelocity) verticalVelocity += gravity * Time.deltaTime;
    }

    // 플레이어가 공격합니다.
    private void Attack()
    {
        if(!input.attack) return;
        input.attack = false;
        if(!grounded) return;
        animator.SetTrigger("Attack");
    }

    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void AttackStart()
    {
        onAttackAnimation = true;
    }

    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void AttackEnd()
    {
        onAttackAnimation = false;
        animator.ResetTrigger("Attack");
    }

    // 플레이어가 가장 가까운 상호작용 오브젝트와 상호작용합니다.
    private void Interact()
    {
        if(!input.interact) return;
        input.interact = false;
        Interaction.ExecuteNearestInteraction();
    }

    // 플레이어가 메뉴를 엽니다.
    private void TapMenu()
    {
        menuUI.ProgressTapMenu();
        if(!input.tapMenu) return;
        input.tapMenu = false;
        if(menuUI.onTapMenu) menuUI.HideTapMenu();
        else menuUI.ShowTapMenu();
    }

    // 플레이어가 스킬을 사용합니다.
    private void Skill()
    {
        if(input.skillQ)
        {
            skill.UseSkillQ();
            input.skillQ = false;
        }
        
        if(input.skillE)
        {
            skill.UseSkillE();
            input.skillE = false;
        }

        skill.UpdateSkillCooltime();
    }


    // 착지 판정 영역을 그립니다.
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // 선택시, GroundedOffset을 기준으로 GroundedRadius 만큼의 구체를 그립니다.
        // 이 구체의 영역이 GroundLayers에 해당하는 레이어와 충돌하면 Grounded를 true로 설정합니다.
        var pos = transform.position - (Vector3.up * groundedOffset);
        Gizmos.DrawSphere(pos, groundedRadius);
    }

    /// 이 메소드는 애니메이션 이벤트로 호출됨
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(controller.center), volume);
            }
        }
    }

    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(controller.center), volume);
        }
    }

    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void OnSlash(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            var index = Random.Range(0, slashAudioClips.Length);
            AudioSource.PlayClipAtPoint(slashAudioClips[index], transform.TransformPoint(controller.center));
        }
    }

    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void ActivateWeapon()
    {
        weapon.ActivateWeapon();
    }

    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void DeactivateWeapon()
    {
        weapon.DeactivateWeapon();
    }

    /// 이 메소드는 애니메이션 이벤트로 호출됨
    private void Respawn()
    {
        controller.enabled = false;
        transform.position = respawnPoint;
        controller.enabled = true;
        playerStat.RestoreFullHP();
    }
}