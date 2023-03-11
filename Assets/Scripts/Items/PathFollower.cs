using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PathFollower : MonoBehaviour
{
    [Header("Connect Settings")]
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private PathController pathController;
    [SerializeField] private GuideFollower guideFollower;

    private BaseSphereItem sphereItem;

    [Header("Child Settings")]
    [SerializeField] private float currentRotationSpeed = 1f;
    public float CurrentRotationSpeed => currentRotationSpeed;

    [Space(10), Header("Move Settings")]
    [SerializeField] private float distanceTravelled;
    [SerializeField] private bool isLocalMove = true;

    [Space(5)]
    [SerializeField] private float currentSpeed;

    [Space(5), Header("Follow")]
    [SerializeField, Range(0, 10)] private float followSpeed = 1f;

    public bool IsMove => isLocalMove;

    private void Awake()
    {
        sphereItem = GetComponent<BaseSphereItem>();
        guideFollower = GetComponent<GuideFollower>();

        //SetChaseState(false);
        distanceTravelled = 0.1f;
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

    public bool HasPathController()
    {
        return pathController;
    }

    public float GetDistanceTravelled()
    {
        return distanceTravelled;
    }

    public bool HasGuideFollower()
    {
        return guideFollower;
    } 

    public GuideFollower GetGuideFollower()
    {
        return guideFollower;
    }

    public void SetDistanceTravelled(float newTravelledValue)
    {
        distanceTravelled = newTravelledValue;
    }

    [ContextMenu("Make Guides")]
    public void MakeGuides()
    {
        if (!HasGuideFollower())
            guideFollower = gameObject.AddComponent<GuideFollower>();
    }

    [SerializeField] private int index;
    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }

    [ContextMenu("Get Object Index")]
    private void GetObjectIndex()
    {
        if (HasPathController())
        {
            if (pathController.HasElementInList(this))
            {
                int index = pathController.GetIndex(this);
                Debug.Log($"Check Index | Temp: {this.index} / In List: {index}");
            }
            else
                Debug.Log("Element Not Found");
        }
    }

    public void SetMoveDistance(float newDistance)
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

    private bool CheckIsMove()
    {
        if (HasGuideFollower())
            return guideFollower.IsMove;
        else
            return isLocalMove;
    }

    private float CheckSpeed()
    {
        return HasPathController() ? pathController.FollowingSpeed : followSpeed;
    }

    private void Update()
    {
        currentRotationSpeed = CheckIsMove()  ? currentSpeed : 0f;
    }

    public void Move(float speed = -1f)
    {
        if (pathCreator == null)
        {
            isLocalMove = false;
            return;
        }
        else
            isLocalMove = true;

        //  if not cheese, set default speed
        if (speed == -1f)
            speed = CheckSpeed();

        currentSpeed = speed;
        distanceTravelled += currentSpeed * Time.deltaTime;

        //distanceTravelled = Mathf.Clamp(distanceTravelled, 0.1f, Mathf.RoundToInt(pathCreator.path.length) - 0.1f);
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
    }
}