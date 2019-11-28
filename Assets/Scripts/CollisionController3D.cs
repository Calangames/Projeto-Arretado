using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController3D : MonoBehaviour
{
    private BoxCollider col;
    private Vector2 drawStart, drawDirection;
    private Vector3 origin, dir, movement, colOffset, colHalfSize;
    private float hMovement, vMovement;
    private RaycastHit hit;
    private string[] collidersTagArray;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        colOffset = col.center;
        colHalfSize = col.size / 2f;
    }

    public void CollidersTagArray(string[] collidersTagArray)
    {
        this.collidersTagArray = collidersTagArray;
    }

    public void CalculateMovement(float hInput, float vInput, float speed)
    {        
        hMovement = hInput * speed * Time.deltaTime;
        vMovement = vInput * speed * Time.deltaTime;
        dir = new Vector3(hInput, 0f, 0f);
        origin = transform.position + col.center;
        Physics.BoxCast(origin, colHalfSize, dir, out hit, Quaternion.identity, Mathf.Abs(hMovement));
        CheckHorizontalHit();

        dir = new Vector3(0f, 0f, vInput);
        origin += new Vector3(hMovement, 0f, 0f);
        Physics.BoxCast(origin, colHalfSize, dir, out hit, Quaternion.identity, Mathf.Abs(vMovement));
        CheckVerticalHit();

        movement = new Vector3(hMovement, 0f, vMovement);
        transform.position += movement;
    }
    
    private void CheckHorizontalHit()
    {
        if (hit.collider != null && IsSolid(hit))
        {            
            hMovement = Mathf.Sign(hMovement) * Mathf.Max((Mathf.Abs(hit.point.x - origin.x) - colHalfSize.x - 0.011f), 0f);
        }
    }

    private void CheckVerticalHit()
    {
        if (hit.collider != null && IsSolid(hit))
        {
            vMovement = Mathf.Sign(vMovement) * Mathf.Max((Mathf.Abs(hit.point.z - origin.z) - colHalfSize.z - 0.011f), 0f);
        }
    }

    public bool IsSolid(RaycastHit ray)
    {
        foreach (string colliderTag in collidersTagArray)
        {
            if (ray.collider.gameObject.CompareTag(colliderTag))
            {
                return true;
            }
        }
        return false;
    }
}