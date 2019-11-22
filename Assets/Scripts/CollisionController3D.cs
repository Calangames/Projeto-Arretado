using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController3D : MonoBehaviour
{
    private BoxCollider col;
    private Vector2 drawStart, drawDirection;
    private Vector3 origin, dir, movement, colOffset, colHalfSize;
    private float horizontalOffset, verticalOffset, horizontalMovement, verticalMovement;
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
        if (hInput != 0f)
        {
            horizontalOffset = colHalfSize.x * hInput;
        }
        else
        {
            horizontalOffset = 0f;
        }
        if (vInput != 0f)
        {
            verticalOffset = colHalfSize.z * vInput;
        }
        else
        {
            verticalOffset = 0f;
        }
        horizontalMovement = hInput * speed * Time.deltaTime;
        verticalMovement = vInput * speed * Time.deltaTime;
        dir = new Vector3(hInput, 0f, 0f);
        origin = transform.position + col.center;
        Physics.BoxCast(origin, colHalfSize, dir, out hit, Quaternion.identity, Mathf.Abs(horizontalMovement));
        CheckHorizontalHit();

        dir = new Vector3(0f, 0f, vInput);
        origin += new Vector3(horizontalMovement, 0f, 0f);
        Physics.BoxCast(origin, colHalfSize, dir, out hit, Quaternion.identity, Mathf.Abs(verticalMovement));
        CheckVerticalHit();

        movement = new Vector3(horizontalMovement, 0f, verticalMovement);
        transform.position += movement;
    }
    
    private void CheckHorizontalHit()
    {
        if (hit.collider != null && IsSolid(hit))
        {
            horizontalMovement = 0f;
        }
    }

    private void CheckVerticalHit()
    {
        if (hit.collider != null && IsSolid(hit))
        {
            verticalMovement = 0f;
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
