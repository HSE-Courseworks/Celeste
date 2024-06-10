using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour {
    [HideInInspector]
    public enum State { idle, walk, jump, fall, climbDown, climbUp, death, climbStatic, grab, wallJump, pullUp, slip, flight, dash };

    [HideInInspector]
    public State state;

    private Indicators indicators;
    private PlayerInput input;
    private Fatigue fatigue;
    private PlayerJump jump;
    private PlayerDash dash;

    private Rigidbody2D rb;

    void Start() {
        indicators = GetComponent<Indicators>();
        input = GetComponent<PlayerInput>();
        fatigue = GetComponent<Fatigue>();
        jump = GetComponent<PlayerJump>();
        dash = GetComponent<PlayerDash>();

        rb = GetComponent<Rigidbody2D>();
    }

    public State getState() {
        if (state == State.death) {
            return State.death;
        }

        if ((!dash.hasDashed && dash.dashCanBeDone()) || dash.isDashing) {
            return State.dash;
        }

        if (indicators.onPullUp && indicators.wallJump == false) {
            return State.pullUp;
        }

        if (indicators.onWall && input.jumpPressed && (state == State.grab || state == State.climbDown || state == State.climbUp || state == State.slip)) {
            return State.wallJump;
        }

        if (indicators.onWall && input.grabPressed && !indicators.wallJump) {
            if (fatigue.fatigue >= fatigue.maxFatigue) {
                return State.slip;
            }
            else if (input.verticalInput > 0f) {
                return State.climbUp;
            }
            else if (input.verticalInput < 0f) {
                return State.climbDown;
            }

            return State.grab;
        }

        if ((jump.coyoteTimeCounter > 0f && jump.jumpBufferCounter > 0f) &&
            state != State.flight) {
            return State.jump;
        }

        if (rb.velocity.y < -jump.jumpBorder) {
            return State.fall;
        }

        if (rb.velocity.y > jump.jumpBorder) {
            return State.flight;
        }

        if (rb.velocity.x != 0) {
            return State.walk;
        }

        return State.idle;
    }
}
