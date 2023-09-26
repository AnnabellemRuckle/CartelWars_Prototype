using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 75f;
    public float jumpVelocity = 5f;
    public float duckedHeight = 0.5f; 
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;

    private float vInput;
    private float hInput;
    private float originalHeight; 

    private Rigidbody _rb;
    private CapsuleCollider _col;

    public delegate void JumpingEvent();
    public event JumpingEvent playerJump;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        originalHeight = _col.height;

       
        playerJump += PerformJump;
    }

    void Update()
    {
        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * rotateSpeed;

        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            playerJump();
        }

      
        if (Input.GetKey(KeyCode.S))
        {
            Duck();
        }
        else
        {
            
            ReleaseDuck();
        }
    }

    private void FixedUpdate()
    {
        Vector3 rotation = Vector3.up * hInput;
        Quaternion angleRot = Quaternion.Euler(rotation * Time.fixedDeltaTime);

        _rb.MovePosition(this.transform.position + this.transform.forward * vInput * Time.fixedDeltaTime);
        _rb.MoveRotation(_rb.rotation * angleRot);
    }

    private bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(_col.bounds.center.x, _col.bounds.min.y, _col.bounds.center.z);
        bool grounded = Physics.CheckCapsule(_col.bounds.center, capsuleBottom, distanceToGround, groundLayer, QueryTriggerInteraction.Ignore);

        return grounded;
    }

    private void PerformJump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, jumpVelocity, _rb.velocity.z);
    }

    private void Duck()
    {
        _col.height = duckedHeight;
    }

    private void ReleaseDuck()
    {
        _col.height = originalHeight;
    }
}
