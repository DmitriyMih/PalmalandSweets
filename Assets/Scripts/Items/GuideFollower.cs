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
        childObjects.Clear();
        childObjects.AddRange(newChilds);
    }

    private void FixedUpdate()
    {
        CheckBehavior();

        if (isMove)
            Move();
    }

    private void Move()
    {
        float defaultSpeed = 0;
        
        if (pathFollower != null)
            pathFollower.Move();

        for (int i = 0; i < childObjects.Count; i++)
        {
            childObjects[i].Move(defaultSpeed);
        }
    }

    #region Behaviour
    public void SetBehavior(Behavior behavior)
    {
        currentBehavior = behavior;
    }

    private void CheckBehavior()
    {
        switch(currentBehavior)
        {
            case Behavior.FollowThePath:
                isMove = true;
                isMoveDirectionForward = true;
                break;

            case Behavior.FollowBack:
                isMoveDirectionForward = false;

                isMove = CheckTheBackwardMovement();
                break;

            case Behavior.Stop:
                isMove = false;
                break;
        }
    }

    private bool CheckTheBackwardMovement()
    {
        if (pathFollower == null)
            return false;
        else if (!pathFollower.HasPathController())
            return false;

        PathController pathController = pathFollower.GetPathController();
        if (pathController.HasElementInList(pathFollower))
        {

        }
        else
        {
            Debug.Log($"Element In {pathController.name} | Not Found");
            return false;
        }

        return true;
    }
    #endregion

    private void OnDestroy()
    {
        if (renderer != null)
            renderer.material = tempMaterial;

        name = "Sphere Item";
    }
}