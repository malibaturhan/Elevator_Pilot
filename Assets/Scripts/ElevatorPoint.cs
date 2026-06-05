using System;
using UnityEngine;

public class ElevatorPoint : MonoBehaviour
{
    [Header("References")]
    private Floor floor;
    private Collider2D col;

    private void Start()
    {
        floor = GetComponentInParent(typeof(Floor)) as Floor;
        col = GetComponentInParent(typeof(Collider2D)) as Collider2D;
    }

    public int GetThisFloorNumber() => floor.FloorNumber;
    
    public Floor GetCurrentFloor() => floor;
}
