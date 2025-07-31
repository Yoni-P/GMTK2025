using System;
using System.Collections;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    [SerializeField] private RotatingPlanet sun;
    [SerializeField] private RotatingPlanet moon;

    private RotatingPlanet[] _planets;
    private int _currentPlanetIndex = 0;

    private void Start()
    {
        _planets = new RotatingPlanet[] { sun, moon };  
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

    private RotatingPlanet GetNextPlanet()
    {
        _currentPlanetIndex = (_currentPlanetIndex + 1) % _planets.Length;
        return _planets[_currentPlanetIndex];
    }
}
