using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Earth earth;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationSpeed = 360f; // Degrees per second
    
    private float currentSpeed = 0f;
    private Vector2 moveInput;
    
    private bool collidingWithObstacle = false;
    
    private CapsuleCollider _capsuleCollider;
    private Collider[] colliders = new Collider[1];
    
    public void OnMove (InputValue v)
    {
        moveInput = v.Get<Vector2>();
        Debug.Log("Input detected: " + moveInput);
    }
    
    private void Move()
    {
        // Calculate the desired speed based on input
        float targetSpeed = moveInput.magnitude * maxSpeed;
        
        // Smoothly adjust the current speed towards the target speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        
        var speed = new Vector2(moveInput.x * currentSpeed, moveInput.y * currentSpeed);
        
        speed = CorrectSpeedAccordingToCollisions(speed);

        // Rotate the Earth based on player movement
        earth.RotatePlanet(speed);
    }

    private Vector2 CorrectSpeedAccordingToCollisions(Vector2 speed)
    {
        // Check if colliding with an obstacle
        if (_capsuleCollider == null)
        {
            _capsuleCollider = GetComponent<CapsuleCollider>();
            if (_capsuleCollider == null)
            {
                Debug.LogError("SphereCollider not found on PlayerMove object.");
                return speed;
            }
        }
        
        // Check for collision with obstacles
        int numColliders = Physics.OverlapCapsuleNonAlloc(_capsuleCollider.bounds.center, 
            _capsuleCollider.bounds.center + Vector3.up * _capsuleCollider.height, 
            _capsuleCollider.radius, colliders, LayerMask.GetMask("Obstacle"));
        collidingWithObstacle = false; // Reset collision state
        if (numColliders > 0)
        {
            Vector3 directionToObstacle = colliders[0].transform.position - transform.position;
            var dir = new Vector2(directionToObstacle.x, directionToObstacle.z);
            if (Vector2.Dot(moveInput, dir) > 0)
            {
                collidingWithObstacle = true;
            }
        }
        
        if (collidingWithObstacle)
        {
            // If colliding with an obstacle, calculate new speed based on tangent of collision
            Vector3 collisionNormal = colliders[0].transform.position - transform.position;
            collisionNormal.y = 0; // Ignore vertical component
            
            // project speed onto the plane defined by the collision normal
            Vector3 projectedSpeed = Vector3.ProjectOnPlane(new Vector3(speed.x, 0, speed.y), collisionNormal);
            
            speed = new Vector2(projectedSpeed.x, projectedSpeed.z);
            currentSpeed = speed.magnitude; // Update current speed based on projected speed
        }

        return speed;
    }

    private void FixedUpdate()
    {
        // Call the Move method with the current moveInput
        Move();
        
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        // Rotate the player to face the direction of movement
        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(-moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, angle - 90, 0), 
                rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        // Handle collision with other objects
        // if (other.gameObject.CompareTag("Obstacle"))
        // {
        //     Debug.Log("Collided with an obstacle: " + other.gameObject.name);
        //     // Check if movement is toward the obstacle
        //     Vector3 directionToObstacle = other.transform.position - transform.position;
        //     var dir = new Vector2(directionToObstacle.x, directionToObstacle.z);
        //     if (Vector2.Dot(moveInput, dir) > 0)
        //     {
        //         // If moving towards the obstacle, stop movement
        //         currentSpeed = 0f;
        //         Debug.Log("Stopping movement due to collision with: " + other.gameObject.name);
        //         collidingWithObstacle = true;
        //     }
        //     else
        //     {
        //         // If moving away from the obstacle, allow movement
        //         collidingWithObstacle = false;
        //         Debug.Log("Moving away from obstacle: " + other.gameObject.name);
        //     }
        // }
    }

    // private void OnCollisionExit(Collision other)
    // {
    //     // Reset collision state when exiting collision with an obstacle
    //     if (other.gameObject.CompareTag("Obstacle"))
    //     {
    //         Debug.Log("Exited collision with obstacle: " + other.gameObject.name);
    //         collidingWithObstacle = false;
    //     }
    // }
}
