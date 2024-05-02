using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region old
//public class Vertex
//{
//    private string name;
//    private double posX, posY;
//    private HalfEdge incidentEdge;
//    private Face face;
//    public Vertex(double x, double y,HalfEdge edge,string name)
//    {
//        posX = x;
//        posY = y;
//        incidentEdge = edge;
//        this.name = name;
//        if(incidentEdge == null)
//        {
//            incidentEdge = new HalfEdge("nil");
//        }
//    }
//    public string getName() { return name; }
//    public double getX() { return posX; }
//    public double getY() { return posY; }
//    public HalfEdge getIncidentEdge() { return incidentEdge; }
//    public void setIncidentEdge(HalfEdge edge){ incidentEdge = edge;}

//    override
//    public string ToString()
//    {
//        string str = name + " (" + posX + ", " + posY + ") "+incidentEdge.getName();
//        return str;
//    }
//}
//public class Face
//{
//    private HalfEdge outerComponent, innerComponent;
//    private string name;
//    public Face(HalfEdge outer,HalfEdge inner,string name)
//    {

//        if(outer != null)
//        {
//            outerComponent = outer;
//            //loop around
//            HalfEdge start = outer;
//            outer.setIncidentFace(this);
//            HalfEdge edge = outer.getNext();
//            while(edge != start)
//            {
//                edge.setIncidentFace(this);
//                edge = edge.getNext();
//            }
//        }
//        else
//        {
//            outerComponent = new HalfEdge("nil");
//        }
//        if (inner != null)
//        {
//            innerComponent = inner;
//            HalfEdge start = inner;
//            inner.setIncidentFace(this);
//            HalfEdge edge = inner.getNext();
//            while (edge != start)
//            {
//                edge.setIncidentFace(this);
//                edge = edge.getNext();
//            }
//        }
//        else
//        {
//            innerComponent = new HalfEdge("nil");
//        }
//        this.name = name;
//    }

//    public HalfEdge getOuter() { return outerComponent; }
//    public HalfEdge getInner() { return innerComponent; }
//    public string getName() { return name; }
//    //f for face c for cell
//    override
//    public string ToString()
//    {
//        string str = name+" "+outerComponent.getName()+" "+innerComponent.getName();
//        return str;
//    }
//}   
//public class HalfEdge
//{
//    private string name;
//    private Vertex origin;
//    private HalfEdge twin,next,prev;
//    private Face incidentFace;

//    public HalfEdge(Vertex o, HalfEdge n, HalfEdge p, Face f,string name)
//    {
//        origin = o;
//        next = n;
//        prev = p;
//        incidentFace = f;
//        this.name = name;
//    }
//    public HalfEdge(string name)
//    {
//        this.name = name;
//    }
//    public Vertex getOrigin() { return origin; }
//    public HalfEdge getTwin() { return twin; }
//    public HalfEdge getNext() { return next; }
//    public HalfEdge getPrev() { return prev; }
//    public Face getIncidentFace() { return incidentFace; }
//    public void setIncidentFace(Face f) { incidentFace = f; }
//    public string getName() { return name; }
//    public void setNext(HalfEdge e,HalfEdge p)
//    {
//            next = e;
//            prev = p;
//            name += e.getOrigin().getName().Substring(1);
//            if (twin == null)
//            {
//                twin = new HalfEdge(next.getOrigin(), null, null, null, name.Substring(0,1)+next.getOrigin().getName().Substring(1)+",");
//                twin.twin = this;

//            }
//    }
//    public void setTwin()
//    {
//        twin.setNext(prev.getTwin(),next.getTwin());
//    }
//    override
//    public string ToString()
//    {
//        string str = name + " " + origin.getName() + " " + twin.getName()
//           + " " + incidentFace.getName()
//           +" "+next.getName()+" "+prev.getName();
//        return str;
//    }

//}
//public class DCEL
//{
//    public List<Vertex> vertices;
//    public List<Face> faces;
//    public List<HalfEdge> edges;
//    public List<Vertex> boundVertices;

//    public DCEL()
//    {
//        vertices = new List<Vertex>();
//        boundVertices = new List<Vertex>();
//        faces = new List<Face>();
//        edges = new List<HalfEdge>();
//    }

//    public Vertex createVertex(double x, double y,HalfEdge edge,int type)
//    {
//        string name = "";
//        switch (type)
//        {
//            case 1: //site
//                name = "p";
//                break;
//            case 2: //voronoi
//                name = "v";
//                break;
//            case 3: //delaunay
//                name = "p";
//                break;
//            case 4:
//                name = "b";
//                break;
//        }
//        Vertex v;
//        if (name.Equals("b"))
//        {
//            v = new Vertex(x, y, edge, name + (boundVertices.Count + 1).ToString());
//            boundVertices.Add(v);
//        }
//        else
//        {
//            v = new Vertex(x, y, edge, name + (vertices.Count + 1).ToString());
//            vertices.Add(v);
//        }
//        return v;
//    }
//    public void removeVertex(Vertex v)
//    {
//        vertices.Remove(v);
//        return;
//    }
//    public HalfEdge createEdge(Vertex o, HalfEdge n, HalfEdge p, Face f, int type)
//    {

//        string name = "";
//        switch (type)
//        {
//            case 1: //site
//                name = "e";
//                break;
//            case 2: //voronoi
//                name = "d";
//                break;
//        }
//        HalfEdge e = new HalfEdge(o, n, p, f,name+o.getName().Substring(1)+",");
//        o.setIncidentEdge(e);
//        edges.Add(e);
//        return e;
//    }
//    public void removeEdge(HalfEdge e)
//    {
//        edges.Remove(e);
//        return;
//    }
//    public Face createFace(HalfEdge outer, HalfEdge inner,int type)
//    {
//        string name = "";
//        switch (type)
//        {
//            case 1: //site
//                name = "f";
//                break;
//            case 2: //voronoi
//                name = "c";
//                break;
//            case 3:
//                name = "v";
//                break;
//        }
//        Face f = new Face(outer, inner,name+(faces.Count+1).ToString());
//        faces.Add(f);
//        return f;
//    }
//    public void removeFace(Face f)
//    {
//        faces.Remove(f);
//        return;
//    }

//    public void printDCEL()
//    {
//        //todo
//        return;
//    }
//}
#endregion
public class Site //INPUT POINTS
{
    public int type; //event/circle
    public int index;
    public Vector2 point;
    public Face face;

    public Site(Vector2 v,int type)
    {
        this.type = type;
        point = v;
    }
}
public class Vertex //POINTS OF VORONOI
{
    public int boundIndex;
    public bool bound = false;
    public Vector2 point;
    public HalfEdge incidentEdge;
    public int type = 0;


    public HalfEdge boundEdge; //the edge that created the point
}
public class HalfEdge
{
    public bool bound = false;
    public Vertex origin;
    public Vertex destination;
    public HalfEdge twin;
    public Face incidentFace;
    public HalfEdge next;
    public HalfEdge prev;
    public Vector2 direction;



    public float slope; //for edge case intersection stuff : )
    public float midX, midY;
    public bool isLeft = true;
    public bool fromCirclEvent = false;
    public bool isTwin = false;
    public bool dontUse = false;
}
public class Face
{
    public int type = 0;
    public HalfEdge outterComponent;
    public HalfEdge innerComponent;
    public Vertex incidentVertex;
    public Site site;
}
public class DCEL
{
    public List<Site> sites;
    public List<Vertex> vertices;
    public List<HalfEdge> edges;
    public List<Face> faces;
    public DCEL()
    {
        sites = new List<Site>();
        vertices = new List<Vertex>();
        edges = new List<HalfEdge>();
        faces = new List<Face>();
    }
}