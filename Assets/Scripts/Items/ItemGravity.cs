using System;
using UnityEngine;

public class ItemGravity : MonoBehaviour
{
    [SerializeField] private float gravityStrength = 9.81f; // Strength of the gravity effect
    [SerializeField] private Earth earth; // Reference to the Earth object
    [SerializeField] private Rigidbody rb;


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
        }
    }
}
