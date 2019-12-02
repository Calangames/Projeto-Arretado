using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionController3D))]
public class Player : MonoBehaviour
{
    [Range(0f, 40f)]
    public float speed = 6f;
    public AnimationCurve decelerationRate;
    public string[] collidersTagArray = new string[1] { "Solid"};

    public static Player instance = null;
    private Vector2 movement;
    private float oldHInput, oldVInput, hInput, vInput;
    private bool movingH, movingV;
    private CollisionController3D _collisionController;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {               
        _collisionController = GetComponent<CollisionController3D>();        
        _collisionController.CollidersTagArray(collidersTagArray);
    }

    // Update is called once per frame
    void Update()
    {        
        if (Input.GetAxisRaw("Horizontal") == 0f)
        {
            if (movingH)
            {
                oldHInput = hInput;
                StopCoroutine(HRelease());
                StartCoroutine(HRelease());
            }
            movingH = false;
        }
        else
        {
            hInput = Input.GetAxisRaw("Horizontal");            
            movingH = true;            
        }

        if (Input.GetAxisRaw("Vertical") == 0f)
        {
            if (movingV)
            {
                oldVInput = vInput;
                StopCoroutine(VRelease());
                StartCoroutine(VRelease());
            }
            movingV = false;
        }
        else
        {
            vInput = Input.GetAxisRaw("Vertical");
            movingV = true;
        }

        Vector2 normalized = new Vector2(Mathf.Abs(hInput), Mathf.Abs(vInput)).normalized;
        movement = new Vector2(hInput, vInput) * normalized;
        _collisionController.CalculateMovement(movement.x, movement.y, speed);
    } 

    private IEnumerator HRelease()
    {
        float time = 0f;        
        while (time < decelerationRate.keys[decelerationRate.length - 1].time)
        {
            time += Time.deltaTime;
            hInput = oldHInput * decelerationRate.Evaluate(time);            
            yield return null;
        }        
    }

    private IEnumerator VRelease()
    {
        float time = 0f;
        while (time < decelerationRate.keys[decelerationRate.length - 1].time)
        {
            time += Time.deltaTime;
            vInput = oldVInput * decelerationRate.Evaluate(time);
            yield return null;
        }
    }
}
