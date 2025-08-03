using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class RotatingPlanet : MonoBehaviour
{
    [SerializeField] protected float duration;
    [SerializeField] protected Earth earth;
    [SerializeField] protected SplineAnimate splineAnimator;
    [SerializeField] private Light lightSource;

    private float maxLightIntensity;
    private Sequence lightSequence;

    private void Awake()
    {
        maxLightIntensity = lightSource != null ? lightSource.intensity : 1f;
        lightSource.intensity = 0f; 
        duration = splineAnimator.Duration;
        
        splineAnimator.Completed += () =>
        {
            Debug.Log($"{gameObject.name} rotation completed.");
            RestartRotation();
            gameObject.SetActive(false);
        };
    }

    protected void Start()
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
    
    protected virtual void Update()
    {
        if (earth != null)
        {
            RotatePlanetToEarth();
        }
    }

    private void RotatePlanetToEarth()
    {
        var cameraEarthAvg = (earth.transform.position + Camera.main.transform.position) / 2f;
        var lookDir = cameraEarthAvg - transform.position;

        transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

        // var angle = Mathf.Atan2(dirToEarth.y, dirToEarth.x) * Mathf.Rad2Deg;
        //
        // var startAngle = 90f;
        //
        // transform.rotation = Quaternion.Euler(0, 0, startAngle + angle);
    }
    
    public virtual void StartRotation()
    {
        splineAnimator.Play();
        if (lightSource != null)
        {
            lightSource.intensity = 0f; // Reset light intensity
            lightSequence = DOTween.Sequence();
            lightSequence.Append(lightSource.DOIntensity(maxLightIntensity, duration / 2f))
                .Append(lightSource.DOIntensity(0, duration / 2f))
                .SetLoops(-1, LoopType.Restart);
        }
    }
    
    // public void StopRotation()
    // {
    //     splineAnimator.Pause();
    //     lightSequence?.Pause();
    // }

    public virtual void RestartRotation()
    {
        splineAnimator.Restart(false);
        lightSequence?.Kill();
        lightSource.intensity = 0;
    }
    
    public float GetProgress()
    {
        return splineAnimator.ElapsedTime / splineAnimator.Duration;
    }
}
