using System;
using UnityEngine;

[Serializable]
public struct SphericalCoords
{
    [SerializeField] private float _radialDistance; //x
    [SerializeField] private float _polar;  //y
    [SerializeField] private float _elevation; //z

    public const float TAU = Mathf.PI * 2;

    public float GetRadialDistance() => _radialDistance;
    public float SetRadialDistance(float radialDist) => (_radialDistance = radialDist);
    
    public float GetPolar() => _polar;
    public float SetPolar(float polar) => _polar = SphericalClamp(polar);
    
    public float GetElevation() => _elevation;
    public float SetElevation(float elevation) => _elevation = SphericalClamp(elevation);
    
    /// <summary>
    /// Spherically clamps the value between 0 and TAU
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float SphericalClamp(float value)
    {
        var flatValue = value >= 0 ? value : TAU - Mathf.Abs( value % TAU); 
        return flatValue % TAU;
    }


    /// <summary>
    ///  <para> Creates a new spherical position with given Radial Distance, Elevation, Polar coordinates</para>
    /// </summary>
    /// <param name="r"></param>
    /// <param name="q"></param>
    /// <param name="p"></param>
    public SphericalCoords(float r, float q, float p)
    {
        this._radialDistance = r;
        this._polar = SphericalClamp(q);
        this._elevation = SphericalClamp(p);
    }
    
    /// <summary>
    /// Converts cartesian coordinates to spherical coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static SphericalCoords FromCartesian(float x, float y, float z)
    {
        return FromCartesian(new Vector3(x,y,z));
    }
    
    /// <summary>
    /// Converts cartesian coordinates to spherical coordinates
    /// </summary>
    /// <param name="cartesianCoord"></param>
    /// <returns></returns>
    public static SphericalCoords FromCartesian(Vector3 cartesianCoord)
    {
        if (cartesianCoord.x.CompareTo(0) == 0)
            cartesianCoord.x = Mathf.Epsilon;
        var radDist = Mathf.Sqrt(cartesianCoord.x * cartesianCoord.x+
                                 cartesianCoord.y * cartesianCoord.y +
                                 cartesianCoord.z * cartesianCoord.z);
        
        var pol = Mathf.Atan(cartesianCoord.z / cartesianCoord.x);
        if (cartesianCoord.x < 0)
            pol += Mathf.PI;
        if (radDist.CompareTo(0) == 0)
            radDist = Mathf.Epsilon;
        var elev = Mathf.Asin(cartesianCoord.y / radDist);
        return new SphericalCoords(radDist, pol, elev);
    }

    /// <summary>
    /// Returns a vector3 with spherical Coordinates
    /// </summary>
    /// <param name="cartesianCoords"></param>
    /// <returns></returns>
    public static Vector3 FromCartesianSphericalVector3(Vector3 cartesianCoords)
    {
        var rs = FromCartesian(cartesianCoords);
        return new Vector3(rs.GetRadialDistance(), rs.GetPolar(), rs.GetElevation());
    }


    /// <summary>
    /// Gets direction vector for two spherical positions
    /// <para>In this method one of the spherical points is indicated as a vector</para>
    /// </summary>
    /// <param name="otherPosition"></param>
    /// <param name="sphericalVector"></param>
    /// <returns></returns>
    public static Vector3 GetSphericalDirectionToFromSphericalVector3(SphericalCoords otherPosition, Vector3 sphericalVector)
    {
        SphericalCoords vec = new SphericalCoords(sphericalVector.x, sphericalVector.y, sphericalVector.z);
        return vec.GetSphericalDirectionTo(otherPosition);
    }

    /// <summary>
    /// Gets direction vector for two spherical positions
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public Vector3 GetSphericalDirectionTo(SphericalCoords other)
    {
        var x = other.GetRadialDistance() - this.GetRadialDistance();
        var y = other.GetPolar() - this.GetPolar();
        var z = other.GetElevation() - this.GetElevation();
        var y_r = TAU - Mathf.Abs(y);
        var z_r = TAU - Mathf.Abs(z);

        if (Mathf.Abs(y) > Mathf.Abs(y_r))
            y = y_r;

        if (Mathf.Abs(z) > Mathf.Abs(z_r))
            y = z_r;
        
        return new Vector3(x,y,z);
    }

    public static SphericalCoords FromVector3(Vector3 sphericalVector3)
    {
        return new SphericalCoords(sphericalVector3.x, sphericalVector3.y, sphericalVector3.z);
        // float a = _radialDistance * Mathf.Cos(_elevation);
        // return new Vector3(
        //     a * Mathf.Cos(_polar),
        //     _radialDistance * Mathf.Sin(_elevation),
        //     a * Mathf.Sin(_polar));
    }
    
    /// <summary>
    /// Converts spherical position to cartesian position
    /// </summary>
    /// <returns></returns>
    public Vector3 ToCartesian()
    {
        float a = _radialDistance * Mathf.Cos(_elevation);
        return new Vector3(
            a * Mathf.Cos(_polar),
            _radialDistance * Mathf.Sin(_elevation),
            a * Mathf.Sin(_polar));
    }

    /// <summary>
    /// Converts spherical position to cartesian position
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public static Vector3 ToCartesian(SphericalCoords coords)
    {
        return coords.ToCartesian();
    }

    /// <summary>
    /// This is much more expansive than just using great circle distance or cartesian distance.
    /// https://math.stackexchange.com/a/1078262
    /// Rule of thumb is that latitude is the elevation
    /// Longitude is the Polar
    /// </summary>
    /// <param name="sc1"></param>
    /// <param name="sc2"></param>
    /// <returns></returns>
    public static float SphericalDistance(SphericalCoords sc1, SphericalCoords sc2)
    {
        return Mathf.Sqrt(
            Mathf.Pow(sc1._radialDistance, 2) + Mathf.Pow(sc2._radialDistance, 2) 
            + (-2) * sc1._radialDistance * sc2._radialDistance 
            * (Mathf.Cos(sc1._elevation) * Mathf.Cos(sc2._elevation) * Mathf.Cos(sc1._polar - sc2._polar) 
               + Mathf.Sin(sc1._elevation)*Mathf.Sin(sc2._elevation)));
    }
    
    /// <summary>
    /// Clamps magnitude of a spherical vector
    /// </summary>
    /// <param name="spc"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static SphericalCoords ClampMagnitude(SphericalCoords spc, float maxLength)
    {
        float sqrMagnitude = SqrMagnitude(spc);
        if ((double) sqrMagnitude <= (double) maxLength * (double) maxLength)
            return spc;
        float num1 = (float) Mathf.Sqrt(sqrMagnitude);
        float num2 = spc._radialDistance / num1;
        float num3 = spc._elevation / num1;
        float num4 = spc._polar / num1;
        return new SphericalCoords(num2 * maxLength, num3 * maxLength, num4 * maxLength);
    }
    
    public static float SqrMagnitude(SphericalCoords vector)
    {
        return (float) ((double) vector._radialDistance * (double) vector._radialDistance + (double) vector._elevation * (double) vector._elevation + (double) vector._polar * (double) vector._polar);
    }
    
    public static SphericalCoords operator +(SphericalCoords a, SphericalCoords b)
    {
        return new SphericalCoords(a._radialDistance + b._radialDistance, a._elevation + b._elevation, a._polar + b._polar);
    }

    public static SphericalCoords operator -(SphericalCoords a, SphericalCoords b)
    {
        return new SphericalCoords(a._radialDistance - b._radialDistance, a._elevation - b._elevation, a._polar - b._polar);
    }

    public static SphericalCoords operator -(SphericalCoords a)
    {
        return new SphericalCoords(-a._radialDistance, -a._elevation, -a._polar);
    }

    public static SphericalCoords operator *(SphericalCoords a, float d)
    {
        return new SphericalCoords(a._radialDistance * d, a._elevation * d, a._polar * d);
    }

    public static SphericalCoords operator *(float d, SphericalCoords a)
    {
        return new SphericalCoords(a._radialDistance * d, a._elevation * d, a._polar * d);
    }

    public static SphericalCoords operator /(SphericalCoords a, float d)
    {
        return new SphericalCoords(a._radialDistance / d, a._elevation / d, a._polar / d);
    }

    public static bool operator ==(SphericalCoords lhs, SphericalCoords rhs)
    {
        float num1 = lhs._radialDistance - rhs._radialDistance;
        float num2 = lhs._elevation - rhs._elevation;
        float num3 = lhs._polar - rhs._polar;
        return (double) num1 * (double) num1 + (double) num2 * (double) num2 + (double) num3 * (double) num3 < 9.99999943962493E-11;
    }

    public static bool operator !=(SphericalCoords lhs, SphericalCoords rhs)
    {
        return !(lhs == rhs);
    }
    
    
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"S({_radialDistance}, {_polar}, {_elevation})";
    }
}