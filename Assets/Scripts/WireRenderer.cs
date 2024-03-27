using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireRenderer : MonoBehaviour
{
    public Transform startObject;
    public Transform endObject;
    public Color lineColor = Color.red;

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, startObject.position);
        lineRenderer.SetPosition(1, endObject.position);
    }
}
