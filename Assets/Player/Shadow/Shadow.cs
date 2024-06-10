using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shadow : MonoBehaviour {
    [SerializeField] private float fadingPower = 0.1f;
    [SerializeField] private float fadingSpeed = 1f;
    [SerializeField] private float fadingSlowerPower = 1f;

    [SerializeField] private float fadingSlowerBoard = 50f;
    private SpriteRenderer sprite;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();

        StartCoroutine(Fade());
    }

    public void Flip() {
        sprite.flipX = true;
    }

    IEnumerator Fade() {
        while (sprite.color.a > 0) {
            yield return new WaitForSeconds(fadingSpeed);

            Color newColor = sprite.color;

            if (sprite.color.a > fadingSlowerBoard) {
                newColor.a -= fadingPower;
            }
            else {
                newColor.a -= fadingSlowerPower;
            }

            sprite.color = newColor;
        }

        Object.Destroy(gameObject);
    }
}
