using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CheckVisibility : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Camera mainCamera;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main; // Get the main camera
    }

    void Update()
    {
        CheckLineVisibility();
    }

    private void CheckLineVisibility()
    {
        bool isVisible = false;

        // Check each point in the LineRenderer
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pointPosition = lineRenderer.GetPosition(i);
            if (IsVisible(pointPosition))
            {
                isVisible = true;
                break;
            }
        }

        // Enable or disable based on visibility
        lineRenderer.enabled = isVisible;
    }

    private bool IsVisible(Vector3 worldPosition)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);
        // Check if the point is inside the camera view
        return viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }
}
