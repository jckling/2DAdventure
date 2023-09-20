using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public SceneLoadEventSO sceneLoadEventSo;
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadGameEventSo;
    public VoidEventSO backToMenuEventSo;

    public PlayerInputControl inputControl;
    public Vector2 inputDirection;

    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;
    private Character character;

    private Vector2 originalOffset;
    private Vector2 originalSize;

    private float speed = 290;
    private float runSpeed;
    private float walkSpeed => speed / 2.5f;
    private float slideSpeed = 0.3f;
    private int slidePowerCost = 5;

    private float jumpForce = 16;
    private float wallJumpForce = 10;
    private float hurtForce = 8;
    private float slideDistance = 3f;

    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;

    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

        originalOffset = coll.offset;
        originalSize = coll.size;

        inputControl = new PlayerInputControl();
        inputControl.Enable();

        // 跳跃
        inputControl.Gameplay.Jump.started += Jump;

        // 步行
        runSpeed = speed;
        inputControl.Gameplay.WalkSwitch.performed += ctx =>
        {
            if (physicsCheck.isGround) speed = walkSpeed;
        };
        inputControl.Gameplay.WalkSwitch.canceled += ctx =>
        {
            if (physicsCheck.isGround) speed = runSpeed;
        };

        // 攻击
        inputControl.Gameplay.Attack.started += PlayerAttack;

        // 滑铲
        inputControl.Gameplay.Slide.started += Slide;
    }

    private void OnEnable()
    {
        sceneLoadEventSo.LoadRequestEvent += OnLoadRequestEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
        loadGameEventSo.OnEventRaised += OnLoadGameEvent;
        backToMenuEventSo.OnEventRaised += OnLoadGameEvent;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        sceneLoadEventSo.LoadRequestEvent -= OnLoadRequestEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadGameEventSo.OnEventRaised -= OnLoadGameEvent;
        backToMenuEventSo.OnEventRaised -= OnLoadGameEvent;
    }

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        CheckState();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isAttack) Move();
    }

    private void OnLoadRequestEvent(GameSceneSO sceneToGo, Vector3 posToGo, bool fade)
    {
        inputControl.Gameplay.Disable();
    }

    private void OnLoadGameEvent()
    {
        isDead = false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        inputControl.Gameplay.Enable();
    }

    private void Move()
    {
        // 移动
        if (!isCrouch && !wallJump)
        {
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        }

        // 面向
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }
        else if (inputDirection.x < 0)
        {
            faceDir = -1;
        }

        transform.localScale = new Vector3(faceDir, 1, 1);

        // 下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (isCrouch)
        {
            coll.offset = new Vector2(-0.05f, 0.85f);
            coll.size = new Vector2(0.7f, 1.7f);
        }
        else
        {
            coll.offset = originalOffset;
            coll.size = originalSize;
        }
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
        {
            GetComponent<AudioDefination>()?.PlayAudioClip();
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            // 打断滑铲
            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicsCheck.onWall)
        {
            GetComponent<AudioDefination>()?.PlayAudioClip();
            rb.AddForce(new Vector2(-inputDirection.x, 2f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        if (!physicsCheck.isGround) return;
        playerAnimation.PlayAttack();
        isAttack = true;
    }

    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;
            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localPosition.x,
                transform.position.y);

            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(TriggerSlide(targetPos));
            character.OnSlide(slidePowerCost);
        }
    }

    IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGround) break;
            if (physicsCheck.touchLeftWall && transform.localScale.x < 0 ||
                physicsCheck.touchRightWall && transform.localScale.x > 0)
            {
                isSlide = false;
                break;
            }

            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed,
                transform.position.y));
        } while (Mathf.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    #region UnityEvent

    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }

    #endregion

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
        rb.velocity = physicsCheck.onWall
            ? new Vector2(rb.velocity.x, rb.velocity.y / 2f)
            : new Vector2(rb.velocity.x, rb.velocity.y);

        if (wallJump && rb.velocity.y < 0f) wallJump = false;

        if (isDead || isSlide) gameObject.layer = LayerMask.NameToLayer("Enemy");
        else gameObject.layer = LayerMask.NameToLayer("Player");
    }
}