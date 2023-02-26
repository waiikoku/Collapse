using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform cam;
    [SerializeField] private Camera m_cam;
    public Camera myCamera => m_cam;
    [SerializeField] private Transform center;
    public Transform target;
    private Transform m_transform;
    [SerializeField] private float baseSensitivity = 100f;
    [Range(1.0f, 17.0f)]
    public float HorizontalLookSensitivity = 1f;
    [Range(1.0f, 17.0f)]
    public float VerticalLookSensitivity = 1f;
    [Range(1.0f, 17.0f)]
    public float HorizontalAimingSensitivity = 1f;
    [Range(1.0f, 17.0f)]
    public float VerticalAimingSensitivity = 1f;

    public bool lockRotate = false;
    public bool clampYawRot = false;
    public bool clampPitchRot = false;
    private Vector3 combineEulerAngle;
    public Vector2 clampPitch = new Vector2(-45,45);
    public Vector2 clampYaw = new Vector2(-180,180);
    public bool FovRelativeSensitivity = false;
    public bool InvertYAxis = false;

    private const string mouseX = "Mouse X";
    private const string mouseY = "Mouse Y";
    private float mxValue;
    private float myValue;
    private Vector3 mouseAxis;
    private Vector3 mainPivot;
    private Vector3 secPivot;
    private Vector3 desireYaw;
    private Vector3 desirePitch;
    public Vector3 CenterOffset = new Vector3(0, 0, 10);
    public event System.Action<Vector3> CenterOfCamera;
    [SerializeField] private LayerMask groundMask;

    [System.Serializable]
    public struct ZoomData
    {
        public Vector2 Pivot;
        public float CamZ;
        public float Multipier;
    }

    [Header("Zoom Attributes")]
    [SerializeField] private ZoomData NoZoom;
    [SerializeField] private ZoomData ZoomIn;
    [SerializeField] private ZoomData ZoomOut;
    [SerializeField] private float fovTransitionDuration = 0.5f;

    [Header("External")]
    [SerializeField] private StatusReceiver sr;
    public StatusReceiver LocalStatus => sr;
    protected override void Awake()
    {
        base.Awake();
        m_transform = transform;
        Zoom(ZoomState.None);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Instance.OnPlayerDied += DeactivateUI;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlayerDied -= DeactivateUI;
    }
    private bool toggleUI = true;
    private void ActivateUI(bool value)
    {
        sr.gameObject.SetActive(value);
    }

    private void DeactivateUI()
    {
        sr.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            toggleUI = !toggleUI;
            ActivateUI(toggleUI);
        }
        if (target == null) return;
        m_transform.position = target.position;
        if (lockRotate) return;
        mxValue = Input.GetAxis(mouseX) * HorizontalLookSensitivity;
        myValue = Input.GetAxis(mouseY) * VerticalLookSensitivity;
        mouseAxis.x = mxValue;
        mouseAxis.y = InvertYAxis ? myValue : -myValue;
        mainPivot.y = mouseAxis.x;
        secPivot.x = mouseAxis.y;
        desireYaw = baseSensitivity * Time.deltaTime * mainPivot;
        desirePitch = baseSensitivity * Time.deltaTime * secPivot;
        //ClampV1();
        ClampV2();
        m_transform.rotation *= Quaternion.Euler(desireYaw);
        pivot.rotation *= Quaternion.Euler(desirePitch * fovApplyToRotate);
    }

    private void FixedUpdate()
    {
        if (target == null) return;
        OldCenter();
        NewCenter();
        //if (CenterOfCamera == null) return;
        /*
        if(Physics.Raycast(cam.position,cam.forward, out RaycastHit hit,100f, groundMask))
        {
            CenterOfCamera.Invoke(hit.point);
        }
        else
        {
            Vector3 point = cam.position + cam.TransformDirection(new Vector3(0, 0, CenterOffset.z));
            CenterOfCamera.Invoke(point);
        }
        */
    }
    private float fovVelocity;
    private Vector3 pivotVelocity;
    private Vector3 camVelocity;
    private void LateUpdate()
    {
        if (target == null) return;
        m_cam.fieldOfView = Mathf.SmoothDamp(m_cam.fieldOfView, desireFov, ref fovVelocity, fovTransitionDuration);
        pivot.localPosition = Vector3.SmoothDamp(pivot.localPosition, desirePivot, ref pivotVelocity, 0.1f);
        cam.localPosition = Vector3.SmoothDamp(cam.localPosition, desireCam, ref camVelocity, 0.1f);
        //m_cam.fieldOfView = Mathf.Lerp(m_cam.fieldOfView, desireFov,fovTransitionSpeed * Time.deltaTime);
    }

    private void OldCenter()
    {
        if (CenterOfCamera == null) return;
        Vector3 point = cam.position + cam.TransformDirection(new Vector3(0, 0, CenterOffset.z));
        CenterOfCamera.Invoke(point);
    }

    private void NewCenter()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 newPos = center.localPosition;
            newPos.z = hit.distance;
            center.localPosition = newPos;
        }
    }

    public Transform GetCenter()
    {
        return center;
    }

    public Quaternion GetY()
    {
        return m_transform.rotation;
    }
    
    public enum ZoomState
    {
        None,
        ZoomIn,
        ZoomOut
    }
    public void Zoom(ZoomState state)
    {
        switch (state)
        {
            case ZoomState.None:
                SetZoomProperties(NoZoom);
                break;
            case ZoomState.ZoomIn:
                SetZoomProperties(ZoomIn);
                break;
            case ZoomState.ZoomOut:
                SetZoomProperties(ZoomOut);
                break;
            default:
                break;
        }
    }

    public struct Fov
    {
        public float value;

        public Fov(float value)
        {
            this.value = value;
        }
    }
    private float fovApplyToRotate;
    private float desireFov;
    public void FovDirectVariation(float speed)
    {
        float defaultFov = 60f;
        if(speed == 0)
        {
            desireFov = defaultFov;
            //m_cam.fieldOfView = defaultFov;
            return;
        }
        float maxFov = 160f;
        float minFov = 30f;
        float maxSpeed = 26.6f;
        float minSpeed = 0.1f;

        //6 = 160 / ms
        float k = maxFov / maxSpeed;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        //60 = k * 10;
        float y = k * speed;
        y = Mathf.Clamp(y, minFov, maxFov);
        desireFov = y;
        //m_cam.fieldOfView = y;
        InverseVariation(y);
    }

    private void InverseVariation(float fov)
    {
        if(fov <= 60f)
        {
            fovApplyToRotate = 1f;
            return;
        }
        float limitFov = Mathf.Clamp(fov - 60f, 0f, 100f) / 100f;
        fovApplyToRotate = 1 - limitFov;
    }

    public void ZoomFov(Fov fieldOfView)
    {
        m_cam.fieldOfView = fieldOfView.value;
    }

    private Vector2 desirePivot;
    private Vector3 desireCam;
    private void SetZoomProperties(ZoomData data)
    {
        /*
        pivot.localPosition = data.Pivot;
        Vector3 camLocal = cam.localPosition;
        camLocal.z = data.CamZ;
        cam.localPosition = camLocal;
        */
        desirePivot = data.Pivot;
        Vector3 camLocal = cam.localPosition;
        camLocal.z = data.CamZ;
        desireCam = camLocal;
    }

    private void ClampV1()
    {
        /*
        if (clampRotation)
        {
            combineEulerAngle.y = m_transform.rotation.eulerAngles.y;
            combineEulerAngle.x = pivot.localRotation.eulerAngles.x;
            if (combineEulerAngle.y > 180)
            {
                combineEulerAngle.y -= 360;
            }
            if (mainPivot.y > 0)
            {
                if (combineEulerAngle.y >= clampYaw.y)
                {
                    mainPivot.y = 0;
                }
            }
            else if (mainPivot.y < 0)
            {
                if (combineEulerAngle.y <= clampYaw.x)
                {
                    mainPivot.y = 0;
                }
            }
            if (combineEulerAngle.x > 180)
            {
                combineEulerAngle.x -= 360;
            }
            if (secPivot.x > 0)
            {
                if (combineEulerAngle.x >= clampPitch.y)
                {
                    secPivot.x = 0;
                }
            }
            else if (secPivot.x < 0)
            {
                if (combineEulerAngle.x <= clampPitch.x)
                {
                    secPivot.x = 0;
                }
            }
        }
        */
    }

    private void ClampV2()
    {
        if (clampPitchRot)
        {
            combineEulerAngle.x = pivot.localRotation.eulerAngles.x;
            if (combineEulerAngle.x > 180)
            {
                combineEulerAngle.x -= 360;
            }
            float cX = combineEulerAngle.x + desirePitch.x;
            if (cX < clampPitch.x)
            {
                desirePitch.x = clampPitch.x - combineEulerAngle.x;
            }
            else if (cX > clampPitch.y)
            {
                desirePitch.x = clampPitch.y - combineEulerAngle.x;
            }
        }

        if (clampYawRot)
        {
            combineEulerAngle.y = m_transform.rotation.eulerAngles.y;
            if (combineEulerAngle.y > 180)
            {
                combineEulerAngle.y -= 360;
            }
            float cY = combineEulerAngle.y + desireYaw.y;
            if (cY < clampYaw.x)
            {
                desireYaw.y = clampYaw.x - combineEulerAngle.y;
            }
            else if (cY > clampYaw.y)
            {
                desireYaw.y = clampYaw.y - combineEulerAngle.y;
            }
        }
    }
}
