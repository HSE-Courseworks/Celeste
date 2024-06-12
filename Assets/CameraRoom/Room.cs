using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField]
    private GameObject virtualCam;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !collision.isTrigger) {
            virtualCam.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !collision.isTrigger) {
            virtualCam.SetActive(false);
        }
    }
}
