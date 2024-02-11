using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesMovement : MonoBehaviour
{
    public Transform startPosition;
    public float speed = 2f;

    private Vector3 initialPosition;
    private Vector3 endPosition;
    private bool movingToEnd = true;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        endPosition = new Vector3(startPosition.position.x + 2.5f, startPosition.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        MoveSpikes();
    }

    void MoveSpikes()
    {
        Vector3 targetPosition = movingToEnd ? endPosition : initialPosition;
        transform.position = Vector3.MoveTowards(
            transform.position, targetPosition, speed * Time.deltaTime
            );
        if (transform.position == targetPosition)
        {
            StartCoroutine(DelayBeforeSwitchDirection());
        }
    }

    void SwitchDirection()
    {
        movingToEnd = !movingToEnd;
    }

    IEnumerator DelayBeforeSwitchDirection()
    {
        yield return new WaitForSeconds(3f);
        SwitchDirection();
    }
}
