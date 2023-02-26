using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Transform m_transform;
    [SerializeField] private Rigidbody rb;

    public float moveSpeed = 1;
    public float rotateSpeed = 1;
    public float normalSpeed = 1;
    public float sprintSpeed = 2;
    public float jumpHeight = 1f;

    private Vector3 direction;
    private Vector3 directionNoY;
    private Vector3 velocity;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float Yoffset = 0.1f;
    [SerializeField] private float checkRadius = 0.1f;
    [SerializeField] private float groundDistance = 1f;
    private bool isGrounded = false;

    private bool jumpQueue = false;
    private bool takeOff, inAir, landed;
    private CameraIntend ci;
    private void Awake()
    {
        m_transform = transform;
    }

    private void Start()
    {
        ci = CameraIntend.Instance;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        HandleRotation();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (isGrounded == false || jumpQueue) return;
        velocity = moveSpeed * direction;
        rb.velocity = Physics.gravity + velocity;
    }

    private void HandleRotation()
    {
        if (isGrounded == false) return;
        if (direction.sqrMagnitude == 0) return;
        m_transform.rotation = Quaternion.Slerp(m_transform.rotation, Quaternion.LookRotation(directionNoY),rotateSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        rb.velocity += new Vector3(0, JumpVelocity(), 0);
    }

    private float JumpVelocity()
    {
        return Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = ci.GetIntend(dir);
        directionNoY = direction;
        directionNoY.y = 0;
    }

    public bool EnqueueJump()
    {
        if (jumpQueue) { return false; }
        jumpQueue = true;
        HandleJump();
        return true;
    }
    #region GroundCheck
    private RaycastHit hit;
    private void GroundCheck()
    {
        Ray ray = new Ray();
        ray.origin = m_transform.position + new Vector3(0, Yoffset, 0);
        ray.direction = Vector3.down;
        isGrounded = Physics.SphereCast(ray, checkRadius,out hit,groundDistance,groundLayer);

        if (jumpQueue)
        {
            if(takeOff == true && inAir == false && landed == true)
            {
                takeOff = false;
                landed = false;
                jumpQueue = false;
            }

            if(isGrounded == true && takeOff == false)
            {

            }
            if(isGrounded == false && takeOff == false)
            {
                takeOff = true;
                inAir = true;
            }
            if(takeOff == true && landed == false)
            {
                if(isGrounded == true)
                {
                    inAir = false;
                    landed = true;
                }
            }
        }
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + new Vector3(0, Yoffset, 0);
        Gizmos.DrawLine(origin, origin - new Vector3(0, groundDistance, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(hit.point, checkRadius);
    }
}
