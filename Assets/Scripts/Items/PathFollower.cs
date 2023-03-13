using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PathFollower : MonoBehaviour
{
    [Header("Connect Settings")]
    [SerializeField] private PathController pathController;
    private GuideFollower currentGuide;

    [SerializeField] private GuideFollower parentGuideFollower;

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
        currentGuide = GetComponent<GuideFollower>();
        parentGuideFollower = currentGuide;

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
        return currentGuide;
    }

    public bool HasParentGuideFollower()
    {
        return parentGuideFollower;
    }

    public GuideFollower GetGuideFollower()
    {
        return currentGuide;
    }

    private void SetDistanceTravelled()
    {
        if (HasPathController() && pathController.HasPathCreator())
        {
            PathCreator pathCreator = pathController.GetPathCreator();
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
        }
    }

    #region Guides
    [ContextMenu("Make Guides")]
    public void AddGuides()
    {
        if (!HasGuideFollower())
        {
            currentGuide = gameObject.AddComponent<GuideFollower>().GetComponent<GuideFollower>();

            Debug.Log(currentGuide.name);
            parentGuideFollower = currentGuide;
        }
    }

    public void RemoveGuides()
    {
        if (HasGuideFollower())
            Destroy(currentGuide);
    }
    #endregion

    public void SetParentGuideFollower(GuideFollower parentGuideFollower)
    {
        this.parentGuideFollower = parentGuideFollower;
    }

    [SerializeField] private int index;
    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }

    public void SetMoveDistance(float newDistance)
    {
        distanceTravelled = newDistance;
        SetDistanceTravelled();
    }

    public void ChangePathController(PathController pathController)
    {
        this.pathController = pathController;
    }

    public void ChangeMoveSpeed(float moveSpeed)
    {
        followSpeed = moveSpeed;
    }

    private bool CheckIsMove()
    {
        if (HasParentGuideFollower())
        {
            return parentGuideFollower.IsMove;
        }
        else
            return isLocalMove;
    }

    private float CheckSpeed()
    {
        return HasPathController() ? pathController.FollowingSpeed : followSpeed;
    }

    private void Update()
    {
        currentRotationSpeed = CheckIsMove() ? currentSpeed : 0f;
    }

    public void Move(float speed = 0)
    {
        if (!HasPathController() || !GetPathController().HasPathCreator())
        {
            isLocalMove = false;
            return;
        }

        PathCreator pathCreator = pathController.GetPathCreator();
        isLocalMove = true;

        //  if not cheese, set default speed
        if (speed == 0)
            speed = CheckSpeed();

        currentSpeed = speed;
        distanceTravelled += currentSpeed * Time.deltaTime;

        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
    }

    [SerializeField] private Material tempMaterial;
    [ContextMenu("Destroy Item")]
    public void DestroyItem()
    {
        List<PathFollower> pathFollowers = new List<PathFollower>();

        if (HasPathController())
        {
            Debug.Log("Normal");
            //pathController.RemoveSphereItem(this);
            int index = pathController.GetFollowerIndex(this);
            int count = pathController.GetFollowersCount();
            int startIndex = 0;

            int offcet = 2;
            if (count > 1)
                offcet = 1;
            else
                offcet = pathController.GetFollowersCount();

            if (index == 0)
            {
                startIndex = index + offcet;
                for (int i = index; i <= index + offcet; i++)
                    pathFollowers.Add(pathController.GetFollowerByIndex(i));
            }
            else if (index == count - 1)
            {
                startIndex = index;
                for (int i = index - offcet; i <= index; i++)
                    pathFollowers.Add(pathController.GetFollowerByIndex(i));
            }
            else
            {
                startIndex = index + 1;
                for (int i = index - 1; i <= index + 1; i++)
                    pathFollowers.Add(pathController.GetFollowerByIndex(i));
            }

            for (int i = 0; i < pathFollowers.Count; i++)
            {
                Material tempMaterial = new Material(this.tempMaterial);
                tempMaterial.color = Color.green;

                Renderer renderer = pathFollowers[i].GetComponentInChildren<Renderer>();
                renderer.material = tempMaterial;
            }

            pathController.RemoveSphereItems(pathFollowers);
        }

        for (int i = 0; i < pathFollowers.Count; i++)
            Destroy(pathFollowers[i].gameObject);
    }
}