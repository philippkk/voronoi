using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Node
{
    public Node left;
    public Node right;
    public Node parent;


    public bool isLeft = false;
    public float key;
    public bool isEdge = false;
    public bool finishedEdge = false;
    public float midX, midY,slope;
    public Vector2 direction = Vector2.positiveInfinity;
    public Vector2 startingDir,lineEnd,intersection;
    public Site site; //on edge its the origin edge/below edge
    public Site site2;//for breakpoint on edge its the edge above
    public Site site3;//for circl
    public Site site4;//squeeze site for circle e
    public GameObject edgeObject;

    public Node leftbp, rightbp;

    //for circleEvent
    public Node leftEdge;
    public Node rightEdge;
    public Site leftArc;
    public Site rightArc;
    public Node nodeToRemove;
    public bool valid = true;

    //for dcel
    public HalfEdge DCELedge;



    public Node prev;
    public Node next;

    public Node(Site s)
    {
        site = s;
        left = right = parent = prev = next = null;
    }
    public Node(float k)
    {
        key = k;
        site = site2 =null;
        left = right = parent = prev = next = null;
    }

}

public class SplayTree
{
    float min_diff, min_diff_key;

    public Node root;
    public SplayTree()
    {
        root = null; 
    }
    public Node max(Node x)
    {
        while (x.right != null)
            x = x.right;
        return x;
    }
    public void leftRotate(Node x) 
    {
        Node y = x.right;
        x.right = y.left;
        if (y.left != null){
            y.left.parent = x;
        }
        y.parent = x.parent;
        if (x.parent == null){
            this.root = y;
        }
        else if (x == x.parent.left){
            x.parent.left = y;
        }
        else{
            x.parent.right = y;
        }
        y.left = x;
        x.parent = y;
    }
    public void rightRotate(Node x)
    {
        Node y = x.left;
        x.left = y.right;
        if(y.right != null)
        {
            y.right.parent = x;
        }
        y.parent = x.parent;
        if(x.parent == null)
        {
            this.root = y;
        }else if(x == x.parent.right)
        {
            x.parent.right = y;
        }
        else
        {
            x.parent.left = y;
        }
        y.right = x;
        x.parent = y;
    }
    public void splay(Node n)
    {
        while(n.parent != null)
        {
            if(n.parent == this.root)
            {
                if(n == n.parent.left)
                {
                    this.rightRotate(n.parent);
                }
                else
                {
                    this.leftRotate(n.parent);
                }
            }
            else
            {
                Node p = n.parent;
                Node g = p.parent;
                if(n.parent.left == n && p.parent.left == p)
                {
                    this.rightRotate(g);
                    this.rightRotate(p);
                }else if(n.parent.right == n && p.parent.right == p)
                {
                    this.leftRotate(g);
                    this.leftRotate(p);
                }else if(n.parent.right == n && p.parent.left == p)
                {
                    this.leftRotate(p);
                    this.rightRotate(g);
                }else if (n.parent.left == n && p.parent.right == p)
                {
                    this.rightRotate(p);
                    this.leftRotate(g);
                }
            }
        }
    }
    public void insert(Node n,bool s)
    {
        Node y = null;
        Node temp = this.root;
        while(temp != null)
        {
            y = temp;
            if (n.key < temp.key)
            {
                temp = temp.left;
            }
            else
            {
                temp = temp.right;
            }
        }

        n.parent = y;

        if (y == null)
        {
            this.root = n;
        }else if (n.key < y.key)
        {
            y.left = n;
            n.parent = y;
            
        }
        else
        {
            y.right = n;
            n.parent = y; 
        }
        if(s)
            this.splay(n);
    }
    public Node search(Node n, float x)
    {
        if(n == null)
        {
            return null;
        }
        if (Mathf.Approximately(x,n.key))
        {
            //this.splay(n);
            return n;
        }else if(x < n.key)
        {
            return this.search(n.left, x);
        }else if (x > n.key)
        {
            return this.search(n.right,x);
        }
        else
        {
            return null;
        }
    }
    public void searchNearUtil(Node n,float x,bool arc)
    {
        if (n == null)
        {
            //Debug.Log("done");
            return;
        }
        if (arc) {
            if (n.isEdge)
            {
                if (x > n.lineEnd.x)
                {
                    //Debug.Log("right on edge " + n.key +" "+ n.lineEnd.x);
                    searchNearUtil(n.right, x, arc);
                }
                else
                {
                    //Debug.Log("left on edge " + n.key + " " + n.lineEnd.x);
                    searchNearUtil(n.left, x, arc);
                }
            }
            else
            {
                if (Mathf.Approximately(x, n.key) && !n.isEdge)
                {
                    min_diff_key = n.key;
                    //Debug.Log("done");
                    return;
                }
                if (min_diff > Mathf.Abs(n.key - x) && !n.isEdge)
                {
                    min_diff = Mathf.Abs(n.key - x);
                    min_diff_key = n.key;
                }

                if (x < n.key && !n.isEdge)
                {
                    //Debug.Log("left " + n.key);
                    searchNearUtil(n.left, x, arc);
                }
                else if (x > n.key && !n.isEdge)
                {
                    //Debug.Log("right " + n.key);
                    searchNearUtil(n.right, x, arc);
                }
            }
            
        }
      
    }
    public Node searchNear(Node n,float x,bool arc)
    {
        if (n == null)
        {
            //Debug.Log("NULL ROOT?????");
            return null;
        }

        min_diff = int.MaxValue;
        min_diff_key = -1;

        searchNearUtil(n, x, arc);
        Node result = search(n, min_diff_key);
        return result;
    }
    public void delete(Node n)
    {
        this.splay(n);
        ////Debug.Log("in delete:" + this.root.key);
        SplayTree leftSubtree = new SplayTree();
        leftSubtree.root = this.root.left;
        if (leftSubtree.root != null)
        {
            leftSubtree.root.parent = null;
        }

        SplayTree rightSubtree = new SplayTree();
        rightSubtree.root = this.root.right;
        if (rightSubtree.root != null)
        {
            rightSubtree.root.parent = null;
        }

        if (leftSubtree.root != null)
        {
            Node m = leftSubtree.max(leftSubtree.root);
            leftSubtree.splay(m);
            leftSubtree.root.right = rightSubtree.root;
            if (rightSubtree.root != null)
            {
                rightSubtree.root.parent = leftSubtree.root;  // Update the parent of rightSubtree.root
            }
            this.root = leftSubtree.root;
        }
        else
        {
            this.root = rightSubtree.root;
        }

        //if (!this.root.isEdge)
        //{
        //    if(this.root.right != null)
        //    {
        //        if (this.root.right.isEdge)
        //        {
        //            splay(this.root.right);
        //        }
        //    }
        //    if (this.root.left != null)
        //    {
        //        if (this.root.left.isEdge)
        //        {
        //            splay(this.root.left);
        //        }
        //    }
                
        //}
    }

    public List<Node> inorder(Node n,List<Node> visitedNodes,bool print)
    {
        if (n != null)
        {
            inorder(n.left,visitedNodes,print);
            if (print) {
                if (n.isEdge)
                {
                    if(n.parent == null && n != root)
                    {
                        //Debug.Log("NO PARENT FOR BELOW");
                    }
                    //Debug.Log(n.isLeft + " edge " + n.direction + " " + n.startingDir+ " "+n.site.point + " " + n.site2.point + " "+n.key + " " + n.lineEnd + " " + n.isLeft);

                }
                else if (n.site2 != null)
                {
                    if (n.parent == null && n != root)
                    {
                        //Debug.Log("NO PARENT FOR BELOW");
                    }
                    //Debug.Log("break point: " + n.site.point.ToString() + " AND " + n.site2.point.ToString()
                       // + " " + n.key + " " + n.startingDir + " " + n.isLeft);
                }
                else if (n.site != null)
                {
                    if (n.parent == null && n != root)
                    {
                        //Debug.Log("NO PARENT FOR BELOW");
                    }
                    //Debug.Log("arc: " + n.site.point.ToString() + " "+n.key + " " + n.isLeft);
                }
            }
            visitedNodes.Add(n);
            inorder(n.right,visitedNodes,print);
            return visitedNodes;
        }
        return visitedNodes;
    }
    public void reverseInOrder(Node n, int indent,TMP_Text text)
    {
        if (n != null)
        {
            indent++;
            reverseInOrder(n.right, indent,text);

            for (int i = 0; i < indent; i++)
            {
                text.text+="        ";
            }
           // text.text+="level: " + indent + " " + n.key + "\n";
            if (n.isEdge)
            {
                text.text += "edge " + n.direction + " " + n.startingDir + " " + n.site.point + " " + n.site2.point + " " + n.key + " " + n.isLeft + "\n";
            }
            else if (n.site2 != null)
            {
                text.text += "break point: " + n.site.point.ToString() + " AND " + n.site2.point.ToString()
                    + " " + n.key + " " + n.startingDir + " " + n.isLeft + "\n";
            }
            else if (n.site != null)
            {
                text.text += ("arc: " + n.site.point.ToString() + " " + n.key + " " + n.isLeft + "\n");
            }

            reverseInOrder(n.left, indent,text);
        }
    }
}

/*
 *   
 * 
 *  Vector2? CheckForIntersection(Vector2 line1Start, Vector2 line1End,Vector2 line2Start,Vector2 line2End, List<GameObject> drawnOjects)
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
           // //Debug.Log("Intersection at (" + x + ", " + y + ")");

            return new Vector2(x, y);
            
        }
    }
                                            midx       midy
    public float calcYline(float m, float x, float Xx, float Yy)
    {
        return m * (x - Xx) + Yy;
    }
 * 
 */
