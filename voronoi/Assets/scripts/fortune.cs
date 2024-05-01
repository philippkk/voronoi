using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using System.Linq;

public class fortune
{
    /*
     * TODO: -21, 43
     *         7, 27
     *       0.8, 14.89
     * this case has the bottom one picking the wrong point as its above, search near broken or sum
     *      idk checked again and issue seems to be linked to circle event detection
     * 
     * issue of now when a circle event created an edge that intersects with a past
     * cricle event later on
     *      found that left and right edge of circle event was not being found correctly
     *      maybe i fixed it but now an issue where the wrong edge is being deleted for some reason
     * 
     * realized im over checkin for cirlce evnts let me fix that real quick and see what the ends up
     * changin
     * 
     * NOW im getting a think where its picking the wrong above arc, causing the beachline to
     * be fucked up. i think fixign this will help the first case i found
     *      I think ive gotta check if its a breakpoint when looking it up,
     *      then calc where it is depending on the two sites it has
     *      then thats how i check : )
     * whats goin on is that when i add a breakpoint, its directly right and left of the site its from
     * the visual does not represent the beachline, 
     * 
     * what i gotta do is check the y value of the parabola of all others in beach?
     * prob just traverse tree till i get to a node that is too small then the right is too big
     * that sorta thing
     *      use calc beakpoint, need to also take into account where breakpoints are in relation
     *          to directX if it lands on one instead of a plane arc
     *          Method:
     *              look for closest point, with calced breakpoint
     *                  if found breakpoint and its to the right of its key,
     *                  use its site 1 to check where the new site intersects on the arc,
     *                  if its to the left of the breakpoint use site 1, else use site 2
     *      edges might be stored wrong need to do more research            
     *      
     *      try messing with the actual stored location of breakpoints - no
     *      
     *      fixed it?
     *      
     *      
     *      something is a little goofy with circle events too <- current issue
     *          todo: first make parabola render to see beachline more clearly to help debug
     *                profit 
     *          todo: invalidate past cirlce events taht odnt work
     *              when adding new point, or when doing the circle event,
     *              check if inside the circle just using Vector2.distance and seeing if its 
     *              less than one of the points of the cirlce
     *          todo: right breakpoint logic seems wrong 
     *          
     *       ISSUE: sometimes a breakpoint from some other line can over take an other arc which
     *       causes a new site to be found incorrectly. need to do proper traversal to find
     *       correct arc
     *       
     *       ive now distinquished the left and right breakpoints
     *       start at root
     *       if node.isEdge, that means either its left or right has to be a breakpoint
     *          if s.x > node.x
     *              go to node to right i
     *       
     *          STILL FUCKING GETTING THE WRONG ARC ABOVE, I THOUGHT IT WAS WORKING
     *          
     *          
     *          todo: now it still doesnt pick the right arc above (sometimes???)
     *          todo: also need a list to keep track of the new verticies and also probably
     *              modifiy things a bit to keep track of were edges are goin and where the came from
     *              just to make the dcel part easier
     *   fixed! todo: now if point1 nad 3 are same x its error for some reason idk searchnear returns nothin
     *              
     *              current issue seems that line end is not being updated properly
     *              lets see if i can get that figured out
     *             
     *          todo: ok almost getting there, now there are two issues, one idk what it is just where it happens
     *                  and two being something is tied to the directx somewhere that shouldnt be
     *      yes Maybe: remove circle events involving the arcs in the thing.
     *    done! todo: figure out how to do the triple arc removal shit 
     *          todo: now have the issue where things should update as it goes from site to site
     *          todo: i think if i just rewrite the delete method to keep the internal
     *              node order idea i can find the correct arc
     *          todo: write a new fucking way to insert and shit
     *              just follow the two sites, it shouldnt be that bad
     *              
     *          todo: now it is checking an arc that should intersect but its returning somwhere
     *                  that it should not be, easy fix
     *                  
     *                  
     *          todo: ok now its just edge cases,
     *              todo: first is co linear points, ill get an infinite slope which is an issue
     *                  i think main issue would be the infity value made by the
     *                  intersection and slope calcs.
     *                  for cocircular points i can hard code a detection for it.
     *                  
     *                  
     *          todo: still aint got the edge cases done, but DCEL issues
     *              neeed to make a check to see if origin/destination/incidentedge is already set
     *              if it is just dont mess with it tbh
     *              should probably debug out the edges first before the check as thatll help a lot i bet
     *              
     *              
     *              
     *              todo:directions from edges seem off?
     *              
     *              
     *         todo: something is wrong in dcel makin just double check the values.
     *         
     *         
     *         todo: FOR TRIANGLE ADD FACE TO SITE AND MAKE SITE LIST
     * fixed
     * DELETING CAUSES A FUCKING NODE TO LOsE ITS PARNET AND THATS WhAT FUCKS EVERYTHIN UP
     */



    public SplayTree beachLine = new SplayTree();
    SplayTree eventQueue = new SplayTree();
    List<GameObject> drawable = new List<GameObject>();
    public DCEL dcel = new DCEL();

    public float highx = -9999;
    public float lowx = 9999;
    public float highy = -9999;
    public float lowy = 9999;

    public DCEL CalcVoronoi(List<Site> sites, float directX, List<GameObject> drawable, List<GameObject> drawnOjects, float lowestPoint)
    {

        this.drawable = drawable;
        //SORT SIZES > y max
        sites.Sort((site1, site2) => site2.point.y.CompareTo(site1.point.y));
        foreach (Site s in sites)
        {
            Node test = eventQueue.search(eventQueue.root, s.point.y);
            if(test != null)
            {
                bool valid = false;
                float additive = 0.005f;
                while (!valid)
                {
                    if(eventQueue.search(eventQueue.root,s.point.y-additive)!= null)
                    {
                        additive += 0.001f;
                    }
                    else
                    {
                        valid = true;
                        break;
                    }
                }
                Node n = new Node(s.point.y - additive); //sort by y add a little offset to not have colinear shit
                n.site = s;
                eventQueue.insert(n, true);
            }
            else
            {
                Node n = new Node(s.point.y); //sort by y
                n.site = s;
                eventQueue.insert(n, true);
            }
            
        }

        //SEARCHIN NEAR
        List<Node> visted = new List<Node>();
        while (eventQueue.root != null)
        {
            Node n = eventQueue.root;
            Site s;
            while(n.right != null)
            {
                n = n.right;
            }
            s = n.site;
            //Debug.Log("Site pos: " + s.point.ToString() + "type:" + s.type);

            if (directX >= s.point.y)
            {
                break;
            }
            if(s.type == 0) //if site event
            {
                if (beachLine.root == null) //new site into beach
                {
                    Node newSite = new Node(s.point.x);
                    newSite.site = s;
                    beachLine.insert(newSite,true);

                    Face f = new Face();
                    f.site = s;
                    dcel.faces.Add(f);
                    s.face = f;

                    if (s.point.x < lowx) { lowx = s.point.x; };
                    if (s.point.x > highx) { highx = s.point.x; };
                    if (s.point.y < lowy) { lowy = s.point.y; };
                    if (s.point.y > highy) { highy = s.point.y; };

                    #region arcobject
                    //bool found = false;
                    //foreach(GameObject g in drawnOjects) {
                    //    if(g.GetComponent<arcController>() != null)
                    //    {
                    //        if(Mathf.Approximately(g.GetComponent<arcController>().key, s.point.x))
                    //        {
                    //            found = true;
                    //        }
                    //    }
                    //}
                    //if (!found)
                    //{
                    //    GameObject arc = GameObject.Instantiate(drawable[3], new Vector3(s.point.x, s.point.y, 0), Quaternion.identity);
                    //    arcController ac = arc.GetComponent<arcController>();
                    //    ac.focus = s.point;
                    //    ac.start = new Vector3(-50, 0, 0);
                    //    ac.end = new Vector3(50, 0, 0);
                    //    ac.key = s.point.x;
                    //    drawnOjects.Add(arc);
                    //}
                    #endregion

                }
                else //not empty handle site event
                {

                    if (s.point.x < lowx) { lowx = s.point.x; };
                    if (s.point.x > highx) { highx = s.point.x; };
                    if (s.point.y < lowy) { lowy = s.point.y; };
                    if (s.point.y > highy) { highy = s.point.y; };

                    if (beachLine.root == null)
                    {
                        //Debug.LogError("root is null");
                    }
                    //find arc directly above event

                    //Node arcAbove = beachLine.searchNear(beachLine.root, s.point.x, true);
                    Node arcAbove = beachLine.root;
                    
                    Site siteAbove = arcAbove.site;
                    int uhoh = 0;
                    while (arcAbove.isEdge)
                    {
                        uhoh++;
                        if(uhoh > 1000)
                        {
                            //Debug.LogError("hit uhoh in arc above search");
                            break;
                        }
                        Vector2 breakpoint,bp1,bp2;
                        //breakpoint = calcBreakPoint(arcAbove.site.point, arcAbove.site2.point, s.point.y - 0.005f).Item2;

                        bp1 = calcBreakPoint(arcAbove.site.point, arcAbove.site2.point, s.point.y - 0.005f).Item1;
                        bp2 = calcBreakPoint(arcAbove.site.point, arcAbove.site2.point, s.point.y - 0.005f).Item2;
                        if (arcAbove.isLeft)
                        {
                            if(bp1.x < bp2.x)
                            {
                                breakpoint = bp1;
                            }
                            else
                            {
                                breakpoint = bp2;
                            }
                        }
                        else
                        {
                            if (bp1.x < bp2.x)
                            {
                                breakpoint = bp2;
                            }
                            else
                            {
                                breakpoint = bp1;
                            }
                        }
                        if (s.point.x < breakpoint.x)
                        {
                            //Debug.Log("left from " + breakpoint);
                            arcAbove = arcAbove.left;
                        }
                        else
                        {
                            //Debug.Log("right from " + breakpoint);
                            arcAbove = arcAbove.right;
                        }
                        
                    }
                    if(arcAbove.site2 != null)
                    {
                        Vector2 bp = calcBreakPoint(arcAbove.site.point, arcAbove.site2.point, s.point.y - 0.005f).Item2;
                        if(s.point.x < bp.x)
                        {
                            if (arcAbove.isLeft)
                            {
                                //Debug.Log("landed on a left breakpoint1");

                                siteAbove = arcAbove.site;
                            }
                            else
                            {
                                //Debug.Log("landed on a right breakpoint1");

                                siteAbove = arcAbove.site;
                            }
                        }
                        else
                        {
                            if (arcAbove.isLeft)
                            {
                                //Debug.Log("landed on a left breakpoint2");

                                siteAbove = arcAbove.site;
                            }
                            else
                            {
                                //Debug.Log("landed on a right breakpoint2");

                                siteAbove = arcAbove.site2;
                            }
                        }
                        
                    }
                    else
                    {
                        siteAbove = arcAbove.site;
                    } //breakpoint

                    //Debug.Log("ARC ABOVE: " + siteAbove.point.ToString());
                    
                    //create new arc
                    Node newSite = new Node(s.point.x);
                    newSite.site = s;

                    //SPLIT ARC INTO TWO
                    //calc break points;
                    Vector2 breakLeft = calcBreakPoint(s.point, siteAbove.point, s.point.y - 0.005f).Item1;
                    Vector2 breakRight = calcBreakPoint(s.point, siteAbove.point, s.point.y - 0.005f).Item2;

                    Node bpLeft = new Node(breakLeft.x);
                    bpLeft.site = siteAbove;
                    bpLeft.site2 = s;
                    bpLeft.isLeft = true;
                    bpLeft.startingDir = breakLeft;
                    bpLeft.isLeft = true;
                    Node bpRight = new Node(breakRight.x);
                    bpRight.site = s;
                    bpRight.site2 = siteAbove;
                    bpRight.startingDir = breakRight;

                    //CALC EDGES
                   
                    Node edgeL = new Node(breakLeft.x + 0.005f);
                    Node edgeR = new Node(breakRight.x - 0.005f);
                    edgeL.site = edgeR.site = s;
                    edgeL.site2 = edgeR.site2 = siteAbove;

                    //direction of edge
                    float midX = (1.0f / 2.0f) * (s.point.x + siteAbove.point.x);
                    float midY = (1.0f / 2.0f) * (s.point.y + siteAbove.point.y);
                    float slope = -(1 / ((newSite.site.point.y - siteAbove.point.y) / (newSite.site.point.x - siteAbove.point.x)));
                    edgeL.direction = (new Vector2(s.point.x - 0.005f, calcYline(slope, s.point.x - 0.005f, midX, midY))
                        - new Vector2(s.point.x - 0.0f, calcYline(slope, s.point.x -0.0f, midX, midY))).normalized;
                    edgeL.isEdge = true;
                    edgeL.startingDir = new Vector2(s.point.x-0.005f, calcYline(slope, s.point.x - 0.005f, midX, midY));
                    edgeR.direction = (new Vector2(s.point.x + 0.005f, calcYline(slope, s.point.x + 0.005f, midX, midY))
                        - new Vector2(s.point.x + 0.0f, calcYline(slope, s.point.x + 0.0f, midX, midY))).normalized;
                    edgeR.isEdge = true;
                    edgeR.startingDir = new Vector2(s.point.x + 0.005f, calcYline(slope, s.point.x + 0.005f, midX, midY));
                    edgeL.midX = edgeR.midX = midX;
                    edgeL.midY = edgeR.midY = midY;
                    edgeL.slope = edgeR.slope = slope;
                    edgeL.isLeft = true;
                    edgeR.isLeft = false;

                    //get pos of breaks based off of the actual directX line
                    breakLeft = calcBreakPoint(s.point, siteAbove.point, directX).Item1;
                    breakRight = calcBreakPoint(s.point, siteAbove.point, directX).Item2;

                    edgeL.lineEnd = breakLeft;
                    edgeR.lineEnd = breakRight;
            
                    GameObject eObj = GameObject.Instantiate(drawable[0], s.point, Quaternion.identity);
                    drawnOjects.Add(eObj);
                    edgeController e = eObj.GetComponent<edgeController>();
                    e.lineKey = edgeL.key;
                    e.start = edgeL.startingDir;
                    e.end = breakLeft;
                    edgeL.edgeObject = eObj;

                    eObj = GameObject.Instantiate(drawable[0], s.point, Quaternion.identity);
                    drawnOjects.Add(eObj);
                    e = eObj.GetComponent<edgeController>();
                    e.start = edgeR.startingDir;
                    e.lineKey = edgeR.key;
                    e.end = breakRight;
                    edgeR.edgeObject = eObj;

                    newSite.leftbp = bpLeft;
                    newSite.rightbp = bpRight;
                    newSite.leftEdge = edgeL;
                    newSite.rightEdge = edgeR;
                 


                    //new insert method VVVV
                    /*
                      since arc above will be a leaf, just get its parent and set the child to
                      the left edge, then set the rest accordingly
                     */
                    if(arcAbove.parent != null)
                    {
                        if (arcAbove.parent.left == arcAbove)
                        {
                            arcAbove.parent.left = edgeL;
                        }
                        else if (arcAbove.parent.right == arcAbove)
                        {
                            arcAbove.parent.right = edgeL;
                        }
                    }
                    else
                    {
                        beachLine.root = edgeL;
                    }
                    
                    edgeL.parent = arcAbove.parent;
                    edgeL.left = bpLeft;
                    bpLeft.parent = edgeL;  
                    edgeL.right = edgeR;
                    edgeR.parent = edgeL;
                    edgeR.left = newSite;
                    newSite.parent = edgeR;
                    edgeR.right = bpRight;
                    bpRight.parent = edgeR;

                    //arcAbove.site.point = new Vector2(69, 420);
                    //todo: uncomment splay because itll balance tree better
                    //dont it break its
                    //beachLine.splay(edgeL);



                    /*
                     * DCEL STUFF
                     * method:
                     *      need to create face for arc above, (if it doesnt already exist)
                     *      insert ONE edge for left and right, and attach a reference to it.
                     *      set incident face of the ONE edge to the new face (or found face) for 
                     *          arc above
                     *      
                     */
                  //  Face faceTry = dcel.faces.Find((x) => x.site == s);
                    Face f;
                    f = new Face();
                    f.site = s;
                    dcel.faces.Add(f);

                    HalfEdge edge = new HalfEdge();
                    edge.incidentFace = f;
                    dcel.edges.Add(edge);
                    edgeL.DCELedge = edgeR.DCELedge = edge;

                    f.outterComponent = edge;
                    



                    List<Node> queue = new List<Node>();
                    eventQueue.inorder(eventQueue.root, queue, false);
                    foreach (Node p in queue)
                    {
                        if (p.site.type == 1) //found a circle event
                        {
                            //need to also check edges to make sure they didnt already finish
                            float distance = Vector2.Distance(p.intersection, p.site.point);
                            float testDistance = Vector2.Distance(p.intersection, s.point) - 0.005f;
                            if (testDistance < distance)
                            {
                                //Debug.LogWarning("invalid event");
                                p.valid = false;
                                foreach (GameObject g in drawnOjects)
                                {

                                    if (g.GetComponent<drawnObjectKey>() != null)
                                    {
                                        if (Mathf.Approximately(g.GetComponent<drawnObjectKey>().key, p.key))
                                        {
                                            GameObject.Destroy(g.GetComponent<drawnObjectKey>().interPoint);
                                            GameObject.Destroy(g);
                                            //Material[] mat = g.GetComponent<MeshRenderer>().materials;
                                            //g.GetComponent<MeshRenderer>().materials = mat;
                                            //mat[0] = g.GetComponent<drawnObjectKey>().mats[0];

                                        }
                                    }


                                }
                            }
                        }
                    }
                    checkCircleEvent(drawnOjects, directX, bpLeft);
                    checkCircleEvent(drawnOjects, directX, bpRight);
                }
            }
            else if(s.type == 1)//is circle event
            {
                if (directX <= n.key)
                {
                    handleCircleEvent(n, drawnOjects, directX);

                    //Debug.Log("ON cIRCLE EVENT");
                }
            }


            eventQueue.delete(n);


        }
        visted = new List<Node>();
        beachLine.inorder(beachLine.root, visted, true);
        TMP_Text text = GameObject.Find("tree").GetComponent<TMP_Text>();
        text.text = "";
        beachLine.reverseInOrder(beachLine.root, 0,text);



        /*
         * bounding box stuff
         *  need highest point, leftmost point, rightmost and lowest,
         *  just create box that is like 10 bigger or something
         */
        Debug.Log("lowx " + lowx + " high x " + highx + " lowy " + lowy + " highy " + highy);
        //box vertex
        int buffer = 15;
        Vertex b1 = new Vertex();
        b1.point = new Vector2(lowx - buffer, highy + buffer);
        Vertex b2 = new Vertex();
        b2.point = new Vector2(highx + buffer, highy + buffer);
        Vertex b3 = new Vertex();
        b3.point = new Vector2(highx + buffer, lowy - buffer);
        Vertex b4 = new Vertex();
        b4.point = new Vector2(lowx - buffer, lowy - buffer);

        b1.type = b2.type = b3.type = b4.type = 1;

        ////box edgex between them
        //HalfEdge e1 = new HalfEdge();
        //e1.origin = b1;
        //e1.destination = b2;
        //b1.incidentEdge = e1;
        //HalfEdge e2 = new HalfEdge();
        //e2.origin = b2;
        //e2.destination = b3;
        //b2.incidentEdge = e2;
        //HalfEdge e3 = new HalfEdge();
        //e3.origin = b3;
        //e3.destination = b4;
        //b3.incidentEdge = e3;
        //HalfEdge e4 = new HalfEdge();
        //e4.origin = b4;
        //e4.destination = b1;
        //b4.incidentEdge = e4;

        Face uf = new Face();
        uf.type = 1;

        //e1.incidentFace = e2.incidentFace = e3.incidentFace = e4.incidentFace = uf;
        //e1.next = e2;
        //e1.prev = e4;
        //e2.next = e3;
        //e2.prev = e1;
        //e3.next = e4;
        //e3.prev = e2;
        //e4.next = e1;
        //e4.prev = e3;

        ////twin edges
        //HalfEdge e1t = new HalfEdge();
        //e1t.twin = e1;
        //e1.twin = e1t;
        //e1t.destination = e1.origin;
        //e1t.origin = e1.destination;
        //HalfEdge e2t = new HalfEdge();
        //e2t.twin = e2;
        //e2.twin = e2t;
        //e2t.destination = e2.origin;
        //e2t.origin = e2.destination;
        //HalfEdge e3t = new HalfEdge();
        //e3t.twin = e3;
        //e3.twin = e3t;
        //e3t.destination = e3.origin;
        //e3t.origin = e3.destination;
        //HalfEdge e4t = new HalfEdge();
        //e4t.twin = e4;
        //e4.twin = e4t;
        //e4t.destination = e4.origin;
        //e4t.origin = e4.destination;

        ////twin incident face will be set when making b5+ i think
        //e1t.next = e4t;
        //e1t.prev = e2t;
        //e2t.next = e2t;
        //e2t.prev = e3t;
        //e3t.next = e2t;
        //e3t.prev = e4t;
        //e4t.next = e3t;
        //e4t.prev = e1t;

        b1.boundIndex = 1;
        b2.boundIndex = 2;
        b3.boundIndex = 3;
        b4.boundIndex = 4;
        b1.bound = true;
        b2.bound = true;
        b3.bound = true;
        b4.bound = true;
        //dcel.vertices.Add(b1);
        //dcel.vertices.Add(b2);
        //dcel.vertices.Add(b3);
        //dcel.vertices.Add(b4);

        //dcel.edges.Add(e1);
        //dcel.edges.Add(e2);
        //dcel.edges.Add(e3);
        //dcel.edges.Add(e4);
        //dcel.edges.Add(e1t);
        //dcel.edges.Add(e2t);
        //dcel.edges.Add(e3t);
        //dcel.edges.Add(e4t);

        dcel.faces.Add(uf);



        LineRenderer boundLR = GameObject.Find("boundingBox").GetComponent<LineRenderer>();
        boundLR.positionCount = 5;
        boundLR.SetPosition(0, b1.point);
        boundLR.SetPosition(1, b2.point);
        boundLR.SetPosition(2, b3.point);
        boundLR.SetPosition(3, b4.point);
        boundLR.SetPosition(4, b1.point);

        /*
         * now need to check the half inifinite edges and set them properly
         *  Method:
         *      loop through all edeges, if no origin or destination calc it comapred to where itll
         *      intersect on the bounding box, 
         *      bounding box defined by low high and buffers
         *      
         *      
         *  dir of half edge always points to the circle event
         *  so basically just use the neg dir to and use that to see where it intersects
         *  -x -y means left or bottom
         *  x -y means right or bottom
         *  -x y means left or up
         *  x y means right or up
         *  only need to check those 2 others dont make much sense
         */

        List<Vertex> top = new List<Vertex>();
        List<Vertex> left = new List<Vertex>();
        List<Vertex> bottom = new List<Vertex>();
        List<Vertex> right = new List<Vertex>();
        left.Add(b1);
        top.Add(b2);
        right.Add(b3);
        bottom.Add(b4);
        int borderCount = 1;

        foreach (HalfEdge e in dcel.edges)
        {
            if(e.origin == null||e.destination==null)
            {
                Vector2 dirToCheck = -e.direction;
                //Debug.Log(" on e" + (dcel.edges.IndexOf(e))+" " + dirToCheck);
                //need to calc some length of line.

                if(e.origin == null && e.destination == null) { continue; }
                Vector2 eStart;
                Vector2 eEnd;
                if(e.origin == null)
                {
                    eStart = e.destination.point;
                    eEnd = e.destination.point + dirToCheck;
                }
                else
                {
                    eStart = e.origin.point;
                    eEnd = e.origin.point + dirToCheck;
                }
                Vector2 inter1,inter2;
                if (dirToCheck.y < 0)
                {
                    if(dirToCheck.x< 0)
                    {
                        //-y,-x
                        //Debug.LogWarning("-x,-y on e" + (dcel.edges.IndexOf(e) + 1) + " " + dirToCheck);
                        inter1 = CheckForIntersection(eStart, eEnd, b4.point, b3.point,null).Value;
                        float distancetoLeft = ((lowx - buffer) - eStart.x) / dirToCheck.x;
                        float distanceToBottom = Vector2.Distance(eStart, inter1);
                        //Debug.LogWarning("distance to left " + distancetoLeft);
                        //Debug.LogWarning("distance to bottom " + distanceToBottom);
                        if (distancetoLeft < distanceToBottom)
                        {
                            //add point to left
                            Vertex v = new Vertex();
                            v.bound = true;
                            v.boundIndex = borderCount++;
                            Vector2 point = new Vector2(b1.point.x, eStart.y+ (dirToCheck.y * distancetoLeft));
                            v.point = point;
                            if (left.Find((x) => Vector2.Equals(x.point,v.point))== null)
                            {
                                left.Add(v);
                                //Debug.LogWarning("l " + point);
                            }
                            if (e.destination == null)
                            {
                                e.destination = v;
                                e.twin.origin = v;
                            } else if (e.origin == null)
                            {
                                e.origin = v;
                                e.twin.destination = v;
                            }
                            v.boundEdge = e;

                        }
                        else
                        {
                            //add point to bottom
                            Vertex v = new Vertex();
                            v.bound = true;
                            v.boundIndex = borderCount++;
                            Vector2 point = new Vector2(eStart.x + (dirToCheck.x * distanceToBottom), b3.point.y);
                            v.point = point;
                            if (bottom.Find((x) => Vector2.Equals(x.point, v.point)) == null)
                            {
                                 bottom.Add(v);
                                //Debug.LogWarning("b " + point);
                            }
                            if (e.destination == null)
                            {
                                e.destination = v;
                                e.twin.origin = v;
                            }
                            else if (e.origin == null)
                            {
                                e.origin = v;
                                e.twin.destination = v;
                            }
                            v.boundEdge = e;
                        }
                    }
                    else
                    {
                        //-y,x
                        //Debug.LogWarning("x,-y on e" + (dcel.edges.IndexOf(e) + 1) + " " + dirToCheck);
                        inter1 = CheckForIntersection(eStart, eEnd, b4.point, b3.point, null).Value;//bottom
                        float distanceToRight = ((highx+ buffer) - eStart.x)/dirToCheck.x;
                        float distanceToBottom = Vector2.Distance(eStart, inter1);
                        //Debug.LogWarning("distance to right " + distanceToRight);
                        //Debug.LogWarning("distance to bottom " + distanceToBottom);
                        if (distanceToRight < distanceToBottom)
                        {
                            Vertex v = new Vertex();
                            v.bound = true;
                            v.boundIndex = borderCount++;
                            Vector2 point = new Vector2(b3.point.x, eStart.y + (dirToCheck.y * distanceToRight));
                            v.point = point;
                            if (right.Find((x) => Vector2.Equals(x.point, v.point)) == null)
                            {
                                right.Add(v);
                                //Debug.LogWarning("r " + point);
                            }
                            if (e.destination == null)
                            {
                                e.destination = v;
                                e.twin.origin = v;
                            }
                            else if (e.origin == null)
                            {
                                e.origin = v;
                                e.twin.destination = v;
                            }
                            v.boundEdge = e;
                        }
                        else
                        {
                            //add point to bottom
                            Vertex v = new Vertex();
                            v.bound = true;
                            v.boundIndex = borderCount++;
                            Vector2 point = new Vector2(eStart.x + (dirToCheck.x * distanceToBottom), b3.point.y);
                            v.point = point;
                            if (bottom.Find((x) => Vector2.Equals(x.point, v.point)) == null)
                            {
                                bottom.Add(v);
                                //Debug.LogWarning("b " + point);
                            }
                            if (e.destination == null)
                            {
                                e.destination = v;
                                e.twin.origin = v;
                            }
                            else if (e.origin == null)
                            {
                                e.origin = v;
                                e.twin.destination = v;
                            }
                            v.boundEdge = e;
                        }
                    }
                }
                else
                {
                    if (dirToCheck.x < 0)
                    {
                        //y,-x
                        //Debug.LogWarning("-x,y on e" + (dcel.edges.IndexOf(e) + 1) + " " + dirToCheck);
                        inter1 = CheckForIntersection(eStart, eEnd, b1.point, b2.point, null).Value;//up
                        float distanceToRight = ((lowx - buffer) - eStart.x) / dirToCheck.x; //left
                        float distanceToBottom = Vector2.Distance(eStart, inter1);
                        //Debug.LogWarning("distance to left " + distanceToRight);
                        //Debug.LogWarning("distance to top " + distanceToBottom);
                        if (distanceToRight < distanceToBottom)
                        {
                            //add point to left
                            Vertex v = new Vertex();
                            v.bound = true;
                            v.boundIndex = borderCount++;
                            Vector2 point = new Vector2(b1.point.x, eStart.y + (dirToCheck.y * distanceToRight));
                            v.point = point;
                            if (left.Find((x) => Vector2.Equals(x.point, v.point)) == null)
                            {
                                left.Add(v);
                                //Debug.LogWarning("l " + point);
                            }
                            if (e.destination == null)
                            {
                                e.destination = v;
                                e.twin.origin = v;
                            }
                            else if (e.origin == null)
                            {
                                e.origin = v;
                                e.twin.destination = v;
                            }
                            v.boundEdge = e;
                        }
                        else
                        {
                            //add point to bottom
                            Vertex v = new Vertex();
                            v.bound = true;
                            v.boundIndex = borderCount++;
                            Vector2 point = new Vector2(eStart.x + (dirToCheck.x * distanceToBottom), b2.point.y);
                            v.point = point;
                            if (top.Find((x) => Vector2.Equals(x.point, v.point)) == null)
                            {
                                top.Add(v);
                                //Debug.LogWarning("t " + point);
                            }
                            if (e.destination == null)
                            {
                                e.destination = v;
                                e.twin.origin = v;
                            }
                            else if (e.origin == null)
                            {
                                e.origin = v;
                                e.twin.destination = v;
                            }
                            v.boundEdge = e;
                        }
                    }
                    else
                    {
                        //y,x
//                        Debug.LogWarning("x,y on e" + (dcel.edges.IndexOf(e) + 1) + " " + dirToCheck);
                        inter1 = CheckForIntersection(eStart, eEnd, b1.point, b2.point, null).Value;//up
                        float distanceToRight = ((highx + buffer) - eStart.x) / dirToCheck.x; //right
                        float distanceToBottom = Vector2.Distance(eStart, inter1);
                        //Debug.LogWarning("distance to right " + distanceToRight);
                        //Debug.LogWarning("distance to top " + distanceToBottom);
                        if (distanceToRight < distanceToBottom)
                        {
                            //add point to right
                            Vertex v = new Vertex();
                            v.bound = true;
                            v.boundIndex = borderCount++;
                            Vector2 point = new Vector2(b3.point.x, eStart.y + (dirToCheck.y * distanceToRight));
                            v.point = point;
                            if (right.Find((x) => Vector2.Equals(x.point, v.point)) == null)
                            {
                                right.Add(v);
                                //Debug.LogWarning("r " + point);
                            }
                            if (e.destination == null)
                            {
                                e.destination = v;
                                e.twin.origin = v;
                            }
                            else if (e.origin == null)
                            {
                                e.origin = v;
                                e.twin.destination = v;
                            }
                            v.boundEdge = e;
                        }
                        else
                        {
                            //add point to bottom
                            Vertex v = new Vertex();
                            v.bound = true;
                            v.boundIndex = borderCount++;
                            Vector2 point = new Vector2(eStart.x + (dirToCheck.x * distanceToBottom), b2.point.y);
                            v.point = point;
                            if (top.Find((x) => Vector2.Equals(x.point, v.point)) == null)
                            {
                                top.Add(v);
                                //Debug.LogWarning("t " + point);
                            }
                            if (e.destination == null)
                            {
                                e.destination = v;
                                e.twin.origin = v;
                            }
                            else if (e.origin == null)
                            {
                                e.origin = v;
                                e.twin.destination = v;
                            }
                            v.boundEdge = e;
                        }
                    }
                }
            }
        }

        left.Sort((site1, site2) => site2.point.y.CompareTo(site1.point.y));//high to low
        right.Sort((site1, site2) => site1.point.y.CompareTo(site2.point.y));//low to high
        top.Sort((site1, site2) => site2.point.x.CompareTo(site1.point.x));//high to low
        bottom.Sort((site1, site2) => site1.point.x.CompareTo(site2.point.x));//low to high

        List<Vertex> finalBoundVertex = new List<Vertex>();
        foreach (Vertex v in bottom) { finalBoundVertex.Add(v); }
        foreach (Vertex v in right) { finalBoundVertex.Add(v); }
        foreach(Vertex v in top) { finalBoundVertex.Add(v); }
        foreach (Vertex v in left) { finalBoundVertex.Add(v); }

        string message = "";
        foreach(Vertex v in finalBoundVertex)
        {
            message += v.point + " ";
        }
        Debug.LogWarning(message);


        borderCount = 1;
        for (int i = 0; i < finalBoundVertex.Count; i++)
        {
            finalBoundVertex[i].boundIndex = borderCount++;
            if (finalBoundVertex[i].boundEdge != null)
            {
                Debug.LogWarning(finalBoundVertex[i].boundIndex + " has an edge");
            }
            
            HalfEdge e = new HalfEdge();
            Vertex v = finalBoundVertex[i];
            v.incidentEdge = e;

            


            e.origin = v;
            if(i != finalBoundVertex.Count - 1)
            {
                e.destination = finalBoundVertex[i + 1];
            }
            else
            {
                e.destination = finalBoundVertex[0];
            }

            

            //on point with edge,
            if(v.boundEdge != null)
            {
                HalfEdge bound;
                Vector2 directionFromPoint;
                bool useTwin = false;
                //FOR THIS IF THEY MATCH WE WANT THAT ONE
                if (v.boundEdge.isLeft)
                {//use normal dir
                    directionFromPoint = (v.point - v.boundEdge.destination.point).normalized;
                    Debug.Log("1 " + directionFromPoint + " " + v.boundEdge.direction + " " + v.boundEdge.twin.direction);
                    if (directionFromPoint == v.boundEdge.twin.direction)
                    {
                        useTwin = true;
                        Debug.Log("using twin");
                    }
                }
                else
                {//negate dir
                    directionFromPoint = (v.point - v.boundEdge.origin.point).normalized;
                    Debug.Log("2 " + directionFromPoint + " " + -v.boundEdge.direction + " " + -v.boundEdge.twin.direction);
                    if (directionFromPoint == -v.boundEdge.twin.direction)
                    {
                        useTwin = true;
                        Debug.Log("using twin");
                    }
                }

                if (useTwin)
                {
                    v.boundEdge.twin.next = e;
                    e.prev = v.boundEdge.twin;
                    e.incidentFace = v.boundEdge.twin.incidentFace;
                }
                else
                {
                    v.boundEdge.next = e;
                    e.prev = v.boundEdge;
                    e.incidentFace = v.boundEdge.incidentFace;
                }

                
            }
            else
            {//no bound edge
                if(i > 0)
                {
                    e.prev = finalBoundVertex[i - 1].incidentEdge;
                    finalBoundVertex[i - 1].incidentEdge.next = e;
                    e.incidentFace = e.prev.incidentFace;
                }
            }

            if (i < finalBoundVertex.Count - 1)
            {
                if (finalBoundVertex[i + 1].boundEdge != null)
                {

                    HalfEdge bound;
                    Vector2 directionFromPoint;
                    bool useTwin = false;
                    //FOR THIS IF THEY MATCH WE WANT THAT ONE
                    if (finalBoundVertex[i + 1].boundEdge.isLeft)
                    {//use normal dir
                        directionFromPoint = (v.point - finalBoundVertex[i + 1].boundEdge.destination.point).normalized;
                        Debug.Log("3 " + directionFromPoint + " " + finalBoundVertex[i + 1].boundEdge.direction + " " + finalBoundVertex[i + 1].boundEdge.twin.direction);
                        if (directionFromPoint == finalBoundVertex[i + 1].boundEdge.twin.direction)
                        {
                            useTwin = true;
                            Debug.Log("using twin");
                        }
                    }
                    else
                    {//negate dir
                        directionFromPoint = (v.point - finalBoundVertex[i + 1].boundEdge.origin.point).normalized;
                        Debug.Log("4 " + directionFromPoint + " " + -finalBoundVertex[i + 1].boundEdge.direction + " " + -finalBoundVertex[i + 1].boundEdge.twin.direction);
                        if (directionFromPoint == -finalBoundVertex[i + 1].boundEdge.twin.direction)
                        {
                            useTwin = true;
                            Debug.Log("using twin");
                        }
                    }
                    if (!finalBoundVertex[i + 1].boundEdge.isLeft)
                    {
                        useTwin = !useTwin;
                    }
                    if (useTwin)
                    {
      
                        finalBoundVertex[i + 1].boundEdge.twin.prev = e;
                        e.next = finalBoundVertex[i + 1].boundEdge.twin;
                        e.incidentFace = finalBoundVertex[i + 1].boundEdge.twin.incidentFace;
                       
                    }
                    else
                    {
                        finalBoundVertex[i + 1].boundEdge.prev = e;
                        e.next = finalBoundVertex[i + 1].boundEdge;
                        e.incidentFace = finalBoundVertex[i + 1].boundEdge.incidentFace;
                    }
                }
            }
            else
            {

                //on last thing
                if (finalBoundVertex[0].boundEdge != null)
                {
                    HalfEdge bound;
                    Vector2 directionFromPoint;
                    bool useTwin = false;
                    //FOR THIS IF THEY MATCH WE WANT THAT ONE
                    if (finalBoundVertex[i + 1].boundEdge.isLeft)
                    {//use normal dir
                        directionFromPoint = (v.point - finalBoundVertex[0].boundEdge.destination.point).normalized;
                        Debug.Log("3 " + directionFromPoint + " " + finalBoundVertex[0].boundEdge.direction + " " + finalBoundVertex[0].boundEdge.twin.direction);
                        if (directionFromPoint == finalBoundVertex[0].boundEdge.twin.direction)
                        {
                            useTwin = true;
                            Debug.Log("using twin");
                        }
                    }
                    else
                    {//negate dir
                        directionFromPoint = (v.point - finalBoundVertex[0].boundEdge.origin.point).normalized;
                        Debug.Log("4 " + directionFromPoint + " " + -finalBoundVertex[0].boundEdge.direction + " " + -finalBoundVertex[0].boundEdge.twin.direction);
                        if (directionFromPoint == -finalBoundVertex[0].boundEdge.twin.direction)
                        {
                            useTwin = true;
                            Debug.Log("using twin");
                        }
                    }

                    if (useTwin)
                    {
                        finalBoundVertex[0].boundEdge.twin.next = e;
                        e.prev = finalBoundVertex[0].boundEdge.twin;
                        e.incidentFace = finalBoundVertex[0].boundEdge.twin.incidentFace;
                    }
                    else
                    {
                        finalBoundVertex[i + 1].boundEdge.next = e;
                        e.prev = finalBoundVertex[i + 1].boundEdge;
                        e.incidentFace = finalBoundVertex[0].boundEdge.incidentFace;
                    }
                }
                else
                {
                    Debug.LogWarning("in here");

                    e.next = finalBoundVertex[0].incidentEdge;
                    finalBoundVertex[0].incidentEdge.prev = e;
                    e.incidentFace = finalBoundVertex[0].incidentEdge.incidentFace;

                    if (finalBoundVertex[i].boundEdge == null) {
                        Debug.LogWarning("in here");

                        e.prev = finalBoundVertex[i - 1].incidentEdge;
                    }
                    else
                    {
                        e.prev = finalBoundVertex[i].boundEdge;
                        finalBoundVertex[i].boundEdge.next = e;
                    }
                }
            }




            HalfEdge twinEdge = new HalfEdge();
            twinEdge.origin = e.destination;
            twinEdge.destination = e.origin;
            twinEdge.twin = e;
            e.twin = twinEdge;
            twinEdge.incidentFace = uf;
            uf.outterComponent = twinEdge;
            if(i > 0 && i < finalBoundVertex.Count - 1)
            {
                twinEdge.prev = finalBoundVertex[i - 1].incidentEdge.twin;
                finalBoundVertex[i - 1].incidentEdge.twin.next = twinEdge;
            }
            if(i == finalBoundVertex.Count - 1)
            {
                twinEdge.prev = finalBoundVertex[i - 1].incidentEdge.twin;
                finalBoundVertex[i - 1].incidentEdge.twin.next = twinEdge;
                twinEdge.next = finalBoundVertex[0].incidentEdge.twin;
                finalBoundVertex[0].incidentEdge.twin.prev = twinEdge;
            }

            dcel.vertices.Add(finalBoundVertex[i]);
            dcel.edges.Add(twinEdge);
            dcel.edges.Add(e);
        }


        return dcel;
    }
    /*
     * 
     * WHEN PRINTING BREAKPOINT MADE ARC MAKE SURE TO CHECK IS LEFT
     * IF IT IS THE RIGHT ARC THEN BASE FOCUS OFF SITE2
     */
    void handleCircleEvent(Node n,List<GameObject> drawnOjects, float directX)
    {

        if (!n.valid)
        {
            //Debug.LogWarning("INVALID CIRCLE EVENT!");
            return;
        }
        else
        {
            //Debug.LogWarning("valid circle");
        }

        /*
         * for bounding box
         * 
         */

        if(n.intersection.x < lowx) { lowx = n.intersection.x; };
        if(n.intersection.x > highx) { highx = n.intersection.x; };
        if (n.intersection.y < lowy) { lowy = n.intersection.y; };
        if (n.intersection.y > highy) { highy = n.intersection.y; };

        //need to remove leftedge,rightedge,and nodetoremove
        //make rightedge and leftedge gameobject have set end before removing from beachline
        //add it at the site of the intersection and have it point down bisector

        //setting the left and right endpoints
        ////Debug.Log("edge " + n.leftEdge.direction + " " + n.leftEdge.startingDir + " " + n.leftEdge.site.point + " " + n.leftEdge.key);
        ////Debug.Log("edge " + n.rightEdge.direction + " " + n.rightEdge.startingDir + " " + n.rightEdge.site.point + " " + n.rightEdge.key);

        foreach (GameObject g in drawnOjects)
        {
            if (g.GetComponent<edgeController>() != null)
            {
                if (g.GetComponent<edgeController>().lineKey == n.leftEdge.key || g.GetComponent<edgeController>().lineKey == n.rightEdge.key)
                {
                    //n.leftEdge.finishedEdge = true;
                    g.GetComponent<edgeController>().end = n.intersection;
                    g.GetComponent<LineRenderer>().startColor = Color.blue;
                    g.GetComponent<LineRenderer>().endColor = Color.blue;
                }
            }else if(g.GetComponent<drawnObjectKey>() != null)
            {
                if (g.GetComponent<drawnObjectKey>().key >= directX)
                {
                    //drawnOjects.Remove(g);
                    GameObject.Destroy(g);
                }
            }
            
            
        }

        //need to figure out when its a left or right edge
        //easy if left focus in lower in y than right focus its right else left
        //doesnt work
        //try using slope
        //if intersection point is lower than edge starting dir y, then the direction needs to be neg
        //otherwise it has to be positive //put in checkin edge lol




        //if righthp, squeezing site3 of n
        //if leftbp, squuzing site2 of n
        //left and right site need to be the other

        //Debug.Log("squeezing " + n.site4.point);
        Site leftSite = n.site2;
        Site rightSite = n.site3;
        //Debug.Log("leftSite" + leftSite.point );
        //Debug.Log("rightsite" + rightSite.point);

        //direction of edge
        
        float midX = (1.0f / 2.0f) * (leftSite.point.x + rightSite.point.x);
        float midY = (1.0f / 2.0f) * (leftSite.point.y + rightSite.point.y);
        float slope = -(1 / ((leftSite.point.y - rightSite.point.y) / (leftSite.point.x - rightSite.point.x)));
        Node edgeR;
        edgeR = new Node(new Vector2(n.intersection.x, calcYline(slope, n.intersection.x, midX, midY)).x);
        if(leftSite.point.y < rightSite.point.y)
        {
            ////Debug.Log("new edge is right");
            edgeR.isLeft = false;
        }
        else
        {
           // //Debug.Log("new edge is left");
            edgeR.isLeft = true;
        }

        if (!n.leftEdge.isLeft && !n.rightEdge.isLeft)
        {
            edgeR.isLeft = false;
        }
        if (n.leftEdge.isLeft && n.rightEdge.isLeft)
        {
            edgeR.isLeft = true;
//            //Debug.LogWarning("its in it");
        }


        edgeR.isEdge = true;

        if (leftSite.point.y != rightSite.point.y)
        {
            if (edgeR.isLeft)
            {
                edgeR.direction = (new Vector2(n.intersection.x - 0.005f, calcYline(slope, n.intersection.x - 0.005f, midX, midY))
                           - new Vector2(n.intersection.x + 0.0f, calcYline(slope, n.intersection.x + 0.0f, midX, midY))).normalized;
                edgeR.site = leftSite;
                edgeR.site2 = rightSite;
            }
            else
            {
                edgeR.direction = (new Vector2(n.intersection.x + 0.005f, calcYline(slope, n.intersection.x + 0.005f, midX, midY))
                           - new Vector2(n.intersection.x + 0.0f, calcYline(slope, n.intersection.x + 0.0f, midX, midY))).normalized;
                edgeR.site = rightSite;
                edgeR.site2 = leftSite;
            }
            //Node parentArc = beachLine.search(beachLine.root,edgeR.site.point.x);
            //parentArc.leftEdge = edgeR;
            edgeR.startingDir = new Vector2(n.intersection.x, calcYline(slope, n.intersection.x, midX, midY));

        }
        else
        {
            edgeR.site = leftSite;
            edgeR.site2 = rightSite;
            edgeR.direction = new Vector2(0, -1);
            edgeR.startingDir = n.intersection;
        }



        //dont want site with middle x value actuall
        //just dont want site that is being remove
        //try just not taking site two
        //def want site3 of n other that just want not site2
        //Site pos1, pos2, pos3,pos4,posBad


        //Site leftSite = nodes[index-2].site;
        //Site rightSite = nodes[index+2].site;

        //get pos of breaks based off of the actual directX line
        //break should actually be the breakpoint between leftedge site and rightedge site

        Vector2 breakPoint = calcBreakPoint(leftSite.point, rightSite.point,n.site.point.y-0.005f).Item2;

        //edgeR.lineEnd = breakPoint;
        edgeR.lineEnd = edgeR.startingDir + edgeR.direction * Vector2.Distance(breakPoint, n.intersection);
        GameObject eObj = GameObject.Instantiate(drawable[0], n.intersection, Quaternion.identity);
        drawnOjects.Add(eObj);
        edgeController e = eObj.GetComponent<edgeController>();
        e.start = n.intersection;
        e.lineKey = edgeR.key;
        e.dir = edgeR.direction;
        breakPoint = calcBreakPoint(leftSite.point, rightSite.point, directX).Item2;
        e.end = edgeR.startingDir + edgeR.direction * Vector2.Distance(breakPoint, n.intersection);
        edgeR.edgeObject = eObj;

        ////Debug.Log(edgeR.isLeft + "new edge " + edgeR.direction + " " + edgeR.startingDir + " " + edgeR.site.point + " " + edgeR.site2.point + " " + edgeR.key + " " + edgeR.lineEnd);

        //beachLine.insert(edgeR,true);
        //beachLine.delete(n.rightEdge);
        //beachLine.delete(n.leftEdge);
        //beachLine.delete(n.nodeToRemove);
        //arcs.delete(n.nodeToRemove);
        /*
         * new deletion shit for splay tree to insert in right spot :)
         * method: take sibling of arc to remove and if the parent of parent of arc is not the other edge,
         * make sibling the child of that other edge
         * if it is the other edge, put the new edge in place then make the sibling its child ezpz
         */
        if(n.nodeToRemove.site2 != null)
        {
            //Debug.LogWarning(n.nodeToRemove.site.point + " " + n.nodeToRemove.site2.point +" left key  "+ n.leftEdge.key + " right key " + n.rightEdge.key);

        }
        Node temp = null;
        Node leftToUse = null;
        Node rightToUse = null;
        if (n.nodeToRemove.parent.left == n.nodeToRemove)
        {
            rightToUse = n.nodeToRemove.parent;
        }
        else if (n.nodeToRemove.parent.right == n.nodeToRemove)
        {
            leftToUse = n.nodeToRemove.parent;
        }
        int uhoh = 0;
        temp = n.nodeToRemove.parent;
        while (true)
        {
            uhoh++;
            if(uhoh > 1000)
            {
                //Debug.LogError("hti hoh no");
                break;
            }
            if(leftToUse == null)
            {
                if(temp.parent.right == temp)
                {
                    leftToUse = temp.parent;
                    break;
                }
                else
                {
                    temp = temp.parent;
                    continue;
                }
            }
            if(rightToUse == null)
            {
                if (temp.parent.left == temp)
                {
                    rightToUse = temp.parent;
                    break;
                }
                else
                {
                    temp = temp.parent;
                    continue;
                }
            }
        }

        if (n.nodeToRemove.parent == rightToUse)
        {
            //Debug.LogWarning("parent is right edge");
            if (rightToUse.parent == leftToUse)
            {
                //Debug.LogWarning("idk if this is actualy a possible state in right");
                //insert new edge first
                if (leftToUse != beachLine.root)
                {
                    if (leftToUse.parent.left == leftToUse)
                    {
                        leftToUse.parent.left = edgeR;
                    }
                    else
                    {
                        leftToUse.parent.right = edgeR;
                    }
                    edgeR.parent = leftToUse.parent;
                }
                else
                {
                    //Debug.LogWarning("edge is now root");

                    beachLine.root = edgeR;
                }
                leftToUse.left.parent = edgeR;
                edgeR.left = leftToUse.left;
                //set sibling in its spots
                edgeR.right = rightToUse.right;
                rightToUse.right.parent = edgeR;


            }
            else
            {
                //Debug.LogWarning("no here");
                //put sibling in its spot
                if (rightToUse.parent.left == rightToUse)
                {
                    rightToUse.parent.left = rightToUse.right;
                }
                else
                {
                    rightToUse.parent.right = rightToUse.right;
                }
                rightToUse.right.parent = rightToUse.parent;
                //checking if left edge is root
                if (leftToUse != beachLine.root)
                {
                    if (leftToUse.parent.left == leftToUse)
                    {
                        leftToUse.parent.left = edgeR;
                    }
                    else
                    {
                        leftToUse.parent.right = edgeR;
                    }
                    edgeR.parent = leftToUse.parent;
                }
                else
                {
                    //Debug.LogWarning("is now root");
                    beachLine.root = edgeR;
                }
                edgeR.right = leftToUse.right;
                leftToUse.right.parent = edgeR;
                edgeR.left = leftToUse.left;
                leftToUse.left.parent = edgeR;
            }
        }
        else if(n.nodeToRemove.parent == leftToUse)
        {
            //Debug.LogWarning("parent is left edge");
            if (leftToUse.parent == rightToUse)
            {
                //Debug.LogWarning("idk if this is actualy a possible state in left");
                //insert new edge first
                if (rightToUse != beachLine.root)
                {
                    if (rightToUse.parent.left == rightToUse)
                    {
                        rightToUse.parent.left = edgeR;
                    }
                    else
                    {
                        rightToUse.parent.right = edgeR;
                    }
                    edgeR.parent = rightToUse.parent;
                }
                else
                {
                    beachLine.root = edgeR;
                }
                rightToUse.right.parent = edgeR;
                edgeR.right = rightToUse.right;
                //set sibling in its spots
                edgeR.left = leftToUse.left;
                leftToUse.left.parent = edgeR;
            }
            else
            {
                //Debug.LogWarning("no here");
                //put sibling in its spot
                if (leftToUse.parent.left == leftToUse)
                {
                    leftToUse.parent.left = leftToUse.left;
                    
                }
                else
                {
                    leftToUse.parent.right = leftToUse.left;
                    
                }
                leftToUse.left.parent = leftToUse.parent;
                if (rightToUse != beachLine.root)
                {
                    if (rightToUse.parent.left == rightToUse)
                    {
                        rightToUse.parent.left = edgeR;
                    }
                    else
                    {
                        rightToUse.parent.right = edgeR;
                    }
                    edgeR.parent = rightToUse.parent;
                }
                else
                {
                    //Debug.LogWarning("is now root");
                    beachLine.root = edgeR;
                }
                edgeR.right = rightToUse.right;
                rightToUse.right.parent = edgeR;
                edgeR.left = rightToUse.left;
                rightToUse.left.parent = edgeR;
            }
        }

        /*
         * some checkin of the edge is wrong,
         * i think i need to specify the edge direction because if they aint got the same slope
         * and not same focus they will always intersect which i feel is the issue rn
         */
        // checkCircleEvent(drawnOjects, directX);

        /*
         * NEED to check the arc to the right and left of the newly inserted line
         * cant do this need to actually traverse the tree
         */

        //todo:uncomment when fixed
        //beachLine.splay(edgeR);

        Node leftArc = null;
        Node rightArc = null;
        if (!edgeR.left.isEdge)
        {
            leftArc = edgeR.left;
        }
        if (!edgeR.right.isEdge)
        {
            rightArc = edgeR.right;
        }

        uhoh = 0;
        Node temp1 = null;
        if(leftArc == null)
        {
             temp1 = edgeR.left;
        }
        Node temp2 = null;
        if(rightArc == null)
        {
             temp2 = edgeR.right;

        }
        while (true)
        {
            uhoh++;
            if (uhoh > 1000)
            {
                break;
            }
            if(leftArc == null)
            {
                if (!temp1.right.isEdge)
                {
                    leftArc = temp1.right;
                }
                else
                {
                    temp1 = temp1.right;
                    continue;
                }
            }
            if(rightArc == null)
            {
                if (!temp2.left.isEdge)
                {
                    rightArc = temp2.left;
                }
                else
                {
                    temp2 = temp2.left;
                    continue;
                }

            }
            if (leftArc != null && rightArc != null)
            {
                break;
            }
        }



        /*
         * DCEL STUFF
         *  todo: need point of site, use intersection point
         *        need to add edge for sites incident edge,
         *        need to get the HalfEdge record for the two incomming edges,
         *              need to set thier origin/destination to the site
         *              set thier next and prev according to if its the left or right edge
         *                  use leftouse and righttouse for that
         * 
         */

        //vertex created
        Vertex v = new Vertex();
        v.point = n.intersection;
        dcel.vertices.Add(v);

        //new edge
        HalfEdge dcelEdge = new HalfEdge();
        dcelEdge.origin = v;
        edgeR.DCELedge = dcelEdge;
        //if (edgeR.isLeft)
        //{
            //Face face = dcel.faces.Find((x) => x.site == n.site2);
            //dcelEdge.incidentFace = face;
        //}
        //else
        //{
            Face face = dcel.faces.Find((x) => x.site == n.site3);
            dcelEdge.incidentFace = face;
        //}
        dcelEdge.incidentFace.outterComponent = dcelEdge;
        dcel.edges.Add(dcelEdge);

        //setting v incident edge
        v.incidentEdge = dcelEdge;

        //UPDATING STUFF IN DCEL
        HalfEdge leftHalf = leftToUse.DCELedge;
        HalfEdge rightHalf = rightToUse.DCELedge;
        Face squeezedFace =dcel.faces.Find((x) => x.site == n.site4);
        if(leftHalf.destination == null)
            leftHalf.destination = v;
        if(rightHalf.origin == null)
            rightHalf.origin = v;
        leftHalf.incidentFace = rightHalf.incidentFace = squeezedFace;
        //leftHalf.incidentFace.outterComponent = leftHalf;
        rightHalf.incidentFace.outterComponent = rightHalf;

        leftHalf.next = rightHalf;
        rightHalf.prev = leftHalf;
        //TWIN STUFF BELOW
        /*
         * meed to check if twin is null otherwise update existing
         */
        HalfEdge leftHalfTwin = new HalfEdge();
        if(leftHalf.twin != null) { leftHalfTwin = leftHalf.twin; }
        else
        {
            dcel.edges.Add(leftHalfTwin);
        }
        HalfEdge rightHalfTwin = new HalfEdge();
        if(rightHalf.twin != null) { rightHalfTwin = rightHalf.twin; }
        else
        {
            dcel.edges.Add(rightHalfTwin);
        }
        HalfEdge newEdgeTwin = new HalfEdge();

        
        leftHalfTwin.twin = leftHalf;
        leftHalf.twin = leftHalfTwin;
        leftHalfTwin.origin = v;
        leftHalfTwin.prev = newEdgeTwin;
        leftHalfTwin.incidentFace = dcel.faces.Find((x) => x.site == n.site2);//left site i hope

        rightHalfTwin.twin = rightHalf;
        rightHalf.twin = rightHalfTwin;
        rightHalfTwin.destination = v;
        rightHalfTwin.next = dcelEdge;
        dcelEdge.prev = rightHalfTwin;
        rightHalfTwin.incidentFace = dcel.faces.Find((x) => x.site == n.site3);//right site i hope

        newEdgeTwin.destination = v;
        newEdgeTwin.twin = dcelEdge;
        dcelEdge.twin = newEdgeTwin;
        newEdgeTwin.next = leftHalfTwin;

        newEdgeTwin.incidentFace = leftHalfTwin.incidentFace;
        newEdgeTwin.incidentFace.outterComponent = newEdgeTwin;

        if (!edgeR.isLeft)
        {
            dcelEdge.isLeft = newEdgeTwin.isLeft = false;
        }
        rightHalf.isLeft = rightHalfTwin.isLeft = false;
        
        leftHalf.direction = leftToUse.direction;
        leftHalfTwin.direction = -leftToUse.direction;
        rightHalf.direction = rightToUse.direction;
        rightHalfTwin.direction = -rightToUse.direction;
        dcelEdge.direction =-edgeR.direction;
        newEdgeTwin.direction = edgeR.direction;
       dcel.edges.Add(newEdgeTwin);



        checkCircleEvent(drawnOjects, directX, leftArc);

        //todo:currently checking the wrong edge for some reason
        checkCircleEvent(drawnOjects, directX, rightArc);


        List<Node> queue = new List<Node>();
        eventQueue.inorder(eventQueue.root, queue, false);
        foreach (Node p in queue)
        {
            if (p.site.type == 1) //found a circle event
            {
                if (p.nodeToRemove == n.nodeToRemove && p.site != n.site)
                {
                    //Debug.LogWarning("invalid event at " + p.intersection);
                    p.valid = false;
                    foreach (GameObject g in drawnOjects)
                    {

                        if (g.GetComponent<drawnObjectKey>() != null)
                        {
                            if (Mathf.Approximately(g.GetComponent<drawnObjectKey>().key, p.key))
                            {
                                GameObject.Destroy(g.GetComponent<drawnObjectKey>().interPoint);
                                GameObject.Destroy(g);
                                //Material[] mat = g.GetComponent<MeshRenderer>().materials;
                                //g.GetComponent<MeshRenderer>().materials = mat;
                                //mat[0] = g.GetComponent<drawnObjectKey>().mats[0];

                            }
                        }


                    }
                }
            }
        }

    }

    void checkCircleEvent(List<GameObject> drawnOjects, float directX, Node arc)
    {
        if(arc.site2 != null)
        {
            //Debug.LogWarning("CHECKING ARC " + arc.site.point + " " + arc.site2.point + " " + arc.key);
            
        }
        else
        {
            //Debug.LogWarning("CHECKING ARC " + arc.site.point + " "+arc.key);

        }
        /*
         * tree traversal method,
         *      take the arc node,
         *          its parent will be one of the edges
         *          the other edge will be somewhere above it i need to figure out relation
         *              probably has to do with if its a left or right child of the parent above it
         *     
         */

        Node leftEdge = null;
        Node rightEdge = null;
        if (arc == arc.parent.left)
        {
            rightEdge = arc.parent;
        }
        else if (arc == arc.parent.right)
        {
            leftEdge = arc.parent;
        }
        int uhoh = 0;

        Node currentNode = arc;
        while (true)
        {
            if(currentNode.parent == null)
            {
                
                return;
            }
            uhoh++;
            if(uhoh > 1000)
            {
                //Debug.LogError("hit uhoh in check circle event");
                break;
            }
            if(leftEdge == null)
            {
                if (currentNode.parent.right == currentNode)
                {
                    leftEdge = currentNode.parent;
                    break;
                   
                }
                else if (currentNode.parent.left == currentNode)
                {
                    currentNode = currentNode.parent;
                    continue;
                }
            }
            if(rightEdge == null)
            {
                if(currentNode.parent.right == currentNode)
                {
                    currentNode = currentNode.parent;
                    continue;
                }else if(currentNode.parent.left == currentNode)
                {
                    rightEdge = currentNode.parent;
                    break;
                }
            }

        }

        Node low, high;
        if (leftEdge.startingDir.x < rightEdge.startingDir.x)
        {
            low = leftEdge;
            high = rightEdge;
        }
        else
        {
            low = rightEdge;
            high = leftEdge;
        }
        ////Debug.Log("left edge " + leftEdge.direction + " " + leftEdge.startingDir + " " + leftEdge.site.point + " key " + leftEdge.key);
        ////Debug.Log("right edge " + rightEdge.direction + " " + rightEdge.startingDir + " " + rightEdge.site.point + " key " + rightEdge.key);
        Vector2? intersection = CheckForIntersection(low.startingDir, low.lineEnd, high.startingDir, high.lineEnd, drawnOjects);
        if (intersection.HasValue)
        {

            
            //low = leftEdge;
            //high = rightEdge;
          //  if (low.direction.y < 0 && high.direction.y > 0) { //Debug.LogWarning("2"); return; }
            if(intersection.Value.x < low.startingDir.x)
            {
                if(low.direction.x > 0) {  return; }
            }
            else
            {
                if(low.direction.x < 0) {  return; }
            }
            if (intersection.Value.x < high.startingDir.x)
            {
                if (high.direction.x > 0) {  return; }
            }
            else
            {
                if (high.direction.x < 0) { return; }
            }
            //Debug.Log("low edge " + low.direction + " " + low.startingDir + " " + low.site.point + low.site2.point + " key " + low.key + " " + low.lineEnd + " " + low.isLeft) ;
            if (arc.site2 != null)
            {
                //Debug.Log(arc.site2.point + " at " + intersection);
            }
            //Debug.Log(arc.site.point + " at " + intersection);
            //Debug.Log("high edge " + high.direction + " " + high.startingDir + " " + high.site.point + high.site2.point + " key " + high.key + " " + high.lineEnd + " " + high.isLeft);
            float directXInter = intersection.Value.y - Vector2.Distance(arc.site.point, intersection.Value)    ;
            // //Debug.Log("DIRECT X INTERSECTION Y VALUIE: " + directXInter);

            //drawing intersection
            GameObject inter = GameObject.Instantiate(drawable[1], new Vector3(intersection.Value.x, intersection.Value.y, 0), Quaternion.identity);
            drawnOjects.Add(inter);

            //ADDING CIRCLE EVENT
            Node circleNode = new Node(directXInter);
            Site circleSite = new Site(new Vector2(intersection.Value.x, directXInter), 1);
            circleNode.leftEdge = low;
            circleNode.intersection = intersection.Value;
            circleNode.rightEdge = high;
            circleNode.nodeToRemove = arc;
            circleNode.site = circleSite;
            circleNode.site2 = arc.site2;
            circleNode.site3 = arc.site;

            Site squeeze;
            if (arc.site2 != null)
            {
                if (!arc.isLeft)
                {
                    ////Debug.Log("squeezing " + arc.site2.point);
                    squeeze = arc.site2;
                }
                else
                {
                    ////Debug.Log("squeezing " + arc.site.point);
                    squeeze = arc.site;
                }
            }
            else
            {
                squeeze = arc.site;
            }
            Site leftSite = leftEdge.site;
            Site rightSite = rightEdge.site;
            if (leftSite == squeeze)
            {
                leftSite = leftEdge.site2;
            }
            if (rightSite == squeeze)
            {
                rightSite = rightEdge.site2;
            }
            Site temp;
            if (leftSite.point.x > rightSite.point.x)
            {
                temp = leftSite;
                leftSite = rightSite;
                rightSite = temp;
            }
            circleNode.site2 = leftSite;
            circleNode.site3 = rightSite;
            circleNode.site4 = squeeze;


            eventQueue.insert(circleNode, true);


            GameObject circleEevent = GameObject.Instantiate(drawable[2], new Vector2(0, directXInter), Quaternion.identity);
            circleEevent.GetComponent<drawnObjectKey>().key = directXInter;
            circleEevent.GetComponent<drawnObjectKey>().interPoint = inter;
            drawnOjects.Add(circleEevent);
        }
    }

    Node getRightEdge(int startIndexR, List<Node> nodes)
    {
        Node rightEdge = null;
        while (startIndexR < nodes.Count)
        {
            if (nodes[startIndexR].isEdge)
            {
                rightEdge = nodes[startIndexR];
                break;
            }
            startIndexR++;
        }
        return rightEdge;
    }
    Node getLeftEdge(int startIndexL, List<Node> nodes)
    {
        Node leftEdge = null;
        while (startIndexL > -1)
        {
            if (nodes[startIndexL].isEdge)
            {
                leftEdge = nodes[startIndexL];
                break;
            }
            startIndexL--;
        }
        return leftEdge;
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
           // //Debug.Log("Lines are parallel, no intersection.");
            return null;
        }
        else
        {
            float x = (b2 - b1) / (m1 - m2);
            float y = m1 * x + b1;
            //Debug.LogWarning("Intersection at (" + x + ", " + y + ")");
            return new Vector2(x, y);
            
        }
    }

    GameObject createArcObject(Vector3 loc, Vector3 start, Vector3 end)
    {
        GameObject arc = GameObject.Instantiate(drawable[3], loc, Quaternion.identity);
        arcController ac = arc.GetComponent<arcController>();
        ac.focus = loc;
        ac.start = start;
        ac.end = end;
        ac.key = loc.x;
        drawable.Add(arc);
        return arc;
    }
}

