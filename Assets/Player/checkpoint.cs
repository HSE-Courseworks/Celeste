using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour {
    public Transform player;
    public int index;
    void Awake() {
        player = GameObject.Find("Player").transform;

        if (StaticData.checkpoint_num == index) {
            player.position = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player") {
            StaticData.checkpoint_num = index;
        }
    }
}
