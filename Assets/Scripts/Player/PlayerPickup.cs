using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private Transform pickupPoint; // Point where the item will be picked up
    private GameObject _currentItem;
    
    private HashSet<GameObject> _timeOutItems = new HashSet<GameObject>(); // To track items that have been picked up and need to be removed after a timeout

    private void OnTriggerEnter(Collider other)
    {
        if (_currentItem != null)
        {
            return; // Ignore if already holding an item
        }
        if (other.attachedRigidbody == null)
        {
            return; // Ignore if the collider does not have a rigidbody
        }
        if (other.attachedRigidbody.gameObject.CompareTag("Item") && _timeOutItems.Contains(other.gameObject) == false)
        {
            _currentItem = other.attachedRigidbody.gameObject;
            
            Debug.Log("Picked up item: " + _currentItem.name);
            PickupItem();
        }
    }

    private void PickupItem()
    {
        if (_currentItem == null)
        {
            Debug.LogWarning("No item to pick up.");
            return;
        }
        _timeOutItems.Add(_currentItem); // Add the item to the timeout set
        // Disable the item's gravity and collider
        var rb = _currentItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Disable physics interactions
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.excludeLayers = LayerMask.GetMask("Player"); // Exclude the item from physics interactions with default layer
        }

        var collider = _currentItem.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false; // Disable collider to prevent further interactions
        }
        
        var itemGravity = _currentItem.GetComponent<ItemGravity>();
        if (itemGravity != null)
        {
            itemGravity.IsHeld = true; // Set the item as held
            itemGravity.DisableCollisionsWithLayer(LayerMask.NameToLayer("Player")); // Disable collisions with the player layer
        }
        
        // _currentItem.layer = LayerMask.NameToLayer("HeldItem"); // Change the layer to "HeldItem" to avoid collisions with other items

        // Move the item to the pickup point
        _currentItem.transform.position = pickupPoint.position;
        _currentItem.transform.SetParent(pickupPoint); // Set the pickup point as the parent

        Debug.Log("Item picked up and moved to pickup point: " + _currentItem.name);
    }
    
    public GameObject GetCurrentItem()
    {
        var currentItem = _currentItem;
        _currentItem = null; // Clear the current item after getting it
        if (currentItem != null)
        {
            // Start a coroutine to remove the item after a timeout
            StartCoroutine(TimoutItem(currentItem, 1f)); // Adjust the timeout duration as needed
            // Re-enable the item's gravity and collider
            var rb = currentItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Enable physics interactions
            }

            var collider = currentItem.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true; // Re-enable collider
            }
            
            var itemGravity = currentItem.GetComponent<ItemGravity>();
            if (itemGravity != null)
            {
                itemGravity.IsHeld = false; // Set the item as not held
                itemGravity.OnThrow();
            }

            currentItem.transform.SetParent(null); // Remove the pickup point as the parent
        }
        return currentItem; // Return the item that was picked up
    }

    public bool HasItem()
    {
        return _currentItem != null; // Check if the player is currently holding an item
    }
    
    private IEnumerator TimoutItem(GameObject item, float timeout)
    {
        yield return new WaitForSeconds(timeout);
        
        if (_timeOutItems.Contains(item))
        {
            var rb = item.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.excludeLayers = 0; // Reset the layer exclusion
            }
            
            var itemGravity = item.GetComponent<ItemGravity>();
            if (itemGravity != null)
            {
                itemGravity.ResetCollisions();
            }
            
            _timeOutItems.Remove(item); // Remove the item from the timeout set
        }
    }
}
