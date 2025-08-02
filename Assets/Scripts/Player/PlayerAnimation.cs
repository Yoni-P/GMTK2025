using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerThrow playerThrow;
    [SerializeField] private PlayerPickup playerPickup; // Reference to the PlayerPickup script
    [SerializeField] private Sun sun;
    
    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (playerMove == null)
        {
            playerMove = GetComponent<PlayerMove>();
        }

        if (playerThrow == null)
        {
            playerThrow = GetComponent<PlayerThrow>();
        }

        if (sun == null)
        {
            sun = FindObjectOfType<Sun>();
            if (sun == null)
            {
                Debug.LogError("Sun object not found in the scene.");
            }
        }
    }
    
    private void Update()
    {
        if (animator == null || playerMove == null || playerThrow == null || sun == null)
        {
            return; // Ensure all components are assigned before proceeding
        }

        // Set the animator parameters based on player movement and actions
        animator.SetBool("Running", playerMove.IsRunning);
        animator.SetBool("Screaming", sun.IsScreaming());
        animator.SetBool("Holding", playerPickup.IsHoldingItem);
    }
    
    private void OnThrow()
    {
        if (animator != null)
        {
            animator.SetTrigger("Throw");
        }
    }
    
    private void OnEnable()
    {
        if (playerThrow != null)
        {
            playerThrow.OnThrow += OnThrow;
        }
    }

    private void OnDisable()
    {
        if (playerThrow != null)
        {
            playerThrow.OnThrow -= OnThrow;
        }
    }
}
