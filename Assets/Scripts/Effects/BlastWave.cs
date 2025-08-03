using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWave : MonoBehaviour
{
    
    [Tooltip("The number of point on the blast circle, more points = rounder circle"),SerializeField] 
    private int pointsCount;
    [Tooltip("The maximum radius of the blast circle in unity units"),SerializeField]
    private float maxRadius;
    [Tooltip("The speed of the blast circle expansion"),SerializeField]
    private float blastSpeed;
    [Tooltip("The width of the circumference of the blast circle"),SerializeField]
    private float startWidth;
    
    private LineRenderer lineRenderer;
    private float curRadius = 0;
    
    // Is the blast wave finished expanding
    public bool IsFinished { get; set; }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        IsFinished = false;
        lineRenderer.positionCount = pointsCount + 1;
    }

    private void Start()
    {
        StartCoroutine(Blast());
    }

    /*
     * Expands the blast wave
     */
    public IEnumerator Blast()
    {
        curRadius = 0f;
        while (curRadius < maxRadius)
        {
            curRadius += blastSpeed * Time.deltaTime;
            lineRenderer.SetPositions(GetCirclePoints());
            lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1 - curRadius / maxRadius);
            yield return null;
        }
        IsFinished = true;
    }

    private Vector3[] GetCirclePoints()
    {
        float angleBetweenPoints = 360f / pointsCount;
        Vector3[] points = new Vector3[pointsCount + 1];
        for (int i = 0; i < pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            Vector3 pos = transform.position + dir * curRadius;
            points[i] = pos;
        }
        
        points[pointsCount] = points[0];
        return points;
    }
    
    /*
     * Returns the current radius of the blast wave
     */
    public float GetCurRadius()
    {
        return curRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}
