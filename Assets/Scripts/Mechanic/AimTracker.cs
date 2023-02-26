using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTracker : MonoBehaviour
{
    [SerializeField] private Transform aimAssistant;
    private Vector3 desirePosition;
    [SerializeField] private float speed;
    private void Start()
    {
        CameraManager.Instance.CenterOfCamera += UpdateTracker;
    }

    private void FixedUpdate()
    {
        //aimAssistant.position = Vector3.Lerp(aimAssistant.position, desirePosition, speed * Time.deltaTime);
        //aimAssistant.position = desirePosition;
    }

    private void UpdateTracker(Vector3 position)
    {
        if (aimAssistant == null) return;
        aimAssistant.position = position; 
        //desirePosition = position;
    }
}
