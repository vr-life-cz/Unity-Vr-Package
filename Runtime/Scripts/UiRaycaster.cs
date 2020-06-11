using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiRaycaster : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        layerMask = LayerMask.GetMask("UI");
    }

    // Update is called once per frame
    void Update()
    {
        HandleUiRaycast();
    }
    
    public void HandleUiRaycast()
    {
        RaycastHit hit;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 200,
                Color.green);
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.enabled = true;
            lineRenderer.SetPositions(new Vector3[]
            {
                transform.position,
                transform.position + 20 * transform.forward
            });
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1, Color.red);
            lineRenderer.enabled = false;
        }
    }
}
