using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseSphereItem sphereItem))
        {
            Destroy(sphereItem.gameObject);
            Debug.Log("Destroy");
        }
    }
}