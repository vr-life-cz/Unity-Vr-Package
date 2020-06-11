using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public RaycastHit HandleUiRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 3, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance,
                Color.green);
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.enabled = true;
            lineRenderer.SetPositions(new Vector3[]
            {
                transform.position,
                transform.position + hit.distance * transform.forward
            });
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1, Color.red);
            lineRenderer.enabled = false;
        }

        return hit;
    }
}
