using System;
using UnityEngine;

[Serializable]
public struct SerializableVector3 : IEquatable<SerializableVector3>
{
    public float x;
    public float y;
    public float z;

    public const float KEpsilon = 1E-05f;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public static implicit operator Vector3(SerializableVector3 sVector)
    {
        return new Vector3(sVector.x, sVector.y, sVector.z);
    }

    public static implicit operator SerializableVector3(Vector3 vector)
    {
        return new SerializableVector3(vector);
    }

    public bool Equals(SerializableVector3 other)
    {
        return CompareApproximate(this, other, KEpsilon);
    }

    public override bool Equals(object obj)
    {
        return obj is SerializableVector3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (x, y, z).GetHashCode();
    }

    public static bool operator ==(SerializableVector3 a, SerializableVector3 b)
    {
        return CompareApproximate(a, b, KEpsilon);
    }

    public static bool operator !=(SerializableVector3 a, SerializableVector3 b)
    {
        return !(a == b);
    }

    public static bool CompareApproximate(SerializableVector3 a, SerializableVector3 b, float epsilon = KEpsilon)
    {
        return Mathf.Abs(a.x - b.x) < epsilon &&
               Mathf.Abs(a.y - b.y) < epsilon &&
               Mathf.Abs(a.z - b.z) < epsilon;
    }
}