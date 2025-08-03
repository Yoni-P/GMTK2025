using System.Collections;
using UnityEngine;

public class Moon : RotatingPlanet
{
    [SerializeField] private GameObject[] itemsToSpawn; // Items that the moon can spawn
    [SerializeField] private int maxItems = 10;
    
    protected override void Update()
    {
        base.Update();
        
        earth.Damage(-0.5f * Time.deltaTime); // The moon heals the Earth slightly over time
    }
    
    public override void StartRotation(float duration = 10f, bool overrideDuration = false)
    {
        base.StartRotation(duration, overrideDuration);

        var numItemsToSpawn = maxItems - earth.GetNumItemsOnEarth();
        
        StartCoroutine(SpawnItems(numItemsToSpawn));
    }

    private IEnumerator SpawnItems(int numItemsToSpawn)
    {
        Debug.Log($"Starting item spawn with {numItemsToSpawn} items to spawn.");
        for (int i = 0; i < numItemsToSpawn; i++)
        {
            var nextSpawnTime = 0.25f + 0.5f * (i / (numItemsToSpawn - 1f));
            Debug.Log($"Next spawn time: {nextSpawnTime} seconds");
            while (GetProgress() < nextSpawnTime)
            {
                yield return null; // Wait for the next frame
            }
            if (itemsToSpawn.Length > 0)
            {
                var itemIndex = Random.Range(0, itemsToSpawn.Length);
                var item = Instantiate(itemsToSpawn[itemIndex], transform.position, Quaternion.identity);
                var itemRb = item.GetComponent<Rigidbody>();
                if (itemRb != null)
                {
                    itemRb.AddForce(Random.onUnitSphere * 1f, ForceMode.Impulse); // Add some random force to the item
                }
                earth.AddItemToEarth(item);
            }
        }
    }
}
