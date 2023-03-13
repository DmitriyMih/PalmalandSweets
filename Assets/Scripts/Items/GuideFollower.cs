using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behavior
{
    FollowThePath,
    FollowBack,
    Stop
}

public class GuideFollower : MonoBehaviour
{
    [Header("Connect Settings")]
    [SerializeField] private PathFollower pathFollower;

    [Header("Behavior Settings"), Space(10)]
    [SerializeField] private Behavior currentBehavior;

    [Header("Move Settings"), Space(10)]
    [SerializeField, Range(0, 10)] private float chaseSpeed = 0.5f;

    [SerializeField] private bool isMoveDirectionForward = true;
    [SerializeField] private bool isMove = true;

    public bool IsMove => isMove;

    [Header("Child Settings"), Space(10)]
    [SerializeField] private List<PathFollower> childObjects = new List<PathFollower>();

    [Header("Debug View")]
    private Renderer renderer;
    private Material tempMaterial;

    private void Awake()
    {
        pathFollower = GetComponent<PathFollower>();
        renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            tempMaterial = renderer.material;

            Material guideMaterial = new Material(tempMaterial);
            guideMaterial.color = Color.yellow;
            renderer.material = guideMaterial;
        }
    }

    public void UpdateChilds(List<PathFollower> newChilds)
    {
        childObjects.Clear();

        childObjects = new List<PathFollower>(newChilds);
        SetGuideFollower(this);
    }

    private void SetGuideFollower(GuideFollower follower)
    {
        for (int i = 0; i < childObjects.Count; i++)
        {
            if (childObjects[i] == null)
                continue;

            childObjects[i].SetParentGuideFollower(follower);
        }
    }

    public bool HasPathFollower()
    {
        return pathFollower;
    }

    public PathFollower GetPathFollower()
    {
        return pathFollower;
    }

    private void FixedUpdate()
    {
        float defaultSpeed = 0;
        CheckBehavior(ref defaultSpeed);

        if (isMove)
            Move(defaultSpeed);
    }

    private void Move(float speed)
    {
        if (pathFollower != null)
            pathFollower.Move(speed);

        for (int i = 0; i < childObjects.Count; i++)
        {
            if (childObjects[i] == null)
            {
                //Debug.Log($"Child In Index: {i} | Not Found");
                continue;
            }

            childObjects[i].Move(speed);
        }
    }

    #region Behaviour
    public void SetBehavior(Behavior behavior)
    {
        currentBehavior = behavior;
    }

    private void CheckBehavior(ref float speed)
    {
        switch (currentBehavior)
        {
            case Behavior.FollowThePath:
                isMove = true;
                isMoveDirectionForward = true;
                break;

            case Behavior.FollowBack:
                isMoveDirectionForward = false;
                speed = -chaseSpeed;
                isMove = CheckTheBackwardMovement();
                break;

            case Behavior.Stop:
                isMove = CheckStopConnection();
                break;
        }
    }

    private bool CheckTheBackwardMovement()
    {
        if (!HasPathFollower() || !pathFollower.HasPathController())
            return false;

        PathController pathController = pathFollower.GetPathController();
        if (!pathController.HasGuiderInList(this))
        {
            Debug.Log($"Element In {pathController.name} | Not Found");
            return false;
        }

        int currentIndex = pathController.GetGuiderIndex(this);
        if (currentIndex == pathController.GetGuidersCount() - 1)
            return false;

        GuideFollower targetGuideFollower = pathController.GetGuiderByIndex(currentIndex + 1);

        if (targetGuideFollower.GetPathFollower().GetDistanceTravelled() + childObjects.Count + 1f >= pathFollower.GetDistanceTravelled())
        {
            JoinGuideFollower(targetGuideFollower);
            currentBehavior = Behavior.FollowThePath;
            Debug.Log("Return Following Element");
        }

        return true;
    }

    private bool CheckStopConnection()
    {
        if (!HasPathFollower() || !pathFollower.HasPathController())
            return false;

        PathController pathController = pathFollower.GetPathController();
        if (!pathController.HasGuiderInList(this))
        {
            Debug.Log($"Element In {pathController.name} | Not Found");
            return false;
        }

        int currentIndex = pathController.GetGuiderIndex(this);
        if (currentIndex == pathController.GetGuidersCount() - 1)
            return false;

        GuideFollower targetGuideFollower = pathController.GetGuiderByIndex(currentIndex + 1);
        float offcet = childObjects.Count == 0 ? 1f : childObjects.Count + 1;

        if (targetGuideFollower.GetPathFollower().GetDistanceTravelled() + offcet >= pathFollower.GetDistanceTravelled())
        {
            JoinGuideFollower(targetGuideFollower);
            currentBehavior = Behavior.FollowThePath;
            Debug.Log("Stop Element");
            return true;
        }

        return false;
    }

    private void JoinGuideFollower(GuideFollower guideFollower)
    {
        guideFollower.GetPathFollower().RemoveGuides();

        if (guideFollower.GetPathFollower().HasPathController())
            StartCoroutine(CooldownAction(guideFollower.GetPathFollower().GetPathController().UpdateIndexes, 0.01f));
    }

    private IEnumerator CooldownAction(Action action, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        action();
    }

    #endregion

    private void OnDestroy()
    {
        if (renderer != null)
            renderer.material = tempMaterial;

        name = "Sphere Item";
    }
}