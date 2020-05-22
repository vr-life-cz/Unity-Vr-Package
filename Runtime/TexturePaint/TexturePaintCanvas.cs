using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vrlife.Core.TexturePaint
{
    public class TexturePaintCanvas : MonoBehaviour
    {
        [SerializeField] private ProximityWatcher _proximityTrigger;

        public Material maskMaterial;
        public Material colorMaterial;
        public Material mergeMaterial;

        private static readonly int Coordinate = Shader.PropertyToID("_Coordinate");
        private static readonly int BrushSize = Shader.PropertyToID("_BrushSize");
        private static readonly int SplatMap = Shader.PropertyToID("_SplatMap");
        private static readonly int DrawColor = Shader.PropertyToID("_Color");
        private static readonly int BrushStrength = Shader.PropertyToID("_BrushStrength");

        private static readonly int MaskMap = Shader.PropertyToID("_Mask");
        private static readonly int ColorMap = Shader.PropertyToID("_ColorMap");
        private static readonly int BaseTexture = Shader.PropertyToID("_BaseTexture");


        private CustomRenderTexture maskTexture;
        private CustomRenderTexture colorTexture;

        private List<TexturePaintBrush> _brushes;


        // Start is called before the first frame update
        void Start()
        {
            _brushes = new List<TexturePaintBrush>();

            maskTexture = new CustomRenderTexture(1024, 1024, RenderTextureFormat.ARGB64);
            colorTexture = new CustomRenderTexture(1024, 1024, RenderTextureFormat.ARGB64);
            colorMaterial.SetTexture(SplatMap, colorTexture);
            maskMaterial.SetTexture(SplatMap, maskTexture);
            mergeMaterial.SetTexture(MaskMap, maskTexture);
            mergeMaterial.SetTexture(ColorMap, colorTexture);


            _proximityTrigger.onProximityTriggerEnter.AddListener(TryAddBrush);
            _proximityTrigger.onProximityTriggerExit.AddListener(TryRemoveBrush);
        }

        private void TryRemoveBrush(ProximityWatcher arg0, Collider arg1)
        {
            var brush = arg1.GetComponent<TexturePaintBrush>();
            if (brush)
                _brushes.Remove(brush);
        }

        private void TryAddBrush(ProximityWatcher arg0, Collider arg1)
        {
            var brush = arg1.GetComponent<TexturePaintBrush>();
            if (brush && !_brushes.Contains(brush))
                _brushes.Add(brush);
        }

        private void OnDestroy()
        {
            _brushes.Clear();
            maskTexture.Release();
        }

        private RaycastHit[] _hits = new RaycastHit[5];

        // Update is called once per frame
        void FixedUpdate()
        {

            var paintBrush = _brushes.FirstOrDefault();
           
            if (!paintBrush) return;

            var ray = paintBrush.GetRay();


            var hitCount = Physics.RaycastNonAlloc(ray, _hits, paintBrush.rayLength);

            if (hitCount == 0)
            {
                ray.direction *= -1;
                hitCount = Physics.RaycastNonAlloc(ray, _hits, paintBrush.rayLength);
            }

            for (int i = 0; i < hitCount; i++)
            {
                var hitInfo = _hits[i];
                if (hitInfo.transform != transform) continue;

                var hitInfoTextureCoord = hitInfo.textureCoord;
                
                maskMaterial.SetColor(DrawColor, Color.blue);

                maskMaterial.SetFloat(BrushStrength, paintBrush.Strength);

                maskMaterial.SetVector(Coordinate, new Vector4(hitInfoTextureCoord.x, hitInfoTextureCoord.y, 0, 0));

                maskMaterial.SetFloat(BrushSize, paintBrush.Size);

                var tempMask = RenderTexture.GetTemporary(1024, 1024, 0, RenderTextureFormat.ARGB64);

                Graphics.Blit(null, tempMask, maskMaterial);

                Graphics.Blit(tempMask, maskTexture);

                RenderTexture.ReleaseTemporary(tempMask);


                colorMaterial.SetColor(DrawColor, paintBrush.Color);

                colorMaterial.SetFloat(BrushStrength, paintBrush.Strength);

                colorMaterial.SetVector(Coordinate, new Vector4(hitInfoTextureCoord.x, hitInfoTextureCoord.y, 0, 0));

                colorMaterial.SetFloat(BrushSize, paintBrush.Size);

                var tempColor = RenderTexture.GetTemporary(1024, 1024, 0, RenderTextureFormat.ARGB64);

                Graphics.Blit(null, tempColor, colorMaterial);

                Graphics.Blit(tempColor, colorTexture);

                RenderTexture.ReleaseTemporary(tempColor);

                // 
            }
        }

        private void OnGUI()
        {
//            GUI.DrawTexture(new Rect(0, 0, 256, 256), maskTexture, ScaleMode.StretchToFill, false, 1);
//            GUI.DrawTexture(new Rect(0, 256, 256, 256), colorTexture, ScaleMode.StretchToFill, false, 1);
        }

        public void ClearAndSetCanvasBackground(Texture2D background)
        {
            if (colorTexture)
                colorTexture.Release();
            if (maskTexture)
                maskTexture.Release();

            maskTexture = new CustomRenderTexture(1024, 1024, RenderTextureFormat.ARGB64);

            colorTexture = new CustomRenderTexture(1024, 1024, RenderTextureFormat.ARGB64);

            colorMaterial.SetTexture(SplatMap, colorTexture);

            maskMaterial.SetTexture(SplatMap, maskTexture);

            mergeMaterial.SetTexture(MaskMap, maskTexture);

            mergeMaterial.SetTexture(ColorMap, colorTexture);

            mergeMaterial.SetTexture(BaseTexture, background);

            print(background);
        }
    }
}