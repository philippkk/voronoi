using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventlineController : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMoving = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
        }

        if (isMoving)
        {
            float distanceFromCamera = 10f;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = distanceFromCamera;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);

            targetPosition = new Vector3( worldPoint.x, worldPoint.y,transform.position.z);
            // Smoothly move towards the target position
            float moveSpeed = 5f; // Adjust as needed
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        }
    }
}
