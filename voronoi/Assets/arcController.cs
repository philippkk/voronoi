using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arcController : MonoBehaviour
{
    LineRenderer lr;
    public float key;
    public Vector3 start, end;
    public Vector3 focus;
    public GameObject eventLine;
    public float resolution = 0.1f;
    private Vector3 eventLinePos;

    // Start is called before the first frame update
    void Start()
    {
        eventLine = GameObject.Find("eventline");
        lr = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        if ((eventLinePos != eventLine.transform.position)
            && transform.position.y >= eventLine.transform.position.y)
        {
            UpdateLine();
        }
        else if (transform.position.y < eventLine.transform.position.y) { lr.positionCount = 0; }



    }

    void UpdateLine()
    {
        lr.positionCount = 0;
        int numPoints = Mathf.RoundToInt(Vector3.Distance(start, end) / resolution);
        for (int i = 0; i <= numPoints; i++)
        {
            float t = i / (float)numPoints;
            Vector3 point = CalculateParabola(t);
            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, point);

        }
    }

    Vector3 CalculateParabola(float t)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        direction /= distance;
        Vector3 position = start + direction * t * distance;
        float x = position.x - focus.x;
        float y = (1 / (2 * (float)(focus.y - eventLine.transform.position.y)))
        * (Mathf.Pow(x, 2))
            + ((focus.y + eventLine.transform.position.y) / (float)2);
        position.y = y;
        return position;
    }

}
