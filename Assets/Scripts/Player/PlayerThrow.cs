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
        Debug.Log("Mouse button pressed down, ready to throw item.");
        
        // raycast to check if the player is pointing at this object
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("UI"))) // Ensure the layer matches your PlayerThrow object
        {
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("Player is pointing at the PlayerThrow object.");
                _startPosition = Input.mousePosition;
                _playerIsThrowing = true; // Set the flag to indicate the player is throwing
            }
            else
            {
                Debug.Log("Player is not pointing at the PlayerThrow object.");
            }
        }
        else
        {
            Debug.Log("No object hit by the raycast.");
        }
         // Store the initial mouse position
        
    }

    private void Released()
    {
        
        _playerIsThrowing = false; // Reset the flag
        Vector2 endPosition = Input.mousePosition; // Get the final mouse position
        Vector2 direction = endPosition - _startPosition; // Calculate the direction vector

        // Normalize the direction vector and apply a force to the player
        Vector2 force = direction.normalized * throwForce; // Adjust the multiplier as needed
        if (playerPickup != null && playerPickup.HasItem())
        {
            GameObject item = playerPickup.GetCurrentItem();
            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                // Apply the force to the item
                itemRigidbody.AddForce(new Vector3(force.x, force.y, 0), ForceMode.Impulse);
                OnThrow?.Invoke(); // Invoke the OnThrow event if there are subscribers
                Debug.Log("Item thrown with force: " + force);
            }
            else
            {
                Debug.LogError("Rigidbody not found on the item.");
            }
        }
        else
        {
            Debug.LogWarning("No item to throw or PlayerPickup script is not set.");
        }
        
    }

    public event Action OnThrow;
}
