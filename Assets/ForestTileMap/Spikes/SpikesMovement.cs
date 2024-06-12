using System.Collections;
using UnityEngine;

public class SpikesMovement : MonoBehaviour {
    public Transform startPosition;
    private float speed = 2f;
    private float movementDistance = 2.5f;
    private float delayTime = 2f;

    private Vector3 initialPosition;
    private Vector3 endPosition;
    private bool isMovingToEnd = true;
    private bool isMovementStarted = false;
    // Start is called before the first frame update
    void Start() {
        Vector3 boxColliderSize = GetComponent<BoxCollider2D>().bounds.size;
        initialPosition = transform.position;
        endPosition = startPosition.position + Vector3.right * boxColliderSize.x * (isMovingToEnd ? 1f : -1f);
        isMovementStarted = true;
    }

    // Update is called once per frame
    void Update() {
        if (!isMovementStarted) {
            return;
        }

        MoveSpikes();
    }

    void MoveSpikes() {
        Vector3 targetPosition = isMovingToEnd ? endPosition : initialPosition;
        transform.position = Vector3.MoveTowards(
            transform.position, targetPosition, speed * Time.deltaTime
            );
        if (Vector3.Distance(transform.position, targetPosition) <= 0.01f) {
            SwitchDirection();
        }
    }

    void SwitchDirection() {
        isMovingToEnd = !isMovingToEnd;

        if (isMovingToEnd) {
            Vector3 boxColliderSize = GetComponent<BoxCollider2D>().bounds.size;
            endPosition = startPosition.position + Vector3.right * boxColliderSize.x * (isMovingToEnd ? 1f : -1f);

        }
        else {
            endPosition = initialPosition;
        }

        StartCoroutine(ContinueMovementWithDelay());
    }

    IEnumerator ContinueMovementWithDelay() {
        isMovementStarted = false;
        yield return new WaitForSeconds(delayTime);
        isMovementStarted = true;
    }
}
