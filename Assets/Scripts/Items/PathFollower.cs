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

    /*[SerializeField] */
    private BaseSphereItem sphereItem;

    [Space(10), Header("Move Settings")]
    [SerializeField] private float distanceTravelled;
    [SerializeField] private bool isMove = true;

    [Space(5)]
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool isMoveDirectionForward = true;

    [Space(5), Header("Follow")]
    [SerializeField, Range(0, 10)] private float followSpeed = 1f;
    private float tempDefaultSpeed;

    [Space(5), Header("Chase")]
    [SerializeField, Range(0, 10)] private float chaseSpeed = 3f;

    [SerializeField] private float targetDistance;
    [SerializeField] private bool isChase;
    [SerializeField] private bool isChaseForward;

    public float CurrentSpeed => currentSpeed;
    public bool IsMove => isMove;

    private void Awake()
    {
        sphereItem = GetComponent<BaseSphereItem>();

        SetChaseState(false);
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

    public float GetDistanceTravelled()
    {
        return distanceTravelled;
    }

    public void SetDistanceTravelled(float newTravelledValue)
    {
        distanceTravelled = newTravelledValue;
    }

    public void MoveToNewDistance(PathFollower targetPathFollower, float offcet)
    {
        
    }

    [ContextMenu("Move To")]
    private void MoveTo()
    {
        isChase = true;
        isChaseForward = targetDistance > distanceTravelled;

        SetChaseState(true);
    }

    [SerializeField] private int index;
    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }

    public void ChangeDirection(bool newDirection)
    {
        isMoveDirectionForward = newDirection;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            MoveTo();

        if (!isChase)
        {
            if (tempDefaultSpeed != followSpeed)
            {
                tempDefaultSpeed = followSpeed;

                float coef = isMoveDirectionForward ? 1 : -1;
                currentSpeed = followSpeed * coef;
            }
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

        if (!isChase)
        {
            float coef = isMoveDirectionForward ? 1 : -1;
            distanceTravelled += followSpeed * coef * Time.deltaTime;
        }
        else
        {
            float coef = isChaseForward ? 1 : -1;
            distanceTravelled += chaseSpeed * coef * Time.deltaTime;

            if (Mathf.RoundToInt(distanceTravelled) == Mathf.RoundToInt(targetDistance))
            {
                SetChaseState(false);
            }
        }

        if (isMoveDirectionForward)
        {
            if (distanceTravelled >= Mathf.RoundToInt(pathCreator.path.length) - 1)
            {
                isMoveDirectionForward = false;

                float coef = isMoveDirectionForward ? 1 : -1;
                currentSpeed = followSpeed * coef;

                Debug.Log("End");
            }
        }
        else
        {
            if (distanceTravelled < 1f)
            {
                isMoveDirectionForward = true;

                float coef = isMoveDirectionForward ? 1 : -1;
                currentSpeed = followSpeed * coef;
                Debug.Log($"Coef: {coef} | Speed: {currentSpeed}");

                Debug.Log("Start");
            }
        }

        distanceTravelled = Mathf.Clamp(distanceTravelled, 0, Mathf.RoundToInt(pathCreator.path.length) - 1);
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
    }

    private void SetChaseState(bool chaseState)
    {
        if (!chaseState)
            currentSpeed = followSpeed;
        else
            currentSpeed = chaseSpeed;

        isChase = chaseState;
    }
}