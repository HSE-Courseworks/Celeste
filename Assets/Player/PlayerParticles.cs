using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [Header("Particles")]

    [SerializeField] private Vector3 jumpingDustOffset; 
    [SerializeField] private Vector3 landingDustOffset; 

    [SerializeField] private GameObject dustGameObject;

    [SerializeField] private GameObject dashDustObject;
    [SerializeField] private ParticleSystem dashTail;

    [SerializeField] private float dashDustSpeed = 0.5f;

    //ParticleSystem.Particle[] dashDustParticles;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        /*
        InitializeIfNeeded();   
    
        int numParticlesAlive = dashDust.GetParticles(dashDustParticles);
        
        Vector2 playerDirection = new Vector2(-rb.velocity.x, -rb.velocity.y);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (dashDustParticles[i].velocity == Vector3.zero)
            {
                //dashDustParticles[i].velocity = playerDirection;
                Debug.Log(dashDustParticles[i].velocity);
            }

        }        
        
        dashDust.SetParticles(dashDustParticles, numParticlesAlive); */
    }

    public void spawnJumpingDust()
    {
        Instantiate(dustGameObject, transform.position + jumpingDustOffset,  Quaternion.identity);
    }

    public void spawnLandingDust()
    {
        Instantiate(dustGameObject, transform.position + jumpingDustOffset,  Quaternion.identity);
    }

    public void spawnDashTail()
    {
        dashTail.time = 0;
        dashTail.Play();
    }

    public void spawnDashDust()
    {
        GameObject newObj = Instantiate(dashDustObject, transform.position, Quaternion.identity);
        newObj.transform.parent = transform;
        ParticleSystem particleSystem = newObj.GetComponent<ParticleSystem>();

        var velocityOverLifetime = particleSystem.velocityOverLifetime;
        velocityOverLifetime.xMultiplier = rb.velocity.x * dashDustSpeed;
        velocityOverLifetime.yMultiplier = rb.velocity.y * dashDustSpeed;
    }

/*
    void InitializeIfNeeded()
    {
        if (dashDustParticles == null || dashDustParticles.Length < dashDust.main.maxParticles)
        {
            dashDustParticles = new ParticleSystem.Particle[dashDust.main.maxParticles];
        }
    }*/
}
