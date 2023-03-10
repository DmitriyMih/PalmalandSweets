using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideFollower : MonoBehaviour
{
    [Header("Connect Settings")]
    [SerializeField] private PathFollower pathFollower;

    [Header("Move Settings"), Space(10)]
    [SerializeField] private bool isMoveDirectionForward = true;
    [SerializeField] private bool isMove = true;

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

    public void InitializationChilds(List<PathFollower> newChilds)
    {
        if (newChilds.Count == 0)
            return;

        childObjects.AddRange(newChilds);
    }

    private void FixedUpdate()
    {
        if (isMove)
            Move();
    }

    private void Move()
    {
        if (pathFollower != null)
            pathFollower.Move();

        for (int i = 0; i < childObjects.Count; i++)
        {
            childObjects[i].Move();
        }
    }

    private void OnDestroy()
    {
        if (renderer != null)
            renderer.material = tempMaterial;
    }
}