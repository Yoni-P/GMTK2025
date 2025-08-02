using FMODUnity;
using UnityEngine;

public class Sun : RotatingPlanet
{
    [SerializeField] private StudioEventEmitter screamEmitter;

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
        
        if (screamEmitter == null || !screamEmitter.IsPlaying())
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
}
