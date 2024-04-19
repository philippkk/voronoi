using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineRenderer : MonoBehaviour
{
    LineRenderer l;
    public List<Transform> points;

    // Start is called before the first frame update
    void Awake()
    {
        l = GetComponent<LineRenderer>();
    }

    public void setUpLine(List<Transform> points)
    {
        l.positionCount = points.Count;
        this.points = points;
        for (int i = 0; i < points.Count; i++)
        {
            l.SetPosition(i, points[i].position);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (points != null)
        {
            for (int i = 0; i < points.Count; i++)
            {
                l.SetPosition(i, points[i].position);
            }
        }
    }
}

