using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Earth : MonoBehaviour
{
    [SerializeField] private SphereCollider earthCollider;
    
    
    public void RotatePlanet(Vector2 rotationSpeed)
    {
        // Ensure the rotation speed is not zero
        if (rotationSpeed == Vector2.zero)
        {
            return;
        }

        // rotate around world z axis according to rotationSpeed.x
        transform.Rotate(Vector3.forward, rotationSpeed.x * Time.fixedDeltaTime, Space.World);
        // rotate around world x axis according to rotationSpeed.y
        transform.Rotate(Vector3.right, -rotationSpeed.y * Time.fixedDeltaTime, Space.World);
    }

    public void PlaceItemOnEarth(GameObject item)
    {
        if (item == null)
        {
            Debug.LogError("Item to place on Earth is null.");
            return;
        }
        bool foundSpawnPosition = false;
        Vector3 spawnPosition = Vector3.zero;

        int maxAttempts = 50;
        int attempts = 0;
        
        while (foundSpawnPosition == false && attempts < maxAttempts)
        {
            attempts++;
            
            Vector3 randomPoint = Random.onUnitSphere * (earthCollider.radius * earthCollider.transform.localScale.x); // Adjust for the scale of the Earth
            
            Debug.Log($"Attempt {attempts}: Random point on sphere: {randomPoint}");
                    
            // calculate the normal at that point
            Vector3 normal = randomPoint.normalized;
            
            // calculate the spawn position by moving the item slightly above the surface
            spawnPosition = earthCollider.transform.position + randomPoint + normal * 0.3f; // Adjust the offset as needed
            
            // Check if the position is valid
            if (Physics.CheckSphere(spawnPosition, 0.2f, LayerMask.GetMask("Obstacle")))
            {
                Debug.LogWarning("Cannot place item on Earth, position is blocked by an obstacle.");
                continue;
            }
            
            foundSpawnPosition = true;
        }
        // Calculate a random position on the Earth's surface
        
        item.transform.position = spawnPosition;
        item.transform.parent = transform; // Set the Earth as the parent of the item
    }

    
}
