using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class edgeController : MonoBehaviour
{

    public Vector2 start, end;
    public float lineKey;
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

}
