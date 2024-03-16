using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fatigue : MonoBehaviour
{
    [Header("Fatigue")]

    public float fatigue = 0;
    [SerializeField] public float maxFatigue = 10f;

    private Color playerColor;
    public bool isFlashing = false;
    [SerializeField] private float flashingFrequency = 0.2f;

    [SerializeField] public float fatigueTick = 1f;
    [SerializeField] public float fatigueJumpTick = 2.5f;

    private SpriteRenderer sprite;

    private RumbleManager rumble;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();      
        playerColor = sprite.color;
        rumble = GetComponent<RumbleManager>();
    }
    
    public IEnumerator FlashPlayer()
    {
        isFlashing = true;
        rumble.startFatigueRumble();

        rumble.startFatigueRumble();

        while(fatigue >= maxFatigue)
        {
            if (sprite.color == playerColor)
            {
                sprite.color = Color.red;
            }
            else
            {
                sprite.color = playerColor;
            }

            if (rumble.isRumbling == false) 
            {
                rumble.startFatigueRumble();
            }

            yield return new WaitForSeconds(flashingFrequency);
        }

        isFlashing = false;
        rumble.endFatigueRumble();
        sprite.color = playerColor;

        rumble.endFatigueRumble();

        yield break;
    }

    public void Tick()
    {
        fatigue += fatigueTick * Time.deltaTime;
    }

    public void JumpTick()
    {
        fatigue += fatigueJumpTick;
    }

    public void nullifyFatigue()
    {
        fatigue = 0f;
    }
}
