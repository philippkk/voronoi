using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fortune
{
    /*
     * TODO: -21, 43
     *         7, 27
     *       0.8, 14.89
     * this case has the bottom one picking the wrong point as its above, search near broken or sum
     * 
     * 
     * DELETING CAUSES A FUCKING NODE TO LOsE ITS PARNET AND THATS WhAT FUCKS EVERYTHIN UP
     */



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
                    Vector2 breakLeft = calcBreakPoint(s.point, arcAbove.site.point, s.point.y - 0.1f).Item1;
                    Vector2 breakRight = calcBreakPoint(s.point, arcAbove.site.point, s.point.y - 0.1f).Item2;
                    /*
                     * ADD SOMETHING tO KEEP TRACK OF ORIGINAL AND DISPLAYED
                     */
                    Node bpLeft = new Node(breakLeft.x);
                    bpLeft.site = arcAbove.site;
                    bpLeft.site2 = s;
                    bpLeft.isLeft = true;
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
                    
                    //direction of edge
                    float midX = (1.0f / 2.0f) * (s.point.x + arcAbove.site.point.x);
                    float midY = (1.0f / 2.0f) * (s.point.y + arcAbove.site.point.y);
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


                    //get pos of breaks based off of the actual directX line
                    breakLeft = calcBreakPoint(s.point, arcAbove.site.point, directX).Item1;
                    breakRight = calcBreakPoint(s.point, arcAbove.site.point, directX).Item2;
                    edgeL.lineEnd = breakLeft;
                    edgeR.lineEnd = breakRight;
                    GameObject eObj = GameObject.Instantiate(drawable[0], s.point, Quaternion.identity);
                    drawnOjects.Add(eObj);
                    edgeController e = eObj.GetComponent<edgeController>();
                    e.lineKey = edgeL.key;
                    e.start = edgeL.startingDir;
                    e.end = breakLeft;


                    eObj = GameObject.Instantiate(drawable[0], s.point, Quaternion.identity);
                    drawnOjects.Add(eObj);
                    e = eObj.GetComponent<edgeController>();
                    e.start = edgeR.startingDir;
                    e.lineKey = edgeR.key;
                    e.end = breakRight;

                    
                    beachLine.delete(arcAbove);
                    beachLine.insert(edgeL);
                    beachLine.insert(edgeR);
                    //check for circle event
                    checkCircleEvent(drawnOjects,directX);

                }
            }
            else if(s.type == 1)//is circle event
            {
                if (directX <= n.key)
                {
                    handleCircleEvent(n, drawnOjects, directX);

                    Debug.Log("ON cIRCLE EVENT");
                }
            }


            eventQueue.delete(n);
            //check if site event


        }
        List<Node> visted = new List<Node>();
        beachLine.inorder(beachLine.root, visted, true);
    }
    /*
     * 
     * WHEN PRINTING BREAKPOINT MADE ARC MAKE SURE TO CHECK IS LEFT
     * IF IT IS THE RIGHT ARC THEN BASE FOCUS OFF SITE2
     */
    void handleCircleEvent(Node n,List<GameObject> drawnOjects, float directX)
    {
        //need to remove leftedge,rightedge,and nodetoremove
        //make rightedge and leftedge gameobject have set end before removing from beachline
        //add it at the site of the intersection and have it point down bisector

        //setting the left and right endpoints
        Debug.Log("edge " + n.leftEdge.direction + " " + n.leftEdge.startingDir + " " + n.leftEdge.site.point + " " + n.leftEdge.key);
        Debug.Log("edge " + n.rightEdge.direction + " " + n.rightEdge.startingDir + " " + n.rightEdge.site.point + " " + n.rightEdge.key);

        foreach (GameObject g in drawnOjects)
        {
            if (g.GetComponent<edgeController>() == null) { continue; }
            if (g.GetComponent<edgeController>().lineKey == n.leftEdge.key)
            {
                g.GetComponent<edgeController>().end = n.intersection;
            }
            if (g.GetComponent<edgeController>().lineKey == n.rightEdge.key)
            {
                g.GetComponent<edgeController>().end = n.intersection;
            }
        }

        //need to figure out when its a left or right edge
        //easy if left focus in lower in y than right focus its right else left
        //maybe not
       
        Node edgeR = new Node(n.intersection.x);

        //direction of edge
        float midX = (1.0f / 2.0f) * (n.leftEdge.site.point.x + n.rightEdge.site.point.x);
        float midY = (1.0f / 2.0f) * (n.leftEdge.site.point.y + n.rightEdge.site.point.y);
        float slope = -(1 / ((n.leftEdge.site.point.y - n.rightEdge.site.point.y) / (n.leftEdge.site.point.x - n.rightEdge.site.point.x)));
        edgeR.direction = (new Vector2(n.intersection.x + 0.1f, calcYline(slope, n.intersection.x + 0.1f, midX, midY))
                       - new Vector2(n.intersection.x + 0.0f, calcYline(slope, n.intersection.x + 0.0f, midX, midY))).normalized;
        edgeR.isEdge = true;
        edgeR.isLeft = false;
        edgeR.startingDir = new Vector2(n.intersection.x, calcYline(slope, n.intersection.x, midX, midY));
        edgeR.site = n.leftEdge.site;

        //get pos of breaks based off of the actual directX line
        //break should actually be the breakpoint between leftedge site and rightedge site
        Vector2 breakPoint = calcBreakPoint(n.leftEdge.site.point, n.rightEdge.site.point, directX).Item2;
        float directXpos = (directX / slope) - (midY / slope);
        edgeR.lineEnd = breakPoint;
        //edgeR.lineEnd = edgeR.startingDir + edgeR.direction * Vector2.Distance(new Vector2(directXpos,directX+3),n.intersection);
        GameObject eObj = GameObject.Instantiate(drawable[0], n.intersection, Quaternion.identity);
        drawnOjects.Add(eObj);
        edgeController e = eObj.GetComponent<edgeController>();
        e.start = n.intersection;
        e.lineKey = edgeR.key;
        e.end = edgeR.lineEnd;

        beachLine.delete(n.rightEdge);
        beachLine.delete(n.leftEdge);
        beachLine.delete(n.nodeToRemove);
        beachLine.insert(edgeR);




    }

    void checkCircleEvent(List<GameObject> drawnOjects,float directX)
    {// for every arc, find edge to the left and right of it, and see if the directions make em intersect
     //traverse the beachline starting from lowest x,
     //general method, check left,right,then parent till
        List<Node> nodes = new List<Node>();
        beachLine.inorder(beachLine.root,nodes,false);
        foreach (Node n in nodes)
        {
            if ((n.site != null || n.site2 != null)&&!n.isEdge)
            {

                //found an arc/breakpoint to check now find the edge to its left and right
                //after that see if those two edges collide as they both have the slope and shit then need
                //if the slopes are equal the are parallel, (or the same line), and wont intersect         
                //finding the two edges
                Node leftEdge = null;
                Node rightEdge = null;
                int startIndex = nodes.IndexOf(n);
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
              //  Debug.Log("possible " + n.site.point + " " + leftEdge.site.point + " " + rightEdge.site.point);
                if (leftEdge != null && rightEdge != null && leftEdge.site.point != rightEdge.site.point)
                {
                   
                    Vector2? intersection =  CheckForIntersection(leftEdge.startingDir, leftEdge.lineEnd, rightEdge.startingDir, rightEdge.lineEnd,drawnOjects);
                    if (intersection.HasValue)
                    {
                        //Debug.Log("left edge " + leftEdge.direction + " " + leftEdge.startingDir + " " + leftEdge.site.point);
                        //Debug.Log(n.site.point);
                        //Debug.Log("right edge " + rightEdge.direction + " " + rightEdge.startingDir + " " + rightEdge.site.point);
                        float directXInter = intersection.Value.y - Vector2.Distance(n.site.point, intersection.Value);
                       // Debug.Log("DIRECT X INTERSECTION Y VALUIE: " + directXInter);

                        //ADDING CIRCLE EVENT
                        Node circleNode = new Node(directXInter);
                        Site circleSite = new Site(new Vector2(intersection.Value.x, directXInter), 1);
                        circleNode.leftEdge = leftEdge;
                        circleNode.intersection = intersection.Value;
                        circleNode.rightEdge = rightEdge;
                        circleNode.nodeToRemove = n;
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
           // Debug.Log("Lines are parallel, no intersection.");
            return null;
        }
        else
        {
            float x = (b2 - b1) / (m1 - m2);
            float y = m1 * x + b1;
           // Debug.Log("Intersection at (" + x + ", " + y + ")");
            GameObject inter = GameObject.Instantiate(drawable[1], new Vector3(x, y, 0), Quaternion.identity);
            drawnOjects.Add(inter);
            return new Vector2(x, y);
            
        }
    }
}

