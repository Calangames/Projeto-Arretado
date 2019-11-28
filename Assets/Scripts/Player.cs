using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionController3D))]
public class Player : MonoBehaviour
{
    [Range(0f, 40f)]
    public float speed = 6f;
    public string[] collidersTagArray = new string[1] { "Solid"};

    public static Player instance = null;

    private float horizontalMovement, verticalMovement;
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
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        _collisionController.CalculateMovement(horizontalMovement, verticalMovement, speed);
    } 
}
