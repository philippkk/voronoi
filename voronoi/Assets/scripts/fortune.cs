using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.FlowStateWidget;

public class fortune
{
    public SplayTree beachLine = new SplayTree();
    SplayTree eventQueue = new SplayTree();
    private List<Node> eventSites = new List<Node>();
    List<GameObject> drawable = new List<GameObject>();

    public void CalcVoronoi(List<Site> sites,float directX,List<GameObject> drawable,List<GameObject> drawnOjects)
    {
        this.drawable = drawable;
        //SORT SIZES > y max
        sites.Sort((site1, site2) => site2.point.y.CompareTo(site1.point.y));
        foreach (Site s in sites)
        {
            Node n = new Node(s.point.y); //sort by y
            n.site = s;
            eventQueue.insert(n);
        }

        //SEARCHIN NEAR
        //Debug.Log(eventQueue.searchNear(eventQueue.root, 27).site.point.ToString());
        //Debug.Log(eventQueue.search(eventQueue.root, 7).site.point.ToString());

        while (eventQueue.root != null)
        {
            Node n = eventQueue.root;
            Site s;
            while(n.right != null)
            {
                n = n.right;
            }
            s = n.site;

            Debug.Log("Site pos: "+s.point.ToString() + "type:" + s.type);


            if(directX >= s.point.y && s.type != 1)
            {
                break;
            }
            if(s.type == 0) //if site event
            {
                if (beachLine.root == null) //new site into beach
                {
                    Node newSite = new Node(s.point.x);
                    newSite.site = s;
                    beachLine.insert(newSite);

                }
                else //not empty handle site event
                {
                    //beachLine.inorder(beachLine.root);
                    //find arc directly above event
                    Node arcAbove = beachLine.searchNear(beachLine.root, s.point.x,false);
                   // Debug.Log("ARC ABOVE: " + arcAbove.site.point.ToString());
                    //create new arc
                    Node newSite = new Node(s.point.x);
                    newSite.site = s;
                    beachLine.insert(newSite);
                    //SPLIT ARC INTO TWO
                    //calc break points;
                    Vector2 breakLeft = calcBreakPoint(s.point, arcAbove.site.point, directX).Item1;
                    Vector2 breakRight = calcBreakPoint(s.point, arcAbove.site.point, directX).Item2;
                    Node bpLeft = new Node(breakLeft.x);
                    bpLeft.site = arcAbove.site;
                    bpLeft.site2 = s;
                    bpLeft.startingDir = breakLeft;
                    Node bpRight = new Node(breakRight.x);
                    bpRight.site = s;
                    bpRight.site2 = arcAbove.site;
                    bpRight.startingDir = breakRight;
                    beachLine.insert(bpLeft);
                    beachLine.insert(bpRight);
                    //CALC EDGES
                    Node edgeL = new Node(newSite.site.point.x - 0.5f);
                    Node edgeR = new Node(newSite.site.point.x + 0.5f);
                    edgeL.site = edgeR.site = s;
                    edgeL.lineEnd = breakLeft;
                    edgeR.lineEnd = breakRight;
                    //direction of edge
                    float midX = (1.0f / 2.0f) * (newSite.site.point.x + arcAbove.site.point.x);
                    float midY = (1.0f / 2.0f) * (newSite.site.point.y + arcAbove.site.point.y);
                    float slope = -(1 / ((newSite.site.point.y - arcAbove.site.point.y) / (newSite.site.point.x - arcAbove.site.point.x)));
                    edgeL.direction = (new Vector2(s.point.x - 0.1f, calcYline(slope, s.point.x - 0.1f, midX, midY))
                        - new Vector2(s.point.x - 0.0f, calcYline(slope, s.point.x -0.0f, midX, midY))).normalized;
                    edgeL.isEdge = true;
                    edgeL.startingDir = new Vector2(s.point.x-0.1f, calcYline(slope, s.point.x - 0.1f, midX, midY));
                    edgeR.direction = (new Vector2(s.point.x + 0.1f, calcYline(slope, s.point.x + 0.1f, midX, midY))
                        - new Vector2(s.point.x + 0.0f, calcYline(slope, s.point.x + 0.0f, midX, midY))).normalized;
                    edgeR.isEdge = true;
                    edgeR.startingDir = new Vector2(s.point.x + 0.1f, calcYline(slope, s.point.x + 0.1f, midX, midY));
                    edgeL.midX = edgeR.midX = midX;
                    edgeL.midY = edgeR.midY = midY;
                    edgeL.slope = edgeR.slope = slope;

                    GameObject eObj = GameObject.Instantiate(drawable[0], s.point, Quaternion.identity);
                    drawnOjects.Add(eObj);
                    edgeController e = eObj.GetComponent<edgeController>();
                    e.start = edgeL.startingDir;
                    e.end = breakLeft;


                    eObj = GameObject.Instantiate(drawable[0], s.point, Quaternion.identity);
                    drawnOjects.Add(eObj);
                    e = eObj.GetComponent<edgeController>();
                    e.start = edgeR.startingDir;
                    e.end = breakRight;

                    beachLine.insert(edgeL);
                    beachLine.insert(edgeR);
                    beachLine.delete(arcAbove);

                    //check for circle event
                    checkCircleEvent(drawnOjects,directX);

                }
            }
            else if(s.type == 1)//is circle event
            {
                Debug.Log("CIRCLE MOTHER FUCKER");
                handleCircleEvent(drawnOjects, directX);
            }


            eventQueue.delete(n);
            //check if site event


        }
        List<Node> nodes = new List<Node>();
       // beachLine.inorder(beachLine.root,nodes,true);
    }

    void handleCircleEvent(List<GameObject> drawnOjects, float directX)
    {

    }

    void checkCircleEvent(List<GameObject> drawnOjects,float directX)
    {// for every arc, find edge to the left and right of it, and see if the directions make em intersect
     //traverse the beachline starting from lowest x,
     //general method, check left,right,then parent till
        List<Node> nodes = new List<Node>();
        beachLine.inorder(beachLine.root, nodes,false);
        foreach(Node n in nodes)
        {
            if ((n.site != null || n.site2 != null)&&!n.isEdge)
            {
                //found an arc/breakpoint to check now find the edge to its left and right
                //after that see if those two edges collide as they both have the slope and shit then need
                //if the slopes are equal the are parallel, (or the same line), and wont intersect

                //finding the two edges
                Node leftEdge = null;
                Node rightEdge = null;
                int startIndex = nodes.IndexOf(n) - 1;
                while(startIndex > -1)
                {
                    if (nodes[startIndex].isEdge)
                    {
                        leftEdge = nodes[startIndex];
                        break;
                    }
                    startIndex--;
                }
                startIndex = nodes.IndexOf(n) + 1;
                while (startIndex < nodes.Count)
                {
                    if (nodes[startIndex].isEdge)
                    {
                        rightEdge = nodes[startIndex];
                        break;
                    }
                    startIndex++;
                }
                if(leftEdge != null && rightEdge != null && leftEdge.site.point != rightEdge.site.point)
                {
                    //Debug.Log("edge " + leftEdge.direction + " " + leftEdge.startingDir + " " + leftEdge.site.point);
                    //Debug.Log("edge " + rightEdge.direction + " " + rightEdge.startingDir + " " + rightEdge.site.point);
                    Vector2? intersection =  CheckForIntersection(leftEdge.startingDir, leftEdge.lineEnd, rightEdge.startingDir, rightEdge.lineEnd,drawnOjects);
                    if (intersection.HasValue)
                    {
                        float directXInter = intersection.Value.y - Vector2.Distance(n.site.point, intersection.Value);
                       // Debug.Log("DIRECT X INTERSECTION Y VALUIE: " + directXInter);

                        //ADDING CIRCLE EVENT
                        Node circleNode = new Node(directXInter);
                        Site circleSite = new Site(new Vector2(intersection.Value.x, directXInter), 1);
                        circleNode.site = circleSite;
                        eventQueue.insert(circleNode);
                    }
                   
                }
            }

        }
    }


    public (Vector2, Vector2) calcBreakPoint(Vector2 f1, Vector2 f2,float directX)
    {
        Vector2 result = Vector2.zero;
        Vector2 result2 = Vector2.zero;
        float x1 = f1.x;
        float y1 = f1.y;
        float x2 = f2.x;
        float y2 = f2.y;
        float yd = directX;

        float under = (Mathf.Pow(x1, 2) - 2 * x1 * x2 +
            Mathf.Pow(x2, 2) + Mathf.Pow((y1 - y2), 2)) / ((y1 - yd) * (y2 - yd));
        if (under < 0)
        {
            return (result, result);
        }
        float resX = (x2 * (y1 - yd) - (y2 - yd) *
            (x1 + Mathf.Sqrt(under) * (-y1 + yd))) / (y1 - y2);

        float resX2 = (x2 * (y1 - yd) -
            ((x1 + (y1 - yd) * Mathf.Sqrt(under)) * (y2 - yd))) / (y1 - y2);
        result.x = resX;
        result2.x = resX2;

        float y = (1 / (2 * (y1 - directX)))
            * Mathf.Pow(resX-x1, 2)
            + ((y1 + directX) / (float)2);
        float y3 = (1 / (2 * (y1 - directX)))
            * Mathf.Pow(resX2 - x1, 2)
            + ((y1 + directX) / (float)2);
        result.y = y;
        result2.y = y3;

        return (result, result2);
    }
    public float calcYline(float m, float x, float Xx, float Yy)
    {
        return m * (x - Xx) + Yy;
    }
    public static Vector2? Intersect(float m1, float b1, float m2, float b2)
    {
        if (m1 == m2)
        {
            return null;
        }

        float x = (b2 - b1) / (m1 - m2);
        float y = m1 * x + b1;

        return new Vector2(x, y);
    }

    Vector2? CheckForIntersection(Vector2 line1Start, Vector2 line1End,Vector2 line2Start,Vector2 line2End, List<GameObject> drawnOjects)
    {
        float m1 = (line1End.y - line1Start.y) / (line1End.x - line1Start.x);
        float b1 = line1Start.y - m1 * line1Start.x;

        float m2 = (line2End.y - line2Start.y) / (line2End.x - line2Start.x);
        float b2 = line2Start.y - m2 * line2Start.x;

        if (m1 == m2)
        {
            Debug.Log("Lines are parallel, no intersection.");
            return null;
        }
        else
        {
            float x = (b2 - b1) / (m1 - m2);
            float y = m1 * x + b1;
            Debug.Log("Intersection at (" + x + ", " + y + ")");
            GameObject inter = GameObject.Instantiate(drawable[1], new Vector3(x, y, 0), Quaternion.identity);
            drawnOjects.Add(inter);
            return new Vector2(x, y);
            
        }
    }
}

