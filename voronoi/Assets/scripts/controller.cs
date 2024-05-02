using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using TMPro;

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

    public DCEL dcel;
    public DCEL ddcel;
    public GameObject edge,breakPoint;
    public List<GameObject> drawable = new List<GameObject>();
    public Dictionary<float, GameObject> drawnObject = new Dictionary<float, GameObject>();
    LineRenderer lineRenderer;
    float lowestPoint = 99999;

    bool showTriangle = false;


    public TMP_Text dcelText;
    public TMP_Text ddcelText;

    public List<Vector2> loadedPoints = PointLoader.LoadPoints("Assets/scripts/points.txt");


    void Start()
    {
        drawable.Add(edge);
        drawable.Add(breakPoint);
        lineRenderer = GetComponent<LineRenderer>();
        eventLine = GameObject.Find("eventline");
        loadedPoints = PointLoader.LoadPoints("Assets/scripts/points.txt");


        foreach (Vector2 v in loadedPoints)
        {
            GameObject point = GameObject.Instantiate(drawable[4], v, Quaternion.identity);
            pointLocations.Add(point);
        }

        for (int i =0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.active)
                pointLocations.Add(transform.GetChild(i).gameObject);
        }


        foreach (GameObject pos in pointLocations)
        {
            Site s = new Site(new Vector2(pos.transform.position.x, pos.transform.position.y),0);
            sites.Add(s);
            if (pos.transform.position.y < lowestPoint)
            {
                lowestPoint = pos.transform.position.y;
            }
            
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //string currentSceneName = SceneManager.GetActiveScene().name;
            //SceneManager.LoadScene(currentSceneName);
            sites.Clear();
            foreach (GameObject pos in pointLocations)
            {
                Site s = new Site(new Vector2(pos.transform.position.x, pos.transform.position.y), 0);
                sites.Add(s);
                if (pos.transform.position.y < lowestPoint)
                {
                    lowestPoint = pos.transform.position.y;
                }
            }

            foreach (GameObject g in drawnObjects)
            {
                if (g != null)
                {
                    if (g.GetComponent<arcController>() == null)
                    {
                        Destroy(g);
                    }
                }
            }
            drawnObjects.RemoveAll(s => s == null);
            ClearLog();
            eventLinePos = eventLine.transform.position;
            f = new fortune();
            f.triangle = showTriangle;
            dcel = f.CalcVoronoi(sites, eventLine.transform.position.y, drawable, drawnObjects,lowestPoint).Item1;

        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            showTriangle = !showTriangle;
            doCalcs();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StringWriter.WriteString(dcelText.text, "Assets/output/voronoiDCEL.txt");
            StringWriter.WriteString(ddcelText.text, "Assets/output/delaunayDCEL.txt");
        }
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
            doCalcs();
        }
       
    }


    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }



    void doCalcs()
    {
        foreach (GameObject g in drawnObjects)
        {
            if (g != null)
            {
                if (g.GetComponent<arcController>() == null)
                {
                    Destroy(g);
                }
            }
        }
        drawnObjects.RemoveAll(s => s == null);
        ClearLog();
        eventLinePos = eventLine.transform.position;
        f = new fortune();
        f.triangle = showTriangle;
        (DCEL, DCEL) dcels = f.CalcVoronoi(sites, eventLine.transform.position.y, drawable, drawnObjects, lowestPoint);
        dcel = dcels.Item1;
        ddcel = dcels.Item2;
        dcelText.text = ddcelText.text = "";


        foreach (Vertex v in dcel.vertices)
        {
            if (v.bound)
            {
                dcelText.text += "\n";
                dcelText.text += "b" + (v.boundIndex) + " " + v.point + " e";
            }
            else
            {
                dcelText.text += "\n";
                dcelText.text += "v" + (dcel.vertices.IndexOf(v) + 1) + " " + v.point + " e";

            }
            if (v.incidentEdge.origin != null)
            {
                if (v.incidentEdge.origin.bound)
                {
                    dcelText.text += "b" + v.incidentEdge.origin.boundIndex;
                }
                else
                {
                    dcelText.text += (dcel.vertices.IndexOf(v.incidentEdge.origin) + 1);
                }
            }
            else { dcelText.text += 0; }
            if (v.incidentEdge.destination != null)
            {
                if (v.incidentEdge.destination.bound)
                {
                    dcelText.text += ",b" + v.incidentEdge.destination.boundIndex;
                }
                else
                {
                    dcelText.text += "," + (dcel.vertices.IndexOf(v.incidentEdge.destination) + 1);
                }
            }
            else { dcelText.text += ",0"; }
        }
        dcelText.text += "\n";
        foreach (Face f in dcel.faces)
        {
            if (f.type == 1)
            {
                dcelText.text += "\n";
                dcelText.text += "uf nil";
                if (f.innerComponent != null)
                {
                    if (f.innerComponent.origin.bound)
                    {
                        dcelText.text += " eb" + f.innerComponent.origin.boundIndex;
                    }
                    else
                    {
                        dcelText.text += " e" + (dcel.vertices.IndexOf(f.innerComponent.origin) + 1);
                    }
                    if (f.innerComponent.destination.bound)
                    {
                        dcelText.text += ",b" + f.innerComponent.destination.boundIndex;
                    }
                    else
                    {
                        dcelText.text += "," + (dcel.vertices.IndexOf(f.innerComponent.destination) + 1);
                    }
                }
                else
                {
                    dcelText.text += " nil";
                }


                //dcelText.text += " nil";
            }
            else
            {
                dcelText.text += "\n";
                dcelText.text += "c" + (dcel.faces.IndexOf(f) + 1);
                if (f.outterComponent != null)
                {
                    if (f.outterComponent.origin == null) { continue; }
                    if (f.outterComponent.origin.bound)
                    {
                        dcelText.text += " b" + f.outterComponent.origin.boundIndex;
                    }
                    else
                    {
                        dcelText.text += " " + (dcel.vertices.IndexOf(f.outterComponent.origin) + 1);
                    }
                    if (f.outterComponent.destination == null) { continue; }
                    if (f.outterComponent.destination.bound)
                    {
                        dcelText.text += ",b" + f.outterComponent.destination.boundIndex;
                    }
                    else
                    {
                        dcelText.text += "," + (dcel.vertices.IndexOf(f.outterComponent.destination) + 1);
                    }
                }
                else
                {
                    dcelText.text += " nil";
                }
                if (f.innerComponent != null)
                {
                    if (f.innerComponent.origin.bound)
                    {
                        dcelText.text += " eb" + f.innerComponent.origin.boundIndex;
                    }
                    else
                    {
                        dcelText.text += "b" + (dcel.vertices.IndexOf(f.innerComponent.origin) + 1);
                    }
                    if (f.innerComponent.destination.bound)
                    {
                        dcelText.text += ",b" + f.innerComponent.destination.boundIndex;
                    }
                    else
                    {
                        dcelText.text += "," + (dcel.vertices.IndexOf(f.innerComponent.destination) + 1);
                    }
                }
                else
                {
                    dcelText.text += " nil";
                }

            }





        }
        dcelText.text += "\n";
        foreach (HalfEdge e in dcel.edges)
        {

            //e.next.prev = e;
           
            dcelText.text += "\n";
            dcelText.text += "e";
            if (e.origin == null) { dcelText.text += " nil"; }
            else
            {
                if (e.origin.bound)
                {
                    dcelText.text += "b" + e.origin.boundIndex;
                }
                else
                {
                    dcelText.text += (dcel.vertices.IndexOf(e.origin) + 1);
                }
            }
         
            if (e.destination != null)
            {
                if (e.destination.bound)
                {
                    dcelText.text += ",b" + e.destination.boundIndex;
                }
                else
                {
                    dcelText.text += "," + (dcel.vertices.IndexOf(e.destination) + 1);
                }
            }
            else
            {
                dcelText.text += ",nil";
            }



            if (e.origin == null) { dcelText.text += " nil"; }
            else
            {
                if (e.origin.bound)
                {
                    dcelText.text += " b" + e.origin.boundIndex;
                }
                else
                {
                    dcelText.text += " v" + (dcel.vertices.IndexOf(e.origin) + 1);
                }

            }



            if (e.twin == null) { dcelText.text += " nil"; }
            else
            {
                dcelText.text += " e";
                if (e.twin.origin != null)
                {
                    if (e.twin.origin.bound)
                    {
                        dcelText.text += "b" + e.twin.origin.boundIndex;
                    }
                    else
                    {
                        dcelText.text += (dcel.vertices.IndexOf(e.twin.origin) + 1);
                    }
                }

                if (e.twin.destination != null)
                {
                    if (e.twin.destination.bound)
                    {
                        dcelText.text += ",b" + e.twin.destination.boundIndex;
                    }
                    else
                    {
                        dcelText.text += "," + (dcel.vertices.IndexOf(e.twin.destination) + 1);
                    }
                }


            }
            if (e.incidentFace == null) { dcelText.text += " nil"; }
            else
            {
                dcelText.text += " c" + (dcel.faces.IndexOf(e.incidentFace) + 1);
            }
            if (e.next == null) { dcelText.text += " nil"; }
            else
            {
                dcelText.text += " e";
                if (e.next.origin == null)
                {
                    dcelText.text += " nil";
                }
                else
                {
                    if (e.next.origin.bound)
                    {
                        dcelText.text += "b" + e.next.origin.boundIndex;
                    }
                    else
                    {
                        dcelText.text += (dcel.vertices.IndexOf(e.next.origin) + 1);
                    }
                }
                if (e.next.destination == null) { dcelText.text += " nil"; }
                else
                {
                    if (e.next.destination.bound)
                    {
                        dcelText.text += ",b" + e.next.destination.boundIndex;
                    }
                    else
                    {
                        dcelText.text += "," + (dcel.vertices.IndexOf(e.next.destination) + 1);
                    }
                }
            }

            if (e.prev == null) { dcelText.text += " nil"; }
            else
            {
                dcelText.text += " e";
                if (e.prev.origin == null) { dcelText.text += " nil"; }
                else
                {
                    if (e.prev.origin.bound)
                    {
                        dcelText.text += "b" + e.prev.origin.boundIndex;
                    }
                    else
                    {
                        dcelText.text += (dcel.vertices.IndexOf(e.prev.origin) + 1);
                    }
                }

                if (e.prev.destination == null) { dcelText.text += " nil"; }
                else
                {
                    if (e.prev.destination.bound)
                    {
                        dcelText.text += ",b" + e.prev.destination.boundIndex;
                    }
                    else
                    {
                        dcelText.text += "," + (dcel.vertices.IndexOf(e.prev.destination) + 1);
                    }
                }
            }


        }



        Debug.Log("vertex count: " + dcel.vertices.Count + " edge count: " + dcel.edges.Count + " face count: " + dcel.faces.Count);


        foreach (Vertex v in ddcel.vertices)
        {

           ddcelText.text += "p" + (ddcel.vertices.IndexOf(v) + 1) + " " + v.point + " e";
            if (v.incidentEdge.origin != null)
            {

              ddcelText.text += (ddcel.vertices.IndexOf(v.incidentEdge.origin) + 1);
            }
            else { ddcelText.text += 0; }
            if (v.incidentEdge.destination != null)
            {
               ddcelText.text += "," + (ddcel.vertices.IndexOf(v.incidentEdge.destination) + 1);
            }
            else { ddcelText.text += ",0"; }
            ddcelText.text += "\n";
        }
        ddcelText.text += "\n";
        foreach (Face f in ddcel.faces)
        {
            if (f.type == 1)
            {
                ddcelText.text += "uf nil";
                if (f.innerComponent != null)
                {
                        ddcelText.text += " d" + (ddcel.vertices.IndexOf(f.innerComponent.origin) + 1);

                        ddcelText.text += "," + (ddcel.vertices.IndexOf(f.innerComponent.destination) + 1);
                }
                else
                {
                    ddcelText.text += " nil";
                }


                //ddcelText.text += " nil";
            }
            else
            {
                ddcelText.text += "t" + (ddcel.faces.IndexOf(f) + 1);
                if (f.outterComponent != null)
                {
                        ddcelText.text += " d" + (ddcel.vertices.IndexOf(f.outterComponent.origin) + 1);
                        ddcelText.text += "," + (ddcel.vertices.IndexOf(f.outterComponent.destination) + 1);
                }
                else
                {
                    ddcelText.text += " nil";
                }
                if (f.innerComponent != null)
                {
                        ddcelText.text += "d" + (ddcel.vertices.IndexOf(f.innerComponent.origin) + 1);
                        ddcelText.text += "," + (ddcel.vertices.IndexOf(f.innerComponent.destination) + 1);
                }
                else
                {
                    ddcelText.text += " nil";
                }

            }


            ddcelText.text += "\n";


        }
        ddcelText.text += "\n";
        foreach (HalfEdge e in ddcel.edges)
        {

            //e.next.prev = e;

            if (e.origin == null) { continue; }
            ddcelText.text += "d";
                ddcelText.text += (ddcel.vertices.IndexOf(e.origin) + 1);
                ddcelText.text += "," + (ddcel.vertices.IndexOf(e.destination) + 1);


            if (e.origin == null) { ddcelText.text += " nil"; }
            else
            {
                    ddcelText.text += " v" + (ddcel.vertices.IndexOf(e.origin) + 1);

            }



            if (e.twin == null) { ddcelText.text += " nil"; }
            else
            {
                ddcelText.text += " d";
                if (e.twin.origin != null)
                {
                   
                        ddcelText.text += (ddcel.vertices.IndexOf(e.twin.origin) + 1);
                }

                if (e.twin.destination != null)
                {
                  
                        ddcelText.text += "," + (ddcel.vertices.IndexOf(e.twin.destination) + 1);
                }


            }
            if (e.incidentFace == null) { ddcelText.text += " nil"; }
            else
            {
                ddcelText.text += " t" + (ddcel.faces.IndexOf(e.incidentFace) + 1);
            }
            if (e.next == null) { ddcelText.text += " nil"; }
            else
            {
                ddcelText.text += " d";
                    ddcelText.text += (ddcel.vertices.IndexOf(e.next.origin) + 1);
             
                    ddcelText.text += "," + (ddcel.vertices.IndexOf(e.next.destination) + 1);
            }
            if (e.prev == null) { ddcelText.text += " nil"; }
            else
            {
                ddcelText.text += " d";
                    ddcelText.text += (ddcel.vertices.IndexOf(e.prev.origin) + 1);
                    ddcelText.text += "," + (ddcel.vertices.IndexOf(e.prev.destination) + 1);
            }
            ddcelText.text += "\n";
        }

    }
}
