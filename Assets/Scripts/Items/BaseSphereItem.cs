using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSphereItem : MonoBehaviour
{
    [Header("Connect Settings")]
    [SerializeField] private PathFollower pathFollower;

    [Header("Rotation Settings")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] protected bool isMove;

    private Vector3 rotationDirection = Vector3.up;

    private void FixedUpdate()
    {
        bool isMove = pathFollower == null ? this.isMove : pathFollower.IsMove;

        if (isMove)
        {
            if (bodyTransform != null)
            {
                float rotationSpeed = pathFollower == null ? this.rotationSpeed : pathFollower.FollowSpeed;
                bodyTransform.transform.Rotate(rotationDirection, rotationSpeed);
            }
        }
    }

    public void AddFollower(PathFollower pathFollower)
    {
        this.pathFollower = pathFollower;
    }

    public void RemoveFollower()
    {
        pathFollower = null;
    }


    protected virtual void Destroy()
    {
        //  play sound
    }
}