using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class pointController : MonoBehaviour
{
    public Material close;
    public Material far;
    Rigidbody rb;
    public float speed;
    public float maxSpeed;
    public float boundX, boundY;


    public List<GameObject> pointsNear = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = Random.Range(-2, 3);
        float y = Random.Range(-2, 3);
        Vector3 grav = new Vector3(0,0,0);
        foreach (GameObject g in pointsNear)
        {
            //float mag = Vector3.Distance(g.transform.position, transform.position);
            float mag = (g.transform.position - transform.position).magnitude;
            if (Vector3.Distance(transform.position, g.transform.position) < 10)
            {
                //repel
                grav -= (1 / Mathf.Pow(mag, 3) * (g.transform.position - transform.position)) * 4;
                GetComponent<MeshRenderer>().material = close;
            }
            else
            {
                //come together
                grav += (1 / Mathf.Pow(mag, 3) * (g.transform.position - transform.position))*2;
                GetComponent<MeshRenderer>().material = far;


            }

            GetComponent<MeshRenderer>().material = close;


        }

        rb.AddForce(((new Vector3(x, y, 0) + grav )* speed));
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        //1/mag^3 * distance vector




        if (transform.position.x < -boundX)
        {
            rb.AddForce(new Vector3(1, 0, 0) * speed * 5);
            //transform.position = new Vector3(boundX - 5, transform.position.y, transform.position.z);
        }
        if (transform.position.x > boundX)
        {
            rb.AddForce(new Vector3(-1, 0, 0) * speed * 5);
            //transform.position = new Vector3(-boundX + 5, transform.position.y, transform.position.z);
        }
        if (transform.position.y < -boundY)
        {
            rb.AddForce(new Vector3(0, 1, 0) * speed * 5);

            ///transform.position = new Vector3(transform.position.x, boundY - 5, transform.position.z);
        }
        if (transform.position.y > boundY)
        {
            rb.AddForce(new Vector3(0, -1, 0) * speed * 5);

            //transform.position = new Vector3(transform.position.x, -boundY + 5, transform.position.z);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("point"))
        {
            pointsNear.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("point"))
        {
            pointsNear.Remove(other.gameObject);
        }
    }
}

