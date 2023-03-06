using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSphereItem : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private float rotationSpeed = 5f;

    private Vector3 rotationDirection = Vector3.forward;

    [SerializeField] protected bool IsMove;

    private void FixedUpdate()
    {
        if (IsMove)
        {
            if (bodyTransform != null)
            {
                bodyTransform.transform.Rotate(rotationDirection, rotationSpeed);
            }
        }
    }

    protected virtual void Destroy()
    {

    }
}