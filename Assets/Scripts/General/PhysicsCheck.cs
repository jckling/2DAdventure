using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    public LayerMask groundLayer;

    public bool manual;
    public bool isPlayer;
    private PlayerController playerController;
    private Rigidbody2D rb;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public Vector2 bottomOffset;

    public bool isGround;
    public float checkRadius;

    private CapsuleCollider2D coll;

    private void Awake()
    {
        if (!manual)
        {
            coll = GetComponent<CapsuleCollider2D>();
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }

        if (isPlayer)
        {
            playerController = GetComponent<PlayerController>();
            rb = GetComponent<Rigidbody2D>();
        }

        // isGround = true;
    }

    private void Update()
    {
        Check();
    }

    private void Check()
    {
        if (transform.localScale.x > 0)
        {
            if (onWall)
            {
                isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset,
                    checkRadius, groundLayer);
            }
            else
            {
                isGround = Physics2D.OverlapCircle(
                    new Vector2(transform.position.x + bottomOffset.x, transform.position.y + 0),
                    checkRadius, groundLayer);
            }

            touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset,
                checkRadius, groundLayer);
            touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset,
                checkRadius, groundLayer);
        }
        else
        {
            if (onWall)
            {
                isGround = Physics2D.OverlapCircle(
                    new Vector2(transform.position.x - bottomOffset.x, transform.position.y + bottomOffset.y),
                    checkRadius, groundLayer);
            }
            else
            {
                isGround = Physics2D.OverlapCircle(
                    new Vector2(transform.position.x - bottomOffset.x, transform.position.y + 0),
                    checkRadius, groundLayer);
            }

            touchRightWall = Physics2D.OverlapCircle(
                new Vector2(transform.position.x - leftOffset.x, transform.position.y + leftOffset.y),
                checkRadius, groundLayer);
            touchLeftWall = Physics2D.OverlapCircle(
                new Vector2(transform.position.x - rightOffset.x, transform.position.y + rightOffset.y),
                checkRadius, groundLayer);
        }

        if (isPlayer)
        {
            onWall = (touchLeftWall && playerController.inputDirection.x < 0f ||
                      touchRightWall && playerController.inputDirection.x > 0f) &&
                     rb.velocity.y < 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (transform.localScale.x > 0)
        {
            Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(
                new Vector2(transform.position.x - bottomOffset.x, transform.position.y + bottomOffset.y),
                checkRadius);
            Gizmos.DrawWireSphere(
                new Vector2(transform.position.x - leftOffset.x, transform.position.y + leftOffset.y),
                checkRadius);
            Gizmos.DrawWireSphere(
                new Vector2(transform.position.x - rightOffset.x, transform.position.y + rightOffset.y),
                checkRadius);
        }
    }
}