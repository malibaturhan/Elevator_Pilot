using System;
using UnityEngine;

public class Bedrock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Elevator"))
        {
            Debug.Log("Elevator hit bottom");
        }
    }
}
