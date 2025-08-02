using System;
using System.Collections;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    [SerializeField] private RotatingPlanet sun;
    [SerializeField] private RotatingPlanet moon;
    
    [SerializeField] private Material skyMaterial; // Material to change the sky color

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
    }

    private RotatingPlanet GetNextPlanet()
    {
        _currentPlanetIndex = (_currentPlanetIndex + 1) % _planets.Length;
        return _planets[_currentPlanetIndex];
    }
}
