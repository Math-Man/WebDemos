using System;
using DefaultNamespace;
using UnityEngine;


public class SphericalMotion : MonoBehaviour
{
    /*
     * x: radius
     * y: polar
     * z: elevation
     */
    public Vector3 sphericalVelocity;
    public Vector3 sphericalPositionCartesian;
    public float velocityDampening = 1f;
    public float radiusAcceleration = 1f;
    public float polarAcceleration = 1f;
    public float elevationAcceleration = 1f;
    public float maxSphericalVelocity = 20f;
    
    public Vector3 worldOffset;
    
    private void Awake()
    {
        sphericalPositionCartesian = new Vector3();
        sphericalVelocity = new Vector3();
        // Helper.CartesianToSpherical(transform.position, out sphericalPositionCartesian.x, out sphericalPositionCartesian.y, out sphericalPositionCartesian.z);
        sphericalPositionCartesian = SphericalCoords.FromCartesianSphericalVector3(
            new Vector3(transform.position.x,
                transform.position.y,
                transform.position.z
            ));
    }


    public void ApplySphericalForce(float radius, float polar, float elevation, float forceMultiplier)
    {
        sphericalVelocity += forceMultiplier *  new Vector3(radius * radiusAcceleration, polar * polarAcceleration, elevation * elevationAcceleration).normalized;
    }
    
    public void Update()
    {
        sphericalVelocity = sphericalVelocity * (1 - Time.deltaTime * velocityDampening);
        sphericalVelocity = Vector3.ClampMagnitude(sphericalVelocity, maxSphericalVelocity);
        sphericalPositionCartesian += sphericalVelocity * Time.deltaTime;
        SphericalCoords sph = SphericalCoords.FromVector3(sphericalPositionCartesian);
        transform.position = worldOffset + sph.ToCartesian();
    }
    
    /// <summary>
    /// Generic helper method to convert cartesian to spherical coordinates
    /// Not used in this example, here for completion sake
    /// </summary>
    /// <param name="cartCoords"></param>
    /// <param name="outRadius"></param>
    /// <param name="outPolar"></param>
    /// <param name="outElevation"></param>
    public static void CartesianToSpherical(Vector3 cartCoords, out float outRadius, out float outPolar, out float outElevation)
    {
        if (cartCoords.x == 0)
            cartCoords.x = Mathf.Epsilon;
        outRadius = Mathf.Sqrt((cartCoords.x * cartCoords.x)
                               + (cartCoords.y * cartCoords.y)
                               + (cartCoords.z * cartCoords.z));
        outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
        if (cartCoords.x < 0)
            outPolar += Mathf.PI;
        outElevation = Mathf.Asin(cartCoords.y / outRadius);
    }
    
}
