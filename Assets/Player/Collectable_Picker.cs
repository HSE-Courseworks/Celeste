using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable_Picker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Collectable")
        {
            Destroy(collider.gameObject);
        }
    }
}
