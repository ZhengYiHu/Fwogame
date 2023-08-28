using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class VectorUtils
{
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles)* dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}
