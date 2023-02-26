using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotatePreview : MonoBehaviour , IPointerDownHandler , IPointerUpHandler , IDragHandler
{
    private Vector3 startPoint;
    private Vector3 previousPoint;
    private Vector3 currentPoint;
    private float delta;
    private float distance;
    public float rotateSpeed = 1f;
    [SerializeField] private Transform target;

    private void LateUpdate()
    {
        /*
        if(previousPoint.x > currentPoint.x)
        {
            delta = (previousPoint - currentPoint).magnitude;
        }
        else
        {
            delta = -(previousPoint - currentPoint).magnitude;
        }
        */
        distance = (startPoint - currentPoint).magnitude;
        previousPoint = currentPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentPoint = eventData.position;
        delta = -(eventData.delta.x);
        target.rotation *= Quaternion.Euler(new Vector3(0, delta * rotateSpeed, 0));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPoint = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
