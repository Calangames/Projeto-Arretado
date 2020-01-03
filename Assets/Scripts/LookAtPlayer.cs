using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer: MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 camera = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        transform.LookAt(camera);
    }
}