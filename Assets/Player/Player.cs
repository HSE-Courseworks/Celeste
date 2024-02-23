using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    
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
    [SerializeField] private Text fatigueText;
    private string textState = "";
    private string textOnGround = "";
    private string textFatigue = "";

    private bool onWall;
    private bool onGround;
    private bool onPullUp;

    private bool landed;

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
    [SerializeField] private GameObject dustGameObject;
    [SerializeField] private Vector3 jumpingDustOffset; 
    [SerializeField] private Vector3 landingDustOffset; 

    private GameObject dust;

    [Header("Pseudo Parallax")]
    [SerializeField] private GameObject clouds;
    [SerializeField] private GameObject mountains; 
    [SerializeField] private GameObject grass;
    [SerializeField] private float cloudsMoveSpeed;
    [SerializeField] private float mountainsMoveSpeed;
    [SerializeField] private float grassMoveSpeed;

    [Header("Fatigue")]

    private float fatigue = 0;
    [SerializeField] private float maxFatigue = 10f;

    //
    private Color playerColor;
    bool isFlashing = false;
    [SerializeField] private float flashingFrequency = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        playerColor = sprite.color;

        basicGravityScale = rb.gravityScale;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        bool onGroundBeforeUpdate = onGround;
    
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);

        landed = !onGroundBeforeUpdate && onGround;

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


        timeCoyotize();
        jumpBufferize();



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

        fatigueText.text = "Fatigue: " + Math.Round(fatigue, 1) + "/" + maxFatigue;
    }

    IEnumerator flashPlayer()
    {
        isFlashing = true;

        while(fatigue >= maxFatigue)
        {
            if (sprite.color == playerColor)
            {
                sprite.color = Color.red;
            }
            else
            {
                sprite.color = playerColor;
            }

            yield return new WaitForSeconds(flashingFrequency);
        }

        isFlashing = false;
        sprite.color = playerColor;

        yield break;
    }

    void FixedUpdate()
    {
        state = getState();

        if (state != State.grab && state != State.climbUp && state != State.climbDown)
        {
            Move();
            Flip();
        }


        if (landed)
        {
            spawnLandingDust();
        }

        if (onGround)
        {
            fatigue = 0f;
        }

        if (fatigue >= maxFatigue && !isFlashing)
        {  
            StartCoroutine(flashPlayer());
        }

        //PseudoParallax();
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
            fatigue += 2.5f;

            wallJump = true;

            textState = "WallJump";
            return State.wallJump;
        }

        if (onWall && grabPressed && !wallJump)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            if (fatigue >= maxFatigue)
            {
                textState = "Slip";        

                rb.velocity = new Vector2(rb.velocity.x, climbSlip);    

                return State.climbDown;
            }
            else if (climbUp)
            {
                rb.velocity = new Vector2(rb.velocity.x, climbUpSpeed);

                textState = "ClimbUp";

                fatigue += Time.deltaTime;
                return State.climbUp;
            }
            else if (climbDown)
            {
                rb.velocity = new Vector2(rb.velocity.x, climbDownSpeed);

                textState = "ClimbDown";
        
                fatigue += Time.deltaTime;
                return State.climbDown;
            }

            textState = "Grab";

            fatigue += Time.deltaTime;
            return State.grab;   
        }

        if ((coyoteTimeCounter > 0f) && (jumpBufferCounter > 0f))
        {
            Jump();   
            spawnJumpingDust();

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


    private void timeCoyotize()
    {
        if (onGround)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void jumpBufferize()
    {
        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
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
        jumpBufferCounter = 0f;
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

    void spawnJumpingDust()
    {
        dust = Instantiate(dustGameObject, transform.position + jumpingDustOffset,  Quaternion.identity);
        dust.GetComponent<Dust>().playJumpingDustAnimation();
    }

    void spawnLandingDust()
    {
        dust = Instantiate(dustGameObject, transform.position + landingDustOffset,  Quaternion.identity);
        dust.GetComponent<Dust>().playLandingDustAnimation();
    }
}
