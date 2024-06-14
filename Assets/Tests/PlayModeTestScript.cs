using NUnit.Framework;
using NSubstitute;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;

public class PlayModeTestScript : MonoBehaviour {

    private GameObject playerObject;
    private Rigidbody2D rb;

    private PlayerJump playerJump;
    private PlayerClimb playerClimb;
    private PlayerDash playerDash;
    private Fatigue playerFatigue;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;

    private GameObject collectable;
    private Collectable_Picker collectablePicker;

    private DialogueManager dialogueManager;
    private Dialogue dialogue;

    private Menu menu;

    private SpriteRenderer spriteRender;


    [UnitySetUp]
    public IEnumerator LoadScene() {
        SceneManager.LoadScene("Level1");
        yield return new WaitForSeconds(1f);

        playerObject = GameObject.FindGameObjectWithTag("Player");

        yield return new WaitForSeconds(1f);

        rb = playerObject.GetComponent<Rigidbody2D>();
        playerJump = playerObject.GetComponent<PlayerJump>();
        playerClimb = playerObject.GetComponent<PlayerClimb>();
        playerDash = playerObject.GetComponent<PlayerDash>();
        playerInput = playerObject.GetComponent<PlayerInput>();
        playerFatigue = playerObject.GetComponent<Fatigue>();
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        spriteRender = playerObject.GetComponent<SpriteRenderer>();


        collectable = GameObject.Find("Collectable");
        collectablePicker = GameObject.FindObjectOfType<Collectable_Picker>();

        dialogueManager = GameObject.FindObjectOfType<DialogueManager>();
        dialogue = new Dialogue();
        dialogue.name = "Dialogue1";
        dialogue.sentences = new string[] { "Hello, world!" };

        menu = GameObject.FindObjectOfType<Menu>();

    }

    [UnityTest]
    public IEnumerator TestPlayerMove() {
        var initialVelocity = rb.velocity;
        playerMovement.Move();

        initialVelocity = new Vector2(playerInput.horizontalInput * 10, initialVelocity.y);

        Assert.AreEqual(initialVelocity.x, rb.velocity.x);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerFlip() {
        playerInput.horizontalInput = -1f;

        playerMovement.Flip();

        Assert.IsTrue(spriteRender.flipX);

        playerInput.horizontalInput = 1f;

        playerMovement.Flip();

        Assert.IsFalse(spriteRender.flipX);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerJumpAppliesForce() {
        float initialVelocity = rb.velocity.y;
        playerJump.Jump();

        Assert.IsTrue(rb.velocity.y > initialVelocity);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerJumpCanPullUp() {
        var initialVelocity = rb.velocity;
        playerJump.pullUpJump();
        initialVelocity = new Vector2(initialVelocity.x, 5);

        Assert.AreEqual(initialVelocity.y, rb.velocity.y, 1);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerJumpNullifyGravity() {
        playerJump.nullifyGravity();

        Assert.AreEqual(0, rb.gravityScale);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerDash() {
        playerInput.horizontalInput = 1.0f;
        playerInput.verticalInput = 0.5f;

        var horizontalBufferField = typeof(PlayerDash).GetField("horizontalBuffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var verticalBufferField = typeof(PlayerDash).GetField("verticalBuffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var dashPowerField = typeof(PlayerDash).GetField("dashPower", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        horizontalBufferField.SetValue(playerDash, playerInput.horizontalInput);
        verticalBufferField.SetValue(playerDash, playerInput.verticalInput);
        dashPowerField.SetValue(playerDash, 20f);

        playerDash.Dash();

        Vector2 expectedVelocity = new Vector2(playerInput.horizontalInput, playerInput.verticalInput).normalized * 20f;
        Vector2 actualVelocity = rb.velocity;

        Assert.AreEqual(expectedVelocity.x, actualVelocity.x, 0.01f);
        Assert.AreEqual(expectedVelocity.y, actualVelocity.y, 0.01f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerFatigueTick() {
        float initialFatigue = playerFatigue.fatigue;

        playerFatigue.fatigueTick = 1f;
        playerFatigue.Tick();

        float expectedFatigue = initialFatigue + playerFatigue.fatigueTick * Time.deltaTime;

        Assert.AreEqual(expectedFatigue, playerFatigue.fatigue, 0.01f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerFatigueJumpTick() {
        float initialFatigue = playerFatigue.fatigue;
        playerFatigue.JumpTick();

        float expectedFatigue = initialFatigue + playerFatigue.fatigueJumpTick;

        Assert.AreEqual(expectedFatigue, playerFatigue.fatigue, 0.01f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerFatigueNullify() {
        playerFatigue.fatigue = 5f;
        playerFatigue.nullifyFatigue();

        Assert.AreEqual(0f, playerFatigue.fatigue, 0.01f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerClimbSlip() {
        var initialVelocity = rb.velocity;
        float climbSlip = -13f;
        initialVelocity = new Vector2(initialVelocity.x, climbSlip);
        playerClimb.Slip();

        Assert.AreEqual(initialVelocity.y, rb.velocity.y, 1f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerClimbUp() {
        var initialVelocity = rb.velocity;
        float climbUpSpeed = 5f;
        initialVelocity = new Vector2(initialVelocity.x, climbUpSpeed);
        playerClimb.ClimbUp();

        Assert.AreEqual(initialVelocity.y, rb.velocity.y, 1f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerClimbDown() {
        var initialVelocity = rb.velocity;
        float climbDownSpeed = -9f;
        initialVelocity = new Vector2(initialVelocity.x, climbDownSpeed);
        playerClimb.ClimbDown();

        Assert.AreEqual(initialVelocity.y, rb.velocity.y, 1f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestCollectablePicker() {
        Collider2D collectableCollider = collectable.GetComponent<Collider2D>();
        collectablePicker.OnTriggerEnter2D(collectableCollider);

        yield return null;
        Assert.IsTrue(collectable == null || collectable.Equals(null));
    }


    //[UnityTest]
    //public IEnumerator TestDialogueStarted() {
    //    Animator dialogueAnimator = dialogueManager.GetComponent<Animator>();
    //    dialogueManager.StartDialogue(dialogue);

    //    yield return null;
    //    Assert.IsTrue(dialogueAnimator.GetBool("dialogueOpen"));
    //}

    //[UnityTest]
    //public IEnumerator TestDialogueFinished() {
    //    Animator dialogueAnimator = dialogueManager.GetComponent<Animator>();
    //    dialogueManager.EndDialogue();

    //    yield return null;
    //    Assert.IsFalse(dialogueAnimator.GetBool("dialogueOpen"));
    //}

    //[UnityTest]
    //public IEnumerator TestMenuLoadScene() {
    //    menu.Play();

    //    yield return null;
    //    Assert.AreEqual(2, SceneManager.GetActiveScene().buildIndex);
    //}

    //[UnityTest]
    //public IEnumerator TestMenuExitApplication() {
    //    menu.Exit();

    //    yield return null;
    //    Assert.IsFalse(Application.isPlaying);
    //}
}
