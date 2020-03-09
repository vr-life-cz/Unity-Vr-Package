using System;
using System.Globalization;
using UnityEngine;

namespace Vrlife.Core.Vr
{
    public static class AxisHelper
    {
        public static Vector3 ToVector(this Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return Vector3.right;
                case Axis.Y:
                    return Vector3.up;
                case Axis.Z:
                    return Vector3.forward;
                case Axis.XNeg:
                    return Vector3.left;
                case Axis.YNeg:
                    return Vector3.down;
                case Axis.ZNeg:
                    return Vector3.back;
                case Axis.Unknown:
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public static float GetEuler(this Axis axis, Quaternion quaternion)
        {
            switch (axis)
            {
                case Axis.XNeg:
                case Axis.X:
                    return quaternion.eulerAngles.x;
                case Axis.YNeg:
                case Axis.Y:
                    return quaternion.eulerAngles.y;
                case Axis.ZNeg:
                case Axis.Z:
                    return quaternion.eulerAngles.z;
                case Axis.Unknown:
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public static Vector3 GetNormal(this Axis planeAxis, Transform transform)
        {
            Vector3 vec;

            switch (planeAxis)
            {
                case Axis.X:
                    vec = transform.right;
                    break;
                case Axis.Y:
                    vec = transform.up;
                    break;
                case Axis.Z:
                    vec = transform.forward;
                    break;
                case Axis.XNeg:
                    vec = transform.right * -1;
                    break;
                case Axis.YNeg:
                    vec = transform.up * -1;
                    break;
                case Axis.ZNeg:
                    vec = transform.forward * -1;
                    break;
                case Axis.Unknown:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return vec;
        }
    }
}