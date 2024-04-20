using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node left;
    public Node right;
    public Node parent;

    public float key;
    public bool isEdge = false;
    public float midX, midY,slope;
    public Vector2 direction = Vector2.positiveInfinity;
    public Vector2 startingDir,lineEnd;
    public Site site;
    public Site site2;
    public HalfEdge leftEdge;
    public HalfEdge rightEdge;

    public Node prev;
    public Node next;

    public Node(Site s, HalfEdge l, HalfEdge r)
    {
        site = s;
        leftEdge = l;
        rightEdge = r;
        left = right = parent = prev = next = null;
    }
    public Node(float k)
    {
        key = k;
        site = site2 =null;
        leftEdge = rightEdge = null;
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
    public void insert(Node n)
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
        }
        else
        {
            y.right = n;
        }
        this.splay(n);
    }
    public Node search(Node n, float x)
    {
        if (n == null)
        {
            Debug.Log("NULL IN SEARCH??");
            return null;
        }
        if (Mathf.Approximately(x,n.key))
        {
            this.splay(n);
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
            return;
        }
        if (arc) {
            if (Mathf.Approximately(x, n.key) && n.site2 == null && !n.isEdge)
            {
                min_diff_key = 0;
                return;
            }
            if (min_diff > Mathf.Abs(n.key - x) && n.site2 == null && !n.isEdge)
            {
                min_diff = Mathf.Abs(n.key - x);
                min_diff_key = n.key;
            }
            if (x < n.key && n.site2 == null && !n.isEdge)
            {
                searchNearUtil(n.left, x, arc);
            }
            else if (x > n.key && n.site2 == null && !n.isEdge)
            {
                searchNearUtil(n.right, x, arc);
            }
        }
        else
        {
            if (Mathf.Approximately(x, n.key))
            {
                min_diff_key = 0;
                return;
            }
            if (min_diff > Mathf.Abs(n.key - x))
            {
                min_diff = Mathf.Abs(n.key - x);
                min_diff_key = n.key;
            }
            if (x < n.key)
            {
                searchNearUtil(n.left, x, arc);
            }
            else
            {
                searchNearUtil(n.right, x, arc);
            }
        }

        
        
    }
    public Node searchNear(Node n,float x,bool arc)
    {
        if (n == null)
        {
            Debug.Log("NULL ROOT?????");
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
        SplayTree leftSubtree = new SplayTree();
        leftSubtree.root = this.root.left;
        if(leftSubtree.root != null)
        {
            leftSubtree.root.parent = null;
        }

        SplayTree rightSubtree = new SplayTree();
        rightSubtree.root = this.root.right;
        if(rightSubtree.root != null)
        {
            rightSubtree.root.parent = null;
        }

        if(leftSubtree.root != null)
        {
            Node m = leftSubtree.max(leftSubtree.root);
            leftSubtree.splay(m);
            leftSubtree.root.right = rightSubtree.root;
            this.root = leftSubtree.root;
        }
        else
        {
            this.root = rightSubtree.root;
        }
    }
    public List<Node> inorder(Node n,List<Node> visitedNodes,bool print)
    {
        if (n != null)
        {
            inorder(n.left,visitedNodes,print);
            if (print) {
                if (n.isEdge)
                {
                    Debug.Log("edge " + n.direction + " " + n.startingDir+ " "+n.site.point);

                }
                else if (n.site2 != null)
                {
                    Debug.Log("break point: " + n.site.point.ToString() + " AND " + n.site2.point.ToString()
                        + " " + n.key + " " + n.startingDir);
                }
                else if (n.site != null)
                {
                    Debug.Log("arc: " + n.site.point.ToString());
                }
            }

            inorder(n.right,visitedNodes,print);
            visitedNodes.Add(n);
            return visitedNodes;
        }
        return visitedNodes;
    }
}
