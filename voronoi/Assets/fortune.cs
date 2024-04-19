using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.FlowStateWidget;

public class fortune
{
    public SplayTree beachLine = new SplayTree();
    SplayTree eventQueue = new SplayTree();
    private List<Node> eventSites = new List<Node>();

    public void CalcVoronoi(List<Site> sites,float directX)
    {
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

            Debug.Log("Site pos: "+s.point.ToString());

            if(s.type == 0) //if site event
            {
                if(beachLine.root == null) //new site into beach
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
                    Debug.Log("ARC ABOVE: " + arcAbove.site.point.ToString());
                    //create new arc
                    Node newSite = new Node(s.point.x);
                    newSite.site = s;
                    beachLine.insert(newSite);
                    //SPLIT ARC INTO TWO
                    //calc break points;
                    Vector2 breakLeft = calcBreakPoint(s.point, arcAbove.site.point, s.point.y-0.5f).Item1;
                    Vector2 breakRight = calcBreakPoint(s.point, arcAbove.site.point, s.point.y-0.5f).Item2;
                    Node bpLeft = new Node(breakLeft.x);
                    bpLeft.site = arcAbove.site;
                    bpLeft.site2 = s;
                    Node bpRight = new Node(breakRight.x);
                    bpRight.site = s;
                    bpRight.site2 = arcAbove.site;
                    beachLine.insert(bpLeft);
                    beachLine.insert(bpRight);
                    //CALC EDGES
                    Node edgeL = new Node(newSite.site.point.x - 0.5f);
                    Node edgeR = new Node(newSite.site.point.x + 0.5f);
                    edgeL.site = edgeR.site = s;
                    //direction of edge
                    float midX = (1.0f / 2.0f) * (newSite.site.point.x + arcAbove.site.point.x);
                    float midY = (1.0f / 2.0f) * (newSite.site.point.y + arcAbove.site.point.y);
                    float slope = -(1 / ((newSite.site.point.y - arcAbove.site.point.y) / (newSite.site.point.x - arcAbove.site.point.x)));
                    edgeL.direction = -(new Vector2(midX, calcYline(slope, midX, midX, midY))
                        - new Vector2(midX - 0.5f, calcYline(slope, midX -0.5f, midX, midY))).normalized;
                    edgeL.isEdge = true;
                    edgeL.startingDir = new Vector2(bpLeft.key, calcYline(slope, bpLeft.key, midX, midY));
                    edgeR.direction = -(new Vector2(midX, calcYline(slope, midX, midX, midY))
                        - new Vector2(midX + 0.5f, calcYline(slope, midX + 0.5f, midX, midY))).normalized;
                    edgeR.isEdge = true;
                    edgeR.startingDir = new Vector2(bpRight.key, calcYline(slope, bpRight.key, midX, midY));
                    edgeL.midX = edgeR.midX = midX;
                    edgeL.midY = edgeR.midY = midY;
                    edgeL.slope = edgeR.slope = slope;
                    beachLine.insert(edgeL);
                    beachLine.insert(edgeR);
                    beachLine.delete(arcAbove);

                    //check for circle event
                    checkCircleEvent();

                }
            }
            else//is circle event
            {

            }


            eventQueue.delete(n);
            //check if site event


        }
        List<Node> nodes = new List<Node>();
        beachLine.inorder(beachLine.root,nodes,true);
    }


    void checkCircleEvent()
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
                //testing to see if found correctly and it seems good
                if(leftEdge != null && rightEdge != null)
                {
                    if(n.site2 != null)
                    {
                        //Debug.Log("breakpoint arc below");
                    }
                   // Debug.Log("left " + leftEdge.startingDir + " focus " + n.site.point + " right " + rightEdge.startingDir);

                }

                //now to do the actual intersection test
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
        return (result, result2);
    }
    public float calcYline(float m, float x, float Xx, float Yy)
    {
        return m * (x - Xx) + Yy;
    }
}
