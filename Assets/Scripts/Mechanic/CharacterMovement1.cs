using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterMovement1 : MonoBehaviour
{
    private Transform m_transform;
    [SerializeField] private CharacterController cc;
    [SerializeField] private Animator anim;
    public bool canMove = false;
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
    public CameraManager cm;
    [SerializeField] private MultiAimConstraint mac_Hips;
    [SerializeField] private MultiAimConstraint mac_Spine;
    private float desireHips;
    private float desireSpine;
    [SerializeField] private float smoothDuration = 1f;
    private float speedMultipier;
    private float speedAddition;
    private float velocitySpeed;
    private bool isSprint = false;
    private void Awake()
    {
        m_transform = transform;
        canMove = true;
    }

    private void Start()
    {
        ci = CameraIntend.Instance;
        SpeedMultipier(1f);
        //SetDesireWeight(0f);
    }

    private void FixedUpdate()
    {
        GroundCheck();
        //HandleRotation();
        HandleMovement();
        velocitySpeed = cc.velocity.magnitude;
    }

    private void LateUpdate()
    {
        if (velocitySpeed < 0.1f)
        {
            print("Idle");
            anim.SetFloat("Speed", 0);
        }
        else
        {
            anim.SetFloat("Speed", isSprint ? 1f : 0.2f);
        }
        if (cm == null) return;
        cm.FovDirectVariation(velocitySpeed);
    }

    private void HandleMovement()
    {
        if (canMove == false) return;
        if (jumpQueue) return;
        if (inAir)
        {
            cc.Move(Physics.gravity);
            return;
        }
        velocity = TotalSpeed() * direction;
        cc.SimpleMove(Physics.gravity + velocity);
    }

    public void SetMoveSpeedState(bool isSprint = false)
    {
        moveSpeed = isSprint ? sprintSpeed : normalSpeed;
        this.isSprint = isSprint;
    }

    private float TotalSpeed()
    {
        return (moveSpeed + speedAddition) * speedMultipier;
    }

    private void HandleRotation()
    {
        if (isGrounded == false) return;
        if (direction.sqrMagnitude == 0) return;
        m_transform.rotation = Quaternion.Slerp(m_transform.rotation, Quaternion.LookRotation(directionNoY), rotateSpeed * Time.deltaTime);
    }

    public void RotateTowardCamera()
    {
        if (cm == null) return;
        m_transform.rotation = Quaternion.Lerp(m_transform.rotation, cm.GetY(), rotateSpeed * Time.deltaTime);
    }


    public void SpeedMultipier(float multipier)
    {
        if (speedMultipier == multipier) return;
        speedMultipier = multipier;
    }

    public void SpeedAddition(float add)
    {
        speedAddition = add;
    }

    public void SetDesireWeight(float value)
    {
        desireHips = value;
        desireSpine = value;
        if(smc != null)
        {
            StopCoroutine(smc);
        }
        smc = StartCoroutine(SmoothWeight());
    }

    private Coroutine smc;
    public IEnumerator SmoothWeight()
    {
        float timer = 0f;
        float percentage;
        bool isForward = desireSpine != 0;
        while (true)
        {
            if(timer >= 1f)
            {
                mac_Hips.weight = desireHips;
                mac_Spine.weight = desireSpine;
                break;
            }
            timer += Time.deltaTime;
            percentage = timer / smoothDuration;
            if (isForward)
            {
                mac_Hips.weight = percentage;
                mac_Spine.weight = percentage;
            }
            else
            {
                float sum = 1 - percentage;
                mac_Hips.weight = sum;
                mac_Spine.weight = sum;
            }

            yield return null;
        }

    }

    private void HandleJump()
    {
        //cc.Move(new Vector3(0, jumpHeight, 0));
    }

    private float JumpVelocity()
    {
        return Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
    }

    public void SetDirection(Vector2 dir)
    {
        if (ci == null) return;
        if(dir.y <= 0)
        {
            SpeedMultipier(0.75f);
        }
        else
        {
            SpeedMultipier(1f);
        }
        direction = ci.GetIntend(dir);
        directionNoY = direction;
        directionNoY.y = 0;
    }

    public bool EnqueueJump()
    {
        /*
        if (jumpQueue) { return false; }
        jumpQueue = true;
        HandleJump();
        */
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
