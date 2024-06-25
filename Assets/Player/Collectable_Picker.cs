using System;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Collections;

public class Collectable_Picker : MonoBehaviour {
    public CollectableStorage Storage;
    private int Score;
    [SerializeField] Text CollectableCounter;

    IEnumerator ShowCollectableCounter() {
        for (float f = 1f; f>=0; f -= 0.05f) {
            Color color = CollectableCounter.color;
            color.a = f;
            CollectableCounter.color = color;
            yield return new WaitForSeconds(0.05f);
        }
        Color endcolor = CollectableCounter.color;
        endcolor.a = 0f;
        CollectableCounter.color = endcolor;
    }

    public void StartShow() {
        StartCoroutine("ShowCollectableCounter");
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Collectable") {
            string index = collider.gameObject.name;
            int id = Convert.ToInt32(index);
            Destroy(collider.gameObject);
            if (Storage != null && CollectableCounter != null) {
                Storage.AddCollectable(id);
                Score = Storage.GetCollectableCount();
                CollectableCounter.text = 'x' + Score.ToString();
                StartShow();
            }
        }
    }
}
