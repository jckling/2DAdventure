using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class Sign : MonoBehaviour
{
    private Animator anim;
    private PlayerInputControl playerInputControl;

    public Transform playerTrans;
    public GameObject signSprite;

    private bool canPress;
    private IInteractive targetItem;

    private void Awake()
    {
        anim = signSprite.GetComponent<Animator>();

        playerInputControl = new PlayerInputControl();
        playerInputControl.Enable();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerInputControl.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        canPress = false;
    }

    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactive"))
        {
            canPress = true;
            targetItem = other.GetComponent<IInteractive>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange == InputActionChange.ActionStarted)
        {
            var d = ((InputAction)obj).activeControl.device;
            switch (d.device)
            {
                case Keyboard:
                    anim.Play("E");
                    break;
                case DualShockGamepad:
                    anim.Play("O");
                    break;
            }
        }
    }
}