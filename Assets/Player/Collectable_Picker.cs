using UnityEngine;

public class Collectable_Picker : MonoBehaviour {
    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Collectable") {
            Destroy(collider.gameObject);
        }
    }
}
