using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer: MonoBehaviour
{
    // public float speed = 2f;

    // private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        //     rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camera = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        transform.LookAt(camera);
      //  rb.velocity = transform.forward * speed;
    }
}