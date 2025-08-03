using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    [SerializeField] private RotatingPlanet sun;
    [SerializeField] private RotatingPlanet moon;
    
    [SerializeField] private Material skyMaterial; // Material to change the sky color
    
    [SerializeField] private StudioEventEmitter dayNightMusicEmitter; // FMOD event for day/night music
    
    [SerializeField] private GameManager gameManager; // Reference to the GameManager

    private RotatingPlanet[] _planets;
    private Light[] _globalLights;
    private int _currentPlanetIndex = 0;

    private void Start()
    {
        _planets = new RotatingPlanet[] { sun, moon };  
        _globalLights = new Light[] { sun.GetComponentInChildren<Light>(), moon.GetComponentInChildren<Light>() };
        StartCoroutine(SlowMoonCycle());
    }

    private IEnumerator SlowMoonCycle()
    {
        moon.gameObject.SetActive(true);
        moon.StartRotation(30f, true); // Slower rotation for the moon
        while (!moon.FinishedRotation)
        {
            yield return null; // Wait for the next frame
            if (gameManager != null && gameManager.IsGameStarted())
            {
                StartCoroutine(HurryUpMoon());
                yield break; // Exit if the game has started
            }
        }
        Debug.Log("Slow moon cycle completed");
        if (gameManager != null && gameManager.IsGameStarted())
        {
            StartCoroutine(DayNightCycle(_planets[_currentPlanetIndex]));
            yield break; // Exit if the game has started
        }
        
        moon.RestartRotation();
        StartCoroutine(SlowMoonCycle());
    }

    private IEnumerator HurryUpMoon()
    {
        var curDuration = moon.GetDuration();
        var targetDuration = 5f;
        while (curDuration > targetDuration)
        {
            curDuration = Mathf.Lerp(curDuration, targetDuration, Time.deltaTime * 0.5f);
            moon.SetDuration(curDuration);
            if (moon.FinishedRotation)
            {
                break; // Exit if the moon has already progressed enough
            }
            yield return null; // Wait for the next frame
        }

        while (!moon.FinishedRotation)
        {
            Debug.Log($"Hurrying up moon: {moon.GetProgress()}");
            yield return null; // Wait for the next frame
        }
        StartCoroutine(DayNightCycle(_planets[_currentPlanetIndex]));
    }

    private IEnumerator DayNightCycle(RotatingPlanet planet)
    {
        planet.gameObject.SetActive(true);
        planet.StartRotation();
        while (planet.GetProgress() < 0.8f)
        {
            yield return null; // Wait for the next frame
        }
        StartCoroutine(DayNightCycle(GetNextPlanet()));
    }
    
    private void Update()
    {
        if (skyMaterial != null)
        {
            // Get the value of all light sources in the scene
            var intensity = 0f;
            foreach (var light in _globalLights)
            {
                if (light != null)
                {
                    intensity += light.intensity;
                }
            }
            intensity = Mathf.Clamp(intensity, 0f, 1f); // Ensure intensity is between 0 and 1
            skyMaterial.SetFloat("_Blend", 1 - intensity);
        }
        
        if (dayNightMusicEmitter != null && dayNightMusicEmitter.IsPlaying())
        {
            var day = 0.5f;
            dayNightMusicEmitter.EventInstance.getParameterByName("day", out var dayParameter);
            if (sun.isActiveAndEnabled)
            {
                var sunProg = sun.GetProgress() <= 0.5f ? sun.GetProgress() : 1f - sun.GetProgress();
                day += sunProg;
            if (moon.isActiveAndEnabled)
            {
                var moonprog = moon.GetProgress() <= 0.5f ? moon.GetProgress() : 1f - moon.GetProgress();
                day -= moonprog;
            }
            day = Mathf.Clamp(day, 0f, 1f); // Ensure day parameter is between 0 and 1
            dayNightMusicEmitter.EventInstance.setParameterByName("day", day);
            }
        }
    }

    private RotatingPlanet GetNextPlanet()
    {
        _currentPlanetIndex = (_currentPlanetIndex + 1) % _planets.Length;
        return _planets[_currentPlanetIndex];
    }
}
