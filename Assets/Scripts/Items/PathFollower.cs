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

    private void OnEnable()
    {
        if (sphereItem != null)
            sphereItem.AddFollower(this);
    }

    private void OnDisable()
    {
        if (sphereItem != null)
            sphereItem.RemoveFollower();
    }

    public PathController GetPathController()
    {
        return pathController;
    }

    public float GetDistanceTravelled()
    {
        return distanceTravelled;
    }

    public void MoveToNewDistance(float newDistance)
    {
        distanceTravelled = newDistance;
    }

    public void ChangePathController(PathController pathController)
    {
        this.pathController = pathController;
        ChangePathCreator(pathController != null ? pathController.GetPathCreator() : null);
    }

    public void ChangeMoveSpeed(float moveSpeed)
    {
        followSpeed = moveSpeed;
    }

    private void ChangePathCreator(PathCreator pathCreator)
    {
        this.pathCreator = pathCreator;
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