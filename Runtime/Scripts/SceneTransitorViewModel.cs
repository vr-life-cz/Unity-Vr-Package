using System;
using UnityEngine;


public class SceneTransitorViewModel : MonoBehaviour
{
    public Material material;
    public Color color;
    private void Awake()
    {
        color = new Color(0,0,0,1);
    }
}