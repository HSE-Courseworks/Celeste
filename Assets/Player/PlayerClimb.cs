using UnityEngine;

public class PlayerClimb : MonoBehaviour {
    [Header("Climb")]
    [SerializeField] private float climbUpSpeed = 2;
    [SerializeField] private float climbDownSpeed = -5;
    [SerializeField] private float climbSlip = -2f;

    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Slip() {
        rb.velocity = new Vector2(rb.velocity.x, climbSlip);
    }

    public void ClimbUp() {
        rb.velocity = new Vector2(rb.velocity.x, climbUpSpeed);
    }

    public void ClimbDown() {
        rb.velocity = new Vector2(rb.velocity.x, climbDownSpeed);
    }
}
