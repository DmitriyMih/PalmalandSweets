using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ClampRoadPosition : MonoBehaviour
{
    [SerializeField] private float clampPositionY = -0.4f;

    private void Update()
    {
        if (transform.position.y != clampPositionY)
            transform.position = new Vector3(transform.position.x, clampPositionY, transform.position.z);
    }
}