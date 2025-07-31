using UnityEngine;

public class Earth : MonoBehaviour
{
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
}
