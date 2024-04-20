
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BreakPoint
{
    Vector3 posl,posr;
    parabolaRenderer relatedArc;
    public BreakPoint(Vector3 l,Vector3 r, parabolaRenderer pr )
    {
        posl = l;
        posr = r;
        relatedArc = pr;
    }
}

public class parabolaRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float resolution = 0.1f;
    public GameObject startObj;
    public GameObject endObj;
    public GameObject focusObj;
    public GameObject otherFocus;
    public GameObject eventLine;

    private Camera mainCamera;
    private Vector3 eventLinePos;
    private Vector3 cameraPos;

    public Vector2 breakPointL;
    public Vector2 breakPointR;
    public Vector3 start, end;
    public parabolaRenderer leftArc; //left up arc
    public parabolaRenderer rightArc; //right up arc


    public List<Vector3> points = new List<Vector3>();
    public List<BreakPoint> breakPoints = new List<BreakPoint>();
    public List<parabolaRenderer> arcs = new List<parabolaRenderer>();

    public int min = 9999, max = -9999;

    private void Start()
    {
        eventLine = GameObject.Find("eventline");
        startObj = GameObject.Find("start");
        endObj = GameObject.Find("end");
        focusObj = this.gameObject;
        mainCamera = Camera.main; // Get the main camera
        start = startObj.transform.position;
        end = endObj.transform.position;




       
       
    }

    void FixedUpdate()
    {
        if ((eventLinePos != eventLine.transform.position || cameraPos != mainCamera.transform.position)
            && transform.position.y >= eventLine.transform.position.y)
        {

            if(leftArc != null)
            {
                breakPointL = calcBreakPoint(this.gameObject, leftArc.gameObject).Item1;
                breakPointR = calcBreakPoint(this.gameObject, leftArc.gameObject).Item2;
            }
            if(rightArc != null)
            {
                breakPointL = calcBreakPoint(rightArc.gameObject, this.gameObject).Item1;
                breakPointR = calcBreakPoint(rightArc.gameObject, this.gameObject).Item2;
            }
            UpdateLine();
        }
        else if(transform.position.y < eventLine.transform.position.y){ lineRenderer.positionCount = 0; }

        
        
    }

    void UpdateLine()
    {
        cameraPos = mainCamera.transform.position;
        eventLinePos = eventLine.transform.position;
        lineRenderer.positionCount = 0;
        min = 9999;
        points.Clear();
        max = -9999;
        int numPoints = Mathf.RoundToInt(Vector3.Distance(startObj.transform.position, endObj.transform.position) / resolution);
        for (int i = 0; i <= numPoints; i++)
        {
            float t = i / (float)numPoints;
            Vector3 point = CalculateParabola(t);
            //points.Add(point);

            //if (IsVisible(point))
            //{
                if(point.x < min)
                {
                    min = (int)point.x;
                }
                if(point.x > max)
                {
                    max = (int)point.x;
                }

                if ((point.x >= breakPointL.x && point.x <= breakPointR.x) && rightArc == null) //has left
                {

                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);

                }
                if ((point.x >= breakPointR.x || point.x <= breakPointL.x) && leftArc == null) //has right
                {

                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);

                }


           // }
        }
    }

    Vector3 CalculateParabola(float t)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        direction /= distance;
        Vector3 position = start + direction * t * distance;
        float x = position.x - focusObj.transform.position.x;
        float y = (1 / (2 * (float)(focusObj.transform.position.y - eventLine.transform.position.y)))
            * (Mathf.Pow(x, 2))
            + ((focusObj.transform.position.y + eventLine.transform.position.y) / (float)2);
        position.y = y;
        return position;
    }

    private bool IsVisible(Vector3 worldPosition)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);
        return viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }

    public (Vector3, Vector3) calcBreakPoint(GameObject f1, GameObject f2)
    {
        Vector3 result = Vector3.zero;
        Vector3 result2 = Vector3.zero;
        float x1 = f1.transform.position.x;
        float y1 = f1.transform.position.y;
        float x2 = f2.transform.position.x;
        float y2 = f2.transform.position.y;
        float yd = eventLine.transform.position.y;

        float under = (Mathf.Pow(x1, 2) - 2 * x1 * x2 +
            Mathf.Pow(x2, 2) + Mathf.Pow((y1 - y2), 2)) / ((y1 - yd) * (y2 - yd));
        if(under < 0)
        {
            return (result, result);
        }
        float resX = (x2 * (y1 - yd) - (y2 - yd) *
            (x1 + Mathf.Sqrt(under) * (-y1 + yd)))/(y1-y2);

        float resX2 = (x2 * (y1 - yd) -
            ((x1 + (y1-yd)*Mathf.Sqrt(under)) * (y2 - yd))) / (y1 - y2);
        result.x = resX;
        result2.x = resX2;
        return (result,result2);
    }

    /* TO DRAW LINE FROM DIRECTION VECTOR TO OTHER ONE
     * Vector3 point_B = point_A + direction.normalized * desiredDistance;
     */
}
