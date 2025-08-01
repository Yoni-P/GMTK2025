using UnityEngine;

public class ItemGravity : MonoBehaviour
{
    [SerializeField] private float gravityStrength = 9.81f; // Strength of the gravity effect
    [SerializeField] private Earth earth; // Reference to the Earth object
    [SerializeField] private Rigidbody rb;

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
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody not found on ItemGravity object.");
                return;
            }
        }

        // Calculate the direction towards the center of the Earth
        Vector3 directionToEarth = (earth.transform.position - transform.position).normalized;

        // Apply gravity force towards the Earth
        rb.AddForce(directionToEarth * gravityStrength, ForceMode.Acceleration);
    }
}
