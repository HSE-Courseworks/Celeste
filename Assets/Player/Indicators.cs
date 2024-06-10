using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicators : MonoBehaviour {
    [HideInInspector]
    public bool onWall;
    [HideInInspector]
    public bool onGround;
    [HideInInspector]
    public bool onPullUp;
    [HideInInspector]
    public bool landed;

    public bool pullUp = false;
    public bool wallJump = false;

    [Header("Collision detectors")]
    [SerializeField] private float collisionRadius;
    [SerializeField] private Vector2 bottomOffset;
    [SerializeField] private Vector2 rightOffset;
    [SerializeField] private Vector2 leftOffset;

    [Header("PullUp detectors")]
    [SerializeField] public float tossAsideDelay;
    public float tossAsideTimer = 0f;

    [SerializeField] private Vector2 rightBottomOffset;
    [SerializeField] private Vector2 rightMiddleOffset;
    [SerializeField] private Vector2 leftBottomOffset;
    [SerializeField] private Vector2 leftMiddleOffset;

    [Header("Base")]
    private Player player;

    void Start() {
        player = GetComponent<Player>();
    }

    void Update() {
        bool onGroundBeforeUpdate = onGround;

        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, player.groundLayer);

        landed = !onGroundBeforeUpdate && onGround;

        onWall = ((Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, player.groundLayer) &&
            player.getPlayerDirection() == Vector2.right) ||
            (Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, player.groundLayer) &&
            player.getPlayerDirection() == Vector2.left));

        onPullUp = (!Physics2D.OverlapCircle((Vector2)transform.position + rightMiddleOffset, collisionRadius, player.groundLayer) &&
                    Physics2D.OverlapCircle((Vector2)transform.position + rightBottomOffset, collisionRadius, player.groundLayer))
                    ||
                    (!Physics2D.OverlapCircle((Vector2)transform.position + leftMiddleOffset, collisionRadius, player.groundLayer) &&
                    Physics2D.OverlapCircle((Vector2)transform.position + leftBottomOffset, collisionRadius, player.groundLayer));
    }

    void OnDrawGizmos() {
        //Gizmos.DrawCube(kek, lol);
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightBottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightMiddleOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftBottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftMiddleOffset, collisionRadius);
    }
}
