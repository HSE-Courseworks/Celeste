using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float MoveSpeed = 5;

    private PlayerJump jump;
    private PlayerInput input;
    private Player player;
    private Indicators indicators;

    private Rigidbody2D rb;

    private SpriteRenderer sprite;

    void Start() {
        jump = GetComponent<PlayerJump>();

        input = GetComponent<PlayerInput>();

        player = GetComponent<Player>();

        indicators = GetComponent<Indicators>();

        sprite = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
    }

    public void Move() {
        rb.velocity = new Vector2(input.horizontalInput * MoveSpeed, rb.velocity.y);
    }

    private void PullUp() {
        if (indicators.tossAsideTimer > 0) {
            indicators.tossAsideTimer -= Time.deltaTime;
        }
        else {
            indicators.pullUp = false;

            rb.velocity = 20 * player.getPlayerDirection() + Vector2.up;
        }
    }

    public void Flip() {
        if (input.horizontalInput < 0) {
            sprite.flipX = true;
        }
        else if (input.horizontalInput > 0) {
            sprite.flipX = false;
        }
    }
}
