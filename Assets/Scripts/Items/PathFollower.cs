using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PathFollower : MonoBehaviour
{
    [Header("Connect Settings")]
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private PathController pathController;

    [SerializeField] private BaseSphereItem sphereItem;

    [Header("Move Settings")]
    [SerializeField] private float distanceTravelled;

    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private bool isMove;

    public float FollowSpeed => followSpeed;
    public bool IsMove => isMove;

    private void Awake()
    {
        sphereItem = GetComponent<BaseSphereItem>();
    }

    public void ChangePathController (PathController pathController)
    {
        this.pathController = pathController;
    }
        
    public void ChangePathCreator(PathCreator pathCreator)
    {
        this.pathCreator = pathCreator;
    }

    public void ChangeMoveSpeed(float moveSpeed)
    {
        followSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        if (sphereItem != null)
        {
            sphereItem.AddFollower(this);
        }
    }

    private void OnDisable()
    {
        if (sphereItem != null)
        {
            sphereItem.RemoveFollower();
        }
    }

    private void FixedUpdate()
    {
        if (pathCreator == null)
        {
            isMove = false;
            return;
        }
        else
            isMove = true;

        distanceTravelled += followSpeed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
    }

}