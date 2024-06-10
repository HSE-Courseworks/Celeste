using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust : MonoBehaviour {
    private Animator anim;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    public void dustDisappear() {
        Object.Destroy(gameObject);
    }

    public void playJumpingDustAnimation() {
        anim.Play("jumpingDust");
    }

    public void playLandingDustAnimation() {
        anim.Play("landingDust");
    }
}
