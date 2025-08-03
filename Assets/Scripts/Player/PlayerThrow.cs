using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrow : MonoBehaviour
{
    [SerializeField] private PlayerPickup playerPickup; // Reference to the PlayerPickup script
    [SerializeField] private float throwForce = 20f; // Force applied when throwing the item
    
    private Vector2 _startPosition;
    private bool _playerIsThrowing = false;


    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // Check for left mouse button press
        {
            Pressed();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && _playerIsThrowing) // Check for left mouse button release
        {
            Released();
        }
    }


    private void Pressed()
    {
        
        // raycast to check if the player is pointing at this object
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("UI"))) // Ensure the layer matches your PlayerThrow object
        {
            if (hit.collider.gameObject == gameObject)
            {
                _startPosition = Mouse.current.position.ReadValue(); // Store the initial mouse position
                _playerIsThrowing = true; // Set the flag to indicate the player is throwing
            }
        }
         // Store the initial mouse position
        
    }

    private void Released()
    {
        
        _playerIsThrowing = false; // Reset the flag
        Vector2 endPosition = Mouse.current.position.ReadValue(); // Get the current mouse position when released
        Vector2 direction = endPosition - _startPosition; // Calculate the direction vector

        // Normalize the direction vector and apply a force to the player
        Vector2 force = direction.normalized * throwForce; // Adjust the multiplier as needed
        if (playerPickup != null && playerPickup.HasItem())
        {
            GameObject item = playerPickup.GetCurrentItem();
            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                itemRigidbody.linearVelocity = Vector3.zero; // Reset the item's velocity before applying the force
                // Apply the force to the item
                itemRigidbody.AddForce(new Vector3(force.x, force.y, 0), ForceMode.Impulse);
                OnThrow?.Invoke(); // Invoke the OnThrow event if there are subscribers
            }
        }
    }

    public event Action OnThrow;
}
