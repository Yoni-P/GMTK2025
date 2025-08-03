using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class RotatingPlanet : MonoBehaviour
{
    [SerializeField] protected float duration;
    private float originalDuration;
    [SerializeField] protected Earth earth;
    [SerializeField] protected SplineAnimate splineAnimator;
    [SerializeField] private Light lightSource;
    [SerializeField] private Transform cameraTransform;

    private float maxLightIntensity;
    private Coroutine lightCoroutine;
    
    public bool FinishedRotation { get; private set; } = false;
    

    private void Awake()
    {
        maxLightIntensity = lightSource != null ? lightSource.intensity : 1f;
        lightSource.intensity = 0f; 
        originalDuration = duration;
        splineAnimator.Duration = duration;
        
        splineAnimator.Completed += () =>
        {
            Debug.Log($"{gameObject.name} rotation completed.");
            RestartRotation();
            gameObject.SetActive(false);
            FinishedRotation = true;
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
        splineAnimator.Duration = duration;
    }

    private void RotatePlanetToEarth()
    {
        var cameraEarthAvg = (1f / 5f) * earth.transform.position +
                             (4f / 5f) * cameraTransform.position;
        Debug.DrawLine(transform.position, cameraEarthAvg, Color.red);
        var lookDir = cameraEarthAvg - transform.position;

        transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

        // var angle = Mathf.Atan2(dirToEarth.y, dirToEarth.x) * Mathf.Rad2Deg;
        //
        // var startAngle = 90f;
        //
        // transform.rotation = Quaternion.Euler(0, 0, startAngle + angle);
    }
    
    public virtual void StartRotation(float duration = 10f, bool overrideDuration = false)
    {
        if (overrideDuration)
        {
            this.duration = duration;
        }
        else
        {
            this.duration = originalDuration;
        }
        splineAnimator.Play();
        FinishedRotation = false;
        if (lightSource != null)
        {
            lightSource.intensity = 0f;
            if (lightCoroutine != null)
            {
                StopCoroutine(lightCoroutine);
            }
            lightCoroutine = StartCoroutine(LightPulse());
        }
    }

    private IEnumerator LightPulse()
    {
        var curProgress = GetProgress();
        while (curProgress < 1f)
        {
            lightSource.intensity = curProgress < 0.5f ? Mathf.Lerp(0, maxLightIntensity, curProgress * 2f) : Mathf.Lerp(maxLightIntensity, 0, (curProgress - 0.5f) * 2f);
            yield return null; // Wait for the next frame
            curProgress = GetProgress();
        }
    }

    public virtual void RestartRotation()
    {
        splineAnimator.Restart(false);
        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
        }
        lightSource.intensity = 0;
    }
    
    public float GetProgress()
    {
        return splineAnimator.ElapsedTime / splineAnimator.Duration;
    }
    
    public void SetDuration(float newDuration)
    {
        duration = newDuration;
        splineAnimator.Duration = newDuration;
    }
    
    public float GetDuration()
    {
        return duration;
    }
}
