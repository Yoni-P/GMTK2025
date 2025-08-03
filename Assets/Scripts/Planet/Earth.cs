using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class Earth : MonoBehaviour
{
    [SerializeField] private SphereCollider earthCollider;

    [SerializeField] private float maxHealth = 30f;
    
    [SerializeField] private CinemachineBasicMultiChannelPerlin cinemachineNoise;
    
    [SerializeField] private StudioEventEmitter rumbleEmitter;
    
    [SerializeField] private ParticleSystem explosionEffect; // Particle effect for Earth destruction

    [SerializeField] private Rigidbody playerRb;
    
    private float health;
    private int numItemsOnEarth = 0; // Counter for items placed on Earth
    private bool isDestroyed = false; // Flag to check if Earth is destroyed
    
    private void Start()
    {
        health = maxHealth;
    }
    
    
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
    
    public void AddItemToEarth(GameObject item)
    {
        if (item == null)
        {
            Debug.LogError("Item to add to Earth is null.");
            return;
        }
        
        numItemsOnEarth++;
        var itemGravity = item.GetComponent<ItemGravity>();
        if (itemGravity != null)
        {
            itemGravity.OnThrow(); // Call OnThrow to trigger any animations or effects
            itemGravity.onDestroy += () => 
            {
                numItemsOnEarth--;
                Debug.Log($"Item removed from Earth. Remaining items: {numItemsOnEarth}");
            };
        }
        
    }
    
    public int GetNumItemsOnEarth()
    {
        return numItemsOnEarth;
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

    
    public void Damage(float damage)
    {
        if (isDestroyed)
        {
            return;
        }
        
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth); // Ensure health does not go below 0 or above maxHealth
        
        // Trigger camera shake effect
        if (cinemachineNoise != null)
        {
            cinemachineNoise.AmplitudeGain = Mathf.Clamp(maxHealth - health, 0f, maxHealth);
            cinemachineNoise.FrequencyGain = Mathf.Clamp((maxHealth - health) / 2f, 0f, maxHealth / 2f);
        }
        
        if (rumbleEmitter != null)
        {
            if (!rumbleEmitter.IsPlaying())
            {
                rumbleEmitter.Play();
            }

            // set global parameter "critical" to earths health percentage
            float criticalValue = Mathf.Clamp01(1 - health / maxHealth);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("critical", criticalValue);
        }
        
        if (health <= 0)
        {
            Debug.Log("Earth has been destroyed!");
            StartCoroutine(DestroyEarth());
        }
    }
    
    public Action OnEarthDestroyed;
    private IEnumerator DestroyEarth()
    {
        isDestroyed = true; // Set the flag to prevent further damage
        explosionEffect.gameObject.SetActive(true);
        
        // add a rigidibody to every child of the Earth and apply an explosion force
        
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            
            var rb = child.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = child.gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false; // Ensure the rigidbody is not kinematic
            rb.AddExplosionForce(10f, transform.position, 5f); // Adjust force and radius as needed
            rb.useGravity = true; // Enable gravity for the debris
        }
        
        var items = FindObjectsByType<ItemGravity>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var item in items)
        {
            item.SetGravityStrength(0f);
        }
        var planetRenderer = GetComponent<Renderer>();
        if (planetRenderer != null)
        {
            planetRenderer.enabled = false; // Hide the Earth renderer
        }
        
        var planetCollider = GetComponent<Collider>();
        if (planetCollider != null)
        {
            planetCollider.enabled = false; // Disable the Earth collider
        }
        
        if (playerRb != null)
        {
            playerRb.isKinematic = true; // Prevent player from moving during destruction
            playerRb.transform.SetParent(null); // Detach player from Earth
            playerRb.constraints = RigidbodyConstraints.None; // Remove all constraints
            playerRb.AddExplosionForce(10f, transform.position, 5f); // Apply explosion force to the player
            playerRb.useGravity = true; // Enable gravity for the player
        }
        
        if (explosionEffect != null)
        {
            yield return new WaitForSeconds(explosionEffect.main.duration); // Wait for the effect to finish
        }
        
        if (rumbleEmitter != null)
        {
            rumbleEmitter.Stop();
        }
        
        if (cinemachineNoise != null)
        {
            cinemachineNoise.AmplitudeGain = 0f; // Reset the noise effect
            cinemachineNoise.FrequencyGain = 0f;
        }
        
        OnEarthDestroyed?.Invoke(); // Invoke the destruction event
    }
}
