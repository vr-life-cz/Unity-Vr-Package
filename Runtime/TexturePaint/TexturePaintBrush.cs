using System;
using UnityEngine;

namespace Vrlife.Core.TexturePaint
{
    public class TexturePaintBrush : MonoBehaviour
    {
        public Color Color = Color.blue;
        public float Size = 1f;
        public float Strength = 200;

        public Material brushrenderer;

        public float rayLength = 10;

        private void Start()
        {
            SetColor(Color);
        }

        private Ray _ray;

        public Transform rayPoint;

        public Ray GetRay()
        {
            _ray.origin = rayPoint.position;

            _ray.direction = rayPoint.forward;

            return _ray;
        }

        public void SetColor(Color color)
        {
            Color = color;
            if (brushrenderer)
            {
                brushrenderer.SetColor("_BaseColor", color);                
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rayPoint.position, rayPoint.position + rayPoint.forward * rayLength);
        }
    }
}