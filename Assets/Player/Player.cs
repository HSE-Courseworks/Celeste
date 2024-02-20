using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [HideInInspector] 
    public enum State {idle, walk, jump, fall, climbDown, climbUp, death, climbStatic, grab, wallJump, pullUp};

    [HideInInspector] 
    public State state;

    private float horizontalInput = 0;
    private float verticalInput = 0;

    [Header("Movement")]
    [SerializeField] private float MoveSpeed = 5;

    [Header("Climb")]
    [SerializeField] private float climbUpSpeed = 2;
    [SerializeField] private float climbDownSpeed = -5;
    [SerializeField] private float climbSlip = -2f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 7;
    [SerializeField] private float jumpBorder = .3f;
    [SerializeField] private float fallForce = 3;

    private float basicGravityScale;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Collider2D coll;

    [Header("Obstacle")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private string killerTag = "killer";

    [Header("Debug")]
    [SerializeField] private Text stateText;
    [SerializeField] private Text onGroundText;
    private string textState = "";
    private string textOnGround = "";

    private bool onWall;
    private bool onGround;
    private bool onPullUp;

    private bool jumpPressed;
    private bool grabPressed;
    private bool climbUp;
    private bool climbDown;

    private bool pullUp = false;

    private bool wallJump = false;

    [Header("Collision")]
    [SerializeField] private float collisionRadius;
    [SerializeField] private Vector2 bottomOffset;
    [SerializeField] private Vector2 rightOffset;
    [SerializeField] private Vector2 leftOffset;

    [Header("PullUp")]
    [SerializeField] private float tossAsideDelay;
    private float tossAsideTimer = 0f;

    [SerializeField] private Vector2 rightBottomOffset; 
    [SerializeField] private Vector2 rightMiddleOffset; 
    [SerializeField] private Vector2 leftBottomOffset; 
    [SerializeField] private Vector2 leftMiddleOffset; 

    [Header("Particles")]
    [SerializeField] private GameObject jumpSmoke;
    [SerializeField] private Vector3 jumpSmokeOffset; 

    [Header("Pseudo Parallax")]
    [SerializeField] private GameObject clouds;
    [SerializeField] private GameObject mountains; 
    [SerializeField] private GameObject grass;
    [SerializeField] private float cloudsMoveSpeed;
    [SerializeField] private float mountainsMoveSpeed;
    [SerializeField] private float grassMoveSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        basicGravityScale = rb.gravityScale;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);

        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) ||
            Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onPullUp = (!Physics2D.OverlapCircle((Vector2)transform.position + rightMiddleOffset, collisionRadius, groundLayer) &&
                    Physics2D.OverlapCircle((Vector2)transform.position + rightBottomOffset, collisionRadius, groundLayer)) 
                    ||
                    (!Physics2D.OverlapCircle((Vector2)transform.position + leftMiddleOffset, collisionRadius, groundLayer) &&
                    Physics2D.OverlapCircle((Vector2)transform.position + leftBottomOffset, collisionRadius, groundLayer));

        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        grabPressed = Input.GetKey(KeyCode.LeftControl);
        climbUp = Input.GetKey(KeyCode.UpArrow);
        climbDown = Input.GetKey(KeyCode.DownArrow);

        //Debug.Log("On ground: " + onGround);
        //Debug.Log("On wall: " + onWall);
        Debug.Log("On pull up: " + onPullUp);


        //Debug
        stateText.text = "State: " + textState;

        if (jumpPressed)
        {
            textOnGround = "True";
        }
        else
        {
            textOnGround = "False";
        }
        onGroundText.text = "OnGround: " + textOnGround;
    }

    void FixedUpdate()
    {
        state = getState();
        Move();
        Flip();

        PseudoParallax();
    }

    private State getState()
    {
        if (state == State.death)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            return State.death;
        }

        Debug.Log(jumpPressed);

        rb.gravityScale = basicGravityScale;   

        if (onPullUp && wallJump == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce / 1.4f);

            wallJump = true;
            return State.jump;
            //tossAsideTimer = tossAsideDelay;
        }

        if (onWall && jumpPressed && (state == State.grab || state == State.climbDown || state == State.climbUp))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            Jump();

            wallJump = true;

            textState = "WallJump";
            return State.wallJump;
        }

        if (onWall && grabPressed && !wallJump)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            if (climbUp)
            {
                rb.velocity = new Vector2(rb.velocity.x, climbUpSpeed);

                textState = "ClimbUp";
                return State.climbUp;
            }
            else if (climbDown)
            {
                rb.velocity = new Vector2(rb.velocity.x, climbDownSpeed);

                textState = "ClimbDown";
                return State.climbDown;
            }

            textState = "Grab";
            return State.grab;   
        }

        if (onGround && jumpPressed)
        {
            Jump();   
            spawnJumpSmoke();
            textState = "Jump";
            return State.jump;
        }

        if (rb.velocity.y < -jumpBorder)
        {
            textState = "Fall";

            wallJump = false;
            return State.fall;
        }

        if (rb.velocity.y > jumpBorder)
        {
            textState = "Jump";
            return State.jump;
        }

        if (rb.velocity.x != 0)
        {
            textState = "Walk";
            return State.walk;
        }

        textState = "Idle";
        return State.idle;
    }

    private void PseudoParallax()
    {
        clouds.transform.position += new Vector3(cloudsMoveSpeed * rb.velocity.x, 0, 0);
        mountains.transform.position += new Vector3(mountainsMoveSpeed * rb.velocity.x, 0, 0);
        grass.transform.position += new Vector3(grassMoveSpeed * rb.velocity.x, 0, 0);
    }

    private void PullUp()
    {
        if (tossAsideTimer > 0)
        {
            tossAsideTimer -= Time.deltaTime;
        }
        else
        {
            pullUp = false;

            rb.velocity = 20 * getPlayerDirection() + Vector2.up;
        }
    }

    private void Kill()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Move()
    {
        rb.velocity = new Vector2(horizontalInput * MoveSpeed, rb.velocity.y);
    }

    private void StopMoving()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

/*
    private bool onWall()
    {
        RaycastHit2D hit;
        Vector2 boxDirection = Vector2.right;
        if (sprite.flipX == true)
        {
            boxDirection = Vector2.left;
        }

        //hit = Physics2D.BoxCast(coll.bounds.center - coll.bounds.size / 4, coll.bounds.size / 2, 0, boxDirection, .1f, LayerMask.GetMask(groundLayer));      
        hit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0, boxDirection, .1f, LayerMask.GetMask(groundLayer));

        return hit.collider != null;
    }
*/
    void OnDrawGizmos()
    {
        //Gizmos.DrawCube(kek, lol);
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightBottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightMiddleOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftBottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftMiddleOffset, collisionRadius);
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0, Vector2.down, .05f, groundLayer);

        return hit.collider != null;
    }

    private void Flip()
    {
        if (horizontalInput < 0)
        {
            sprite.flipX = true;
        }
        else if (horizontalInput > 0)
        {
            sprite.flipX = false;
        }
    }

    private Vector2 getPlayerDirection()
    {
        if (sprite.flipX == true)
        {
            return Vector2.left;
        }

        return Vector2.right;
    }

    void OnCollisionEnter2D(Collision2D col) 
    {
        if (col.gameObject.tag == "Killer")
        {
            state = State.death;
        }
    }

    //Particles

    void spawnJumpSmoke()
    {
        Instantiate(jumpSmoke, transform.position + jumpSmokeOffset,  Quaternion.identity);
    }
}
