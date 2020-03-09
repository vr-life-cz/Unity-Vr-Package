using System;
using UnityEngine;

namespace Vrlife.Core.Vr
{
    [Serializable]

    public class TrackingInformation
    {
        public TrackingInformation(HumanBodyPart part)
        {
            Part = part;
        }

        public Vector3 Position;

        public Quaternion Rotation;

        public Vector3 Velocity;

        public Vector3 Acceleration;

        private HumanBodyPart Part { get; }
    }
}