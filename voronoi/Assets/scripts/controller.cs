using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

public class controller : MonoBehaviour
{
    //GameObject cam;
    //public List<Transform> points = new List<Transform>();
    //public int numPoints;
    //public lineRenderer line;
    //public GameObject pointObject;
    public List<GameObject> pointLocations = new List<GameObject>();
    public List<Node> nodes = new List<Node>();
    public List<Site> sites = new List<Site>(0);
    public fortune f = new fortune();
    public GameObject eventLine;
    private Vector3 eventLinePos;
    public List<GameObject> drawnObjects = new List<GameObject>();
    //public List<GameObject> pointObjects = new List<GameObject>();
    //public DCEL dcel;
    // Start is called before the first frame update

    public GameObject edge,breakPoint;
    public List<GameObject> drawable = new List<GameObject>();
    LineRenderer lineRenderer;
    void Start()
    {
        drawable.Add(edge);
        drawable.Add(breakPoint);
        lineRenderer = GetComponent<LineRenderer>();
        eventLine = GameObject.Find("eventline");
        foreach(GameObject pos in pointLocations)
        {
            Site s = new Site(new Vector2(pos.transform.position.x, pos.transform.position.y),0);
            sites.Add(s);
            
        }
       



        //lineRenderer.positionCount = 4;
        //Vector3 point_B = new Vector2(-12.70f, -11.85f) + new Vector2(-0.89f, -0.46f) * 500;
        //lineRenderer.SetPosition(0, new Vector2(-7.43f, -9.11f));
        //lineRenderer.SetPosition(1, point_B);
        //Vector3 point_B2 = new Vector2(-12.70f, -11.85f) + new Vector2(0.89f, 0.46f) * 500;
        //lineRenderer.SetPosition(2, new Vector2(-6.90f, -8.83f));
        //lineRenderer.SetPosition(3, point_B2);
        #region old
        ////cam = GameObject.Find("Main Camera");
        ////dcel = new DCEL();
        ////Vertex v1 = dcel.createVertex(0, 0, null, 2);
        ////Vertex v2 = dcel.createVertex(1, 0, null, 2);
        ////Vertex v3 = dcel.createVertex(1, 1, null, 2);
        ////Vertex v4 = dcel.createVertex(0, 1, null, 2);
        ////HalfEdge e1 = dcel.createEdge(v1, null, null, null, 1);
        ////HalfEdge e2 = dcel.createEdge(v2, null, null, null, 1);
        ////HalfEdge e3 = dcel.createEdge(v3, null, null, null, 1);
        ////HalfEdge e4 = dcel.createEdge(v4, null, null, null, 1);
        ////e1.setNext(e2, e4);
        ////e2.setNext(e3, e1);
        ////e3.setNext(e4, e2);
        ////e4.setNext(e1, e3);
        ////e1.setTwin();
        ////e2.setTwin();
        ////e3.setTwin();
        ////e4.setTwin();
        ////Face f1 = dcel.createFace(null, e1, 1);
        ////Face f2 = dcel.createFace(e1.getTwin(), null, 1);


        ////genRandomPoints();
        //// jarvisMarch();

        ////Vertex v1 = dcel.createVertex(0, 0, null, 2);
        ////Vertex v2 = dcel.createVertex(1, 0, null, 2);
        ////HalfEdge e1 = dcel.createEdge(v1, null, null, null, 1);
        ////e1.setNext(e1, e1);
        ////e1.setTwin();
        ////Face f1 = dcel.createFace(null, e1, 1);



        ////foreach (Vertex v in dcel.vertices)
        ////{
        ////    Debug.Log(v.ToString());
        ////}
        ////HalfEdge firstOfTri = null;
        ////GameObject tri = null;
        ////foreach (HalfEdge e in dcel.edges)
        ////{
        ////    Debug.Log(e.ToString());
        ////    Debug.Log(e.getTwin().ToString());


        ////    //need to draw line from orgin to next...
        ////    if (!displayedEdges.Contains(e))
        ////    {
        ////        GameObject p = Instantiate(pointObject, new Vector3((float)e.getOrigin().getX(), (float)e.getOrigin().getY(), 1), Quaternion.identity);
        ////        if (firstOfTri == null)
        ////        {
        ////            firstOfTri = e;
        ////            tri = p;
        ////        }
        ////        points.Add(p.transform);
        ////        displayedEdges.Add(e);
        ////    }
        ////    if (!displayedEdges.Contains(e.getNext()))
        ////    {
        ////        GameObject p2 = Instantiate(pointObject, new Vector3((float)e.getNext().getOrigin().getX(), (float)e.getNext().getOrigin().getY(), 1), Quaternion.identity);
        ////        points.Add(p2.transform);
        ////        displayedEdges.Add(e.getNext());
        ////    }
        ////    else if(e.getNext() == firstOfTri)
        ////    {
        ////        points.Add(tri.transform);
        ////        firstOfTri = null;
        ////    }


        ////}
        ////foreach (Face f in dcel.faces)
        ////{
        ////    Debug.Log(f.ToString());
        ////}

        ////  line.setUpLine(points);
        #endregion

    }

    #region old
    //void genRandomPoints()
    //{
    //    for(int i = 0; i < numPoints; i++)
    //    {
    //        int x = Random.Range(-50, 50);
    //        int y = Random.Range(-50, 50);
    //        if(pointLocations.Contains(new Vector3(x, y, 0)))
    //        {
    //            continue;
    //        }

    //        GameObject p = Instantiate(pointObject, new Vector3(x, y, 0), Quaternion.identity);
    //        pointObjects.Add(p);
    //        pointLocations.Add(p.transform.position);
    //    }
    //}

    //void jarvisMarch()
    //{
    //    points.Clear();
    //    List<GameObject> workinList = new List<GameObject>(pointObjects);
    //    List<GameObject> hull = new List<GameObject>();

    //    float min = -99999;
    //    GameObject point = null;
    //    GameObject ogPoint = null;
    //    GameObject next = null;
    //    for (int i = 0; i < pointObjects.Count; i++)
    //    {

    //        if (pointObjects[i].transform.position.x > min)
    //        {
    //            min = pointObjects[i].transform.position.x;
    //            point = pointObjects[i];
    //        }
    //    }

    //    points.Add(point.transform);
    //    ogPoint = point;
    //    float lastAngle = 0;
    //    float absoluteAngle = 0;
    //    int uhoh = 0;
    //    do
    //    {
    //        uhoh++;
    //        if (uhoh > 100)
    //        {
    //            break;
    //        }
    //        float angle = 9999999;
    //        //sort by polar and pick smallest
    //        for (int i = 0; i < workinList.Count; i++)
    //        {
    //            if (workinList[i] == point)
    //            {
    //                continue;
    //            }
    //            float relativeX = workinList[i].transform.position.x - point.transform.position.x;
    //            float relativeY = workinList[i].transform.position.y - point.transform.position.y;

    //            float testAngle = Mathf.Atan2(relativeY, relativeX);
    //            if (testAngle < 0)
    //            {
    //                testAngle += Mathf.PI * 2;
    //            }
    //            testAngle -= lastAngle;
    //            if (testAngle < 0)
    //            {
    //                testAngle += Mathf.PI * 2;
    //            }
    //            if (testAngle < angle)
    //            {
    //                angle = testAngle;
    //                next = workinList[i];
    //                absoluteAngle = Mathf.Atan2(relativeY, relativeX);
    //            }
    //        }
    //        points.Add(next.transform);
    //        hull.Add(next);
    //        point = next;
    //        workinList.Remove(next);
    //        if (absoluteAngle < 0)
    //        {
    //            absoluteAngle += Mathf.PI * 2;
    //        }
    //        lastAngle = absoluteAngle;


    //    } while (ogPoint != point);


    //    float camX = 0, camY = 0;
    //    foreach (GameObject g in hull)
    //    {
    //        camX += g.transform.position.x;
    //        camY += g.transform.position.y;
    //    }
    //    //camX = camX / hull.Count;
    //    //camY = camY / hull.Count;
    //    //cam.transform.position = new Vector3(camX, camY, cam.transform.position.z);
    //}
    #endregion
    // Update is called once per frame
    void FixedUpdate()
    {
        if (eventLinePos != eventLine.transform.position)
        {
            foreach(GameObject g in drawnObjects)
            {
                Destroy(g);
             
            }
            ClearLog();
            drawnObjects.Clear();
            eventLinePos = eventLine.transform.position;
            f = new fortune();
            f.CalcVoronoi(sites,eventLine.transform.position.y,drawable,drawnObjects);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);

        }

    }


    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }




}