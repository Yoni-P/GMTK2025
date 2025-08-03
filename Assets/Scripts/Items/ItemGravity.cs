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
        if (other.gameObject.CompareTag("Earth") && !IsHeld)
        {
            Debug.Log("Item collided with Earth: " + gameObject.name);
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
}
