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
        SetGuideFollower(null);
        childObjects.Clear();

        childObjects.AddRange(newChilds);
        SetGuideFollower(this);
    }

    private void SetGuideFollower(GuideFollower follower)
    {
        for (int i = 0; i < childObjects.Count; i++)
            if (childObjects[i] != null)
                childObjects[i].SetParentGuideFollower(follower);
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
                speed = -3f;
                isMove = CheckTheBackwardMovement();
                break;

            case Behavior.Stop:
                isMove = false;
                break;
        }
    }

    private bool CheckTheBackwardMovement()
    {
        if (!HasPathFollower() || !pathFollower.HasPathController())
            return false;

        PathController pathController = pathFollower.GetPathController();
        if (pathController.HasGuiderInList(this))
        {
            int index = pathController.GetGuiderIndex(this);
            GuideFollower targetFollower = new GuideFollower();

            if (index == pathController.GetGuidersCount() - 1)
            {
                Debug.Log("End Element");
                currentBehavior = Behavior.Stop;
                return false;
            }
            else
                targetFollower = pathController.GetGuiderByIndex(index + 1);

            if (targetFollower.GetPathFollower().GetDistanceTravelled() + childObjects.Count + 1f >= pathFollower.GetDistanceTravelled())
            {
                currentBehavior = Behavior.Stop;
                Debug.Log("Stop Element");
                return false;
            }
            else
                return true;
        }
        else
        {
            Debug.Log($"Element In {pathController.name} | Not Found");
            return false;
        }
    }

    #endregion

    private void OnDestroy()
    {
        if (renderer != null)
            renderer.material = tempMaterial;

        name = "Sphere Item";
    }
}