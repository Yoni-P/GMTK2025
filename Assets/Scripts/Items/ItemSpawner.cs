using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] itemsToSpawn;
    
    [SerializeField] private float spawnInterval = 5f; // Time in seconds between spawns
    
    [SerializeField] private Earth earth; // Reference to the Earth object
    
    private float spawnTimer;
    private void Start()
    {
        spawnTimer = spawnInterval; // Initialize the timer
        
        if (earth == null)
        {
            earth = FindObjectOfType<Earth>();
            if (earth == null)
            {
                Debug.LogError("Earth object not found in the scene.");
            }
        }
    }
    
    private void Update()
    {
        spawnTimer -= Time.deltaTime; // Decrease the timer by the time since last frame
        
        if (spawnTimer <= 0f)
        {
            SpawnItem();
            spawnTimer = spawnInterval; // Reset the timer
        }
    }

    private void SpawnItem()
    {
        var itemIndex = Random.Range(0, itemsToSpawn.Length);
        
        var item = Instantiate(itemsToSpawn[itemIndex], transform.position, Quaternion.identity);
        
        earth.PlaceItemOnEarth(item); // Place the item on the Earth
    }
}
