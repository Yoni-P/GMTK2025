using System;
using FMODUnity;
using UnityEngine;

public class ItemGravity : MonoBehaviour
{
    [SerializeField] private float gravityStrength = 9.81f; // Strength of the gravity effect
    [SerializeField] private Earth earth; // Reference to the Earth object
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private Collider[] colliders; // Colliders to disable when the item is held
    
    [SerializeField] private StudioEventEmitter throwSoundEmitter; // Sound emitter for throwing sound
    [SerializeField] private StudioEventEmitter hitSoundEmitter; // Sound emitter for hit sound
    
    private float hitSoundTimeout = 3f; // Time before the hit sound can be played again
    private float hitSoundCooldown = 3f; // Cooldown time for hit sound
    private Animator _animator;

    public void OnThrow()
    {
        if (_animator != null)
        {
            _animator.SetBool("Thrown", true);
        }
        if (throwSoundEmitter != null)
        {
            throwSoundEmitter.Play(); // Play the throw sound
        }
    }
    
    public void SetGravityStrength(float strength)
    {
        gravityStrength = strength; // Set the gravity strength
    }


    public bool IsHeld = false;
    private void Start()
    {
        if (earth == null)
        {
            earth = FindObjectOfType<Earth>();
            if (earth == null)
            {
                Debug.LogError("Earth object not found in the scene.");
            }
        }
        
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        colliders = GetComponentsInChildren<Collider>();
        
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        
        if (throwSoundEmitter == null)
        {
            throwSoundEmitter = GetComponent<StudioEventEmitter>();
            if (throwSoundEmitter == null)
            {
                Debug.LogError("Throw sound emitter not found on ItemGravity object.");
            }
        }
    }

    private void Update()
    {
        if (earth != null)
        {
            ApplyGravity();
        }
        if (hitSoundTimeout > 0f)
        {
            hitSoundTimeout -= Time.deltaTime; // Decrease the timeout
        }
    }

    private void ApplyGravity()
    {

        // Calculate the direction towards the center of the Earth
        Vector3 directionToEarth = (earth.transform.position - transform.position).normalized;

        // Apply gravity force towards the Earth
        rb.AddForce(directionToEarth * gravityStrength, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (hitSoundTimeout <= 0f && hitSoundEmitter != null)
        {
            hitSoundEmitter.Play(); // Play the hit sound
            hitSoundTimeout = hitSoundCooldown; // Reset the timeout
        }
        if (other.gameObject.CompareTag("Earth") && !IsHeld)
        {
            rb.constraints = RigidbodyConstraints.None;
            transform.SetParent(earth.transform); // Attach the item to the Earth when it collides
            if (_animator != null)
            {
                _animator.SetBool("Thrown", false); // Reset the thrown state
            }
        }
    }
    
    public void DisableCollisionsWithLayer(int layer)
    {
        if (colliders == null || colliders.Length == 0)
        {
            colliders = GetComponentsInChildren<Collider>();
        }

        foreach (var collider in colliders)
        {
            if (collider != null)
            {
                collider.excludeLayers = layer;
            }
        }
    }
    
    public void ResetCollisions()
    {
        if (colliders == null || colliders.Length == 0)
        {
            colliders = GetComponentsInChildren<Collider>();
        }

        foreach (var collider in colliders)
        {
            if (collider != null)
            {
                collider.excludeLayers = 0; // Reset to default layer
            }
        }
    }

    public event Action onDestroy;
    
    private void OnDestroy()
    {
        onDestroy?.Invoke(); // Invoke the destroy event
        ResetCollisions(); // Reset collisions when the item is destroyed
    }
}
