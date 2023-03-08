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
    [SerializeField] private bool isMoveForward = true;

    [Space(5), Header("Follow")]
    [SerializeField, Range(0, 10)] private float followSpeed = 1f;
    private float tempDefaultSpeed;

    [Space(5), Header("Chase")]
    [SerializeField, Range(0, 10)] private float chaseSpeed = 3f;

    [SerializeField] private float targetDistance;
    [SerializeField] private bool isChase;

    public float CurrentSpeed => currentSpeed;
    public bool IsMove => isMove;

    private void Awake()
    {
        sphereItem = GetComponent<BaseSphereItem>();

        SetSpeed();
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

    public void MoveToNewDistance(float newDistance)
    {
        //StartCoroutine(MoveTo(newDistance));
    }

    [ContextMenu("Move To")]
    private void MoveTo()
    {
        isChase = true;
        SetSpeed(false);

        int followCoef = targetDistance > distanceTravelled ? 1 : -1;
        currentSpeed = currentSpeed * followCoef;
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
        if (!isChase)
        {
            if (tempDefaultSpeed != followSpeed)
            {
                tempDefaultSpeed = followSpeed;
                currentSpeed = followSpeed;
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
            float coef = isMoveForward ? 1 : -1;
            distanceTravelled += followSpeed * coef * Time.deltaTime;
        }
        else
        {
            distanceTravelled += currentSpeed * Time.deltaTime;

            if (Mathf.RoundToInt(distanceTravelled) == Mathf.RoundToInt(targetDistance))
            {
                isChase = false;
                SetSpeed();
            }
        }

        if (currentSpeed > 0)
        {
            if (distanceTravelled >= pathCreator.path.length - 0.1f)
            {
                isMoveForward = false;

                float coef = isMoveForward ? 1 : -1;
                currentSpeed = followSpeed * coef;

                Debug.Log("End");
            }
        }
        else
        {
            if (distanceTravelled < 0.1f)
            {
                isMoveForward = true;

                float coef = isMoveForward ? 1 : -1;
                currentSpeed = followSpeed * coef;

                Debug.Log("Start");
            }
        }

        distanceTravelled = Mathf.Clamp(distanceTravelled, 0.05f, pathCreator.path.length - 0.05f);
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
    }

    private void SetSpeed(bool isFollow = true)
    {
        if (isFollow)
            currentSpeed = followSpeed;
        else
            currentSpeed = chaseSpeed;
    }
}