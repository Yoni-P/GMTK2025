using UnityEngine;

public class Moon : RotatingPlanet
{
    protected override void Update()
    {
        base.Update();
        
        earth.Damage(-0.5f * Time.deltaTime); // The moon heals the Earth slightly over time
    }
}
