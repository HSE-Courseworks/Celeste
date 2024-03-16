using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Collider2D coll;

    [Header("Obstacle")]
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] private string killerTag = "killer";

    [Header("Debug")]
    [SerializeField] private Text stateText;
    [SerializeField] private Text onGroundText;
    [SerializeField] private Text fatigueText;
    private string textState = "";
    private string textOnGround = "";
    private string textFatigue = "";

    private PlayerState playerState;
    private PlayerJump jump;
    private PlayerClimb climb;
    private PlayerDash dash;
    private Indicators indicators;
    private Fatigue fatigue;
    private PlayerMovement movement;
    private PlayerParticles particles;
    private PlayerInput input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        playerState = GetComponent<PlayerState>();
        jump = GetComponent<PlayerJump>();
        dash = GetComponent<PlayerDash>();
        climb = GetComponent<PlayerClimb>();
        indicators = GetComponent<Indicators>();
        fatigue = GetComponent<Fatigue>();
        movement = GetComponent<PlayerMovement>();
        particles = GetComponent<PlayerParticles>();
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        jump.timeCoyotize();
        jump.jumpBufferize();   

        //Debug
        debugState();
        stateText.text = "State: " + textState;

        //Debug.Log(jump.coyoteTimeCounter + " " + jump.jumpBufferCounter);

        /*
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
        */
    }
    void FixedUpdate()
    {
        playerState.state = playerState.getState();
        manageState();

        if (playerState.state != PlayerState.State.grab && 
            playerState.state != PlayerState.State.climbUp && 
            playerState.state != PlayerState.State.climbDown && 
            playerState.state != PlayerState.State.slip &&
            playerState.state != PlayerState.State.dash)
        {
            movement.Move();
        }
        
        if (playerState.state != PlayerState.State.grab && 
            playerState.state != PlayerState.State.climbUp && 
            playerState.state != PlayerState.State.climbDown && 
            playerState.state != PlayerState.State.slip)
        {
            movement.Flip();
        }

        if (indicators.landed)
        {
            particles.spawnLandingDust();
        }

        if (indicators.onGround)
        {
            dash.dashRefresh();
            fatigue.nullifyFatigue();
        }

        if (fatigue.fatigue >= fatigue.maxFatigue && !fatigue.isFlashing)
        {  
            StartCoroutine(fatigue.FlashPlayer());
        }

    }
    
    private void manageState()
    {
        switch(playerState.state)
        {
            case PlayerState.State.dash:
            {
                if (!dash.hasDashed)
                {
                    dash.DashStart();
                }

                break;
            }
            case PlayerState.State.death:
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;

                break;
            }
            case PlayerState.State.pullUp:
            {
                rb.gravityScale = jump.basicGravityScale;   

                jump.pullUpJump();
                indicators.wallJump = true;

                playerState.state = PlayerState.State.jump;
                break;
            }
            case PlayerState.State.jump:
            {
                rb.gravityScale = jump.basicGravityScale;   

                jump.Jump();
                particles.spawnJumpingDust();
                playerState.state = PlayerState.State.flight;
                                
                break;
            }
            case PlayerState.State.wallJump:
            {
                rb.gravityScale = jump.basicGravityScale;   

                rb.velocity = new Vector2(rb.velocity.x, 0f);
                jump.Jump();
                fatigue.JumpTick();

                indicators.wallJump = true;

                playerState.state = PlayerState.State.flight;
                break;
            }
            case PlayerState.State.slip:
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.gravityScale = 0;
                climb.Slip();

                break;
            }
            case PlayerState.State.climbUp:
            {
                rb.gravityScale = 0;

                climb.ClimbUp();
                fatigue.Tick();
                
                break;
            }
            case PlayerState.State.climbDown:
            {
                rb.gravityScale = 0;

                climb.ClimbDown();
                fatigue.Tick();

                break;
            }
            case PlayerState.State.grab:
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.gravityScale = 0;

                fatigue.Tick();
                
                break;
            }
            case PlayerState.State.fall:
            {
                rb.gravityScale = jump.basicGravityScale;   
                indicators.wallJump = false;

                break;
            }
            case PlayerState.State.flight:
            {
                rb.gravityScale = jump.basicGravityScale;   

                break;
            }
            case PlayerState.State.walk:
            {
                rb.gravityScale = jump.basicGravityScale;   

                break;
            }
            case PlayerState.State.idle:
            {
                rb.gravityScale = jump.basicGravityScale;   

                break;
            }
            default:
            {
                break;
            }
        }

    }

    private void Kill()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public Vector2 getPlayerDirection()
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
            playerState.state = PlayerState.State.death;
        }
    }

    void debugState()
    {
        switch(playerState.state)
        {
            case PlayerState.State.death:
            {
                textState = "death";

                break;
            }
            case PlayerState.State.pullUp:
            {
                textState = "pull up";

                break;
            }
            case PlayerState.State.wallJump:
            {
                textState = "wall jump";

                break;
            }
            case PlayerState.State.slip:
            {
                textState = "slip";

                break;
            }
            case PlayerState.State.climbUp:
            {
                textState = "climb up";
                
                break;
            }
            case PlayerState.State.climbDown:
            {
                textState = "climb down";

                break;
            }
            case PlayerState.State.grab:
            {
                textState = "grab";
                
                break;
            }
            case PlayerState.State.fall:
            {
                textState = "fall";

                break;
            }
            case PlayerState.State.jump:
            {
                textState = "jump";

                break;
            }
            case PlayerState.State.walk:
            {
                textState = "walk";

                break;
            }
            case PlayerState.State.flight:
            {
                textState = "flight";

                break;
            }
            case PlayerState.State.idle:
            {
                textState = "idle";

                break;
            }
            case PlayerState.State.dash:
            {
                textState = "dash";
                
                break;
            }
            default:
            {
                textState = "Empty";
                
                break;
            }
        }
    }
}
