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

    private RotatingPlanet[] _planets;
    private Light[] _globalLights;
    private int _currentPlanetIndex = 0;

    private void Start()
    {
        _planets = new RotatingPlanet[] { sun, moon };  
        _globalLights = new Light[] { sun.GetComponentInChildren<Light>(), moon.GetComponentInChildren<Light>() };
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
