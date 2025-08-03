using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

public class Sun : RotatingPlanet
{
    private static readonly int Eat = Animator.StringToHash("Eat");
    [SerializeField] private StudioEventEmitter screamEmitter;
    [SerializeField] private Animator animator;
    private bool _iseating = false;

    public bool IsScreaming()
    {
        if (screamEmitter == null)
        {
            return false; // If the scream emitter is not assigned, return false
        }
        
        screamEmitter.EventInstance.getParameterByName("scream", out var screamParameter);
        return screamParameter > 0.5f; // Check if the scream parameter is greater than 0
    }

    protected override void Update()
    {
        base.Update();
        
        if (screamEmitter == null || !screamEmitter.IsPlaying() || _iseating)
        {
            return;
        }
        
        screamEmitter.EventInstance.getParameterByName("scream", out var screamParameter);
        var newScreamValue = Mathf.MoveTowards(screamParameter, 1f, Time.deltaTime * 0.3f);
        newScreamValue = Mathf.Min(newScreamValue, GetMaxScreamValue(GetProgress()));
        screamEmitter.EventInstance.setParameterByName("scream", newScreamValue);
    }
    
    private float GetMaxScreamValue(float progress)
    {
        if (progress < 0.8f)
        {
            return 1f;
        }
        else
        {
            return (1 - progress) * 5f; // Decrease scream value as progress approaches 1
        }
    }
    
    public override void StartRotation()
    {
        base.StartRotation();
        
        screamEmitter.Play();
        screamEmitter.EventInstance.setParameterByName("scream", 0f);
    }
    
    public override void RestartRotation()
    {
        base.RestartRotation();
        screamEmitter.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.CompareTag("Item"))
        {
            other.attachedRigidbody.isKinematic = true; // Prevent physics interactions while eating
            other.attachedRigidbody.transform.SetParent(transform); // Set the sun as the parent to keep the item in place
            StartCoroutine(EatItem(other.gameObject));
        }
    }

    private IEnumerator EatItem(GameObject otherGameObject)
    {
        animator.SetTrigger(Eat);
        _iseating = true;
        float stopScreamTime = 0.5f; // Time to stop the scream before eating
        float t = 0f;
        screamEmitter.EventInstance.getParameterByName("scream", out var screamParameter);
        while (t < stopScreamTime)
        {
            t += Time.deltaTime;
            screamEmitter.EventInstance.setParameterByName("scream", Mathf.Lerp(screamParameter, 0f, t / stopScreamTime));
            yield return null;
        }
        Destroy(otherGameObject);
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        _iseating = false;
    }
}
