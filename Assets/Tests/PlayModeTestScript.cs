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

    private GameObject collectablePicker;
    private Collectable_Picker collectablePickerComponent;

    private GameObject dialogueObject;
    private DialogueAnimator dialogueAnimator;

    private GameObject menuObject;
    private Menu menu;

    private SpriteRenderer spriteRender;

    [UnitySetUp]
    public IEnumerator LoadScene() {
        SceneManager.LoadScene("Level 1");
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


        collectablePicker = GameObject.Find("Collectable");
        collectablePickerComponent = collectablePicker.GetComponent<Collectable_Picker>();

        dialogueObject = new GameObject();
        dialogueAnimator = GameObject.FindObjectOfType<DialogueAnimator>();

        menuObject = new GameObject();
        menu = GameObject.FindObjectOfType<Menu>();

    }

    [UnityTest, Ignore("This is just an example")]
    public IEnumerator AssertExample() {
        yield return null;
        Assert.Equals(2 + 2, 4);
    }

    [UnityTest]
    public IEnumerator TestPlayerMove() {
        var initialVelocity = rb.velocity;
        playerMovement.Move();

        initialVelocity = new Vector2(playerInput.horizontalInput * 10, initialVelocity.y);

        yield return null;
        Assert.AreEqual(initialVelocity.x, rb.velocity.x);
    }

    [UnityTest]
    public IEnumerator TestPlayerFlip() {
        playerInput.horizontalInput = -1f;

        playerMovement.Flip();

        yield return null;
        Assert.IsTrue(spriteRender.flipX);

        playerInput.horizontalInput = 1f;

        playerMovement.Flip();

        yield return null;
        Assert.IsFalse(spriteRender.flipX);
    }

    [UnityTest]
    public IEnumerator TestPlayerJumpAppliesForce() {
        float initialVelocity = rb.velocity.y;
        playerJump.Jump();

        yield return null;
        Assert.IsTrue(rb.velocity.y > initialVelocity);
    }

    [UnityTest]
    public IEnumerator TestPlayerJumpCanPullUp() {
        var initialVelocity = rb.velocity;
        playerJump.pullUpJump();
        initialVelocity = new Vector2(initialVelocity.x, 5);

        yield return null;
        Assert.AreEqual(initialVelocity.y, rb.velocity.y, 1);
    }

    [UnityTest]
    public IEnumerator TestPlayerJumpNullifyGravity() {
        playerJump.nullifyGravity();

        yield return null;
        Assert.AreEqual(0, rb.gravityScale);
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

        yield return null;
        Vector2 expectedVelocity = new Vector2(playerInput.horizontalInput, playerInput.verticalInput).normalized * 20f;
        Vector2 actualVelocity = rb.velocity;

        Assert.AreEqual(expectedVelocity.x, actualVelocity.x, 0.01f);
        Assert.AreEqual(expectedVelocity.y, actualVelocity.y, 0.01f);
    }

    [UnityTest]
    public IEnumerator TestPlayerFatigueTick() {
        float initialFatigue = playerFatigue.fatigue;

        playerFatigue.fatigueTick = 1f;
        playerFatigue.Tick();

        float expectedFatigue = initialFatigue + playerFatigue.fatigueTick * Time.deltaTime;

        yield return null;
        Assert.AreEqual(expectedFatigue, playerFatigue.fatigue, 0.01f);
    }

    [UnityTest]
    public IEnumerator TestPlayerFatigueJumpTick() {
        float initialFatigue = playerFatigue.fatigue;
        playerFatigue.JumpTick();

        float expectedFatigue = initialFatigue + playerFatigue.fatigueJumpTick;

        yield return null;
        Assert.AreEqual(expectedFatigue, playerFatigue.fatigue, 0.01f);
    }

    [UnityTest]
    public IEnumerator TestPlayerFatigueNullify() {
        playerFatigue.fatigue = 5f;
        playerFatigue.nullifyFatigue();

        yield return null;
        Assert.AreEqual(0f, playerFatigue.fatigue, 0.01f);
    }

    [UnityTest]
    public IEnumerator TestPlayerClimbSlip() {
        yield return null;

        var initialVelocity = rb.velocity;
        float climbSlip = -13f;
        initialVelocity = new Vector2(initialVelocity.x, climbSlip);
        playerClimb.Slip();

        yield return null;
        Assert.AreEqual(initialVelocity.y, rb.velocity.y, 1f);
    }

    [UnityTest]
    public IEnumerator TestPlayerClimbUp() {
        var initialVelocity = rb.velocity;
        float climbUpSpeed = 5f;
        initialVelocity = new Vector2(initialVelocity.x, climbUpSpeed);
        playerClimb.ClimbUp();

        yield return null;
        Assert.AreEqual(initialVelocity.y, rb.velocity.y, 1f);
    }

    [UnityTest]
    public IEnumerator TestPlayerClimbDown() {
        var initialVelocity = rb.velocity;
        float climbDownSpeed = -9f;
        initialVelocity = new Vector2(initialVelocity.x, climbDownSpeed);
        playerClimb.ClimbDown();

        yield return null;
        Assert.AreEqual(initialVelocity.y, rb.velocity.y, 1f);
    }

    [UnityTest]
    public IEnumerator TestCollectablePicker() {
        Collider2D collectableCollider = collectablePicker.GetComponent<BoxCollider2D>();
        collectablePickerComponent.OnTriggerEnter2D(collectableCollider);

        yield return null;
        Assert.IsTrue(collectableCollider == null);
    }

    [UnityTest]
    public IEnumerator TestPlayerInputMove() {
        InputAction.CallbackContext context = new InputAction.CallbackContext();
        var action = Substitute.For<InputAction>();

        action.ReadValue<Vector2>().Returns(new Vector2(1f, 0f));
        context.action.Returns(action);

        playerInput.Move(context);

        yield return null;
        Assert.AreEqual(1f, playerInput.horizontalInput);
        Assert.AreEqual(0f, playerInput.verticalInput);
    }

    [UnityTest]
    public IEnumerator TestPlayerInputJump() {
        InputAction.CallbackContext context = new InputAction.CallbackContext();
        var action = Substitute.For<InputAction>();

        context.action.Returns(action);
        action.triggered.Returns(true);

        playerInput.Jump(context);

        yield return null;
        Assert.IsTrue(playerInput.jumpPressed);
    }

    [UnityTest]
    public IEnumerator TestPlayerInputDash() {
        InputAction.CallbackContext context = new InputAction.CallbackContext();
        var action = Substitute.For<InputAction>();

        context.action.Returns(action);
        action.triggered.Returns(true);

        playerInput.Dash(context);

        yield return null;
        Assert.IsTrue(playerInput.dashPressed);
    }

    [UnityTest]
    public IEnumerator TestPlayerInputGrab() {
        InputAction.CallbackContext context = new InputAction.CallbackContext();
        var action = Substitute.For<InputAction>();

        context.action.Returns(action);
        action.triggered.Returns(true);

        playerInput.Grab(context);

        yield return null;
        Assert.IsTrue(playerInput.grabPressed);
    }

    [UnityTest]
    public IEnumerator TestShouldOpenDialogue() {
        Animator animator = dialogueObject.AddComponent<Animator>();
        Collider2D collider = dialogueObject.AddComponent<BoxCollider2D>();

        dialogueAnimator.dialogueStartAnimator = animator;

        collider.isTrigger = true;
        dialogueAnimator.OnTriggerEnter2D(collider);

        yield return null;
        Assert.IsTrue(dialogueAnimator.playerIsClose);
        Assert.IsTrue(animator.GetBool("startOpen"));
    }

    [UnityTest]
    public IEnumerator TestShouldCloseDialogue() {
        Animator animator = dialogueObject.AddComponent<Animator>();
        Collider2D collider = dialogueObject.AddComponent<BoxCollider2D>();

        dialogueAnimator.dialogueStartAnimator = animator;

        collider.isTrigger = true;
        dialogueAnimator.OnTriggerExit2D(collider);

        yield return null;
        Assert.IsFalse(dialogueAnimator.playerIsClose);
        Assert.IsFalse(animator.GetBool("startOpen"));
    }

    [UnityTest]
    public IEnumerator TestMenuLoadScene() {
        menu.Play();

        yield return null;
        Assert.AreEqual(2, SceneManager.GetActiveScene().buildIndex);
    }

    [UnityTest]
    public IEnumerator TestMenuExitApplication() {
        menu.Exit();

        yield return null;
        Assert.IsFalse(Application.isPlaying);
    }
}
