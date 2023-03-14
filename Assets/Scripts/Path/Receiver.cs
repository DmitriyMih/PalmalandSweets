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
                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.GameInProgress)
                        GameManager.Instance.GameStateChanged?.Invoke(false);
                    else
                        pathController.RemoveFollowItem(pathFollower);
                }
                else
                    pathController.RemoveFollowItem(pathFollower);
            }
            Debug.Log("Destroy");
        }
    }
}