using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    [SerializeField] private PathController pathController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PathFollower pathFollower))
        {
            if (pathController != null)
            {
                if (!pathController.isEndGame)
                    pathController.EndGameMovement();
                else
                    pathController.RemoveFollowItem(pathFollower);
            }
            Debug.Log("Destroy");
        }
    }
}