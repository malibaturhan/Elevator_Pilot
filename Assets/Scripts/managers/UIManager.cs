using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Gameobject References")]
    private Elevator elevator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        elevator = FindObjectOfType<Elevator>();
    }

    private void OnEnable()
    {
        Elevator.OnElevatorArrived += ElevatorArrivedCallback;
        Elevator.OnTargetFloorChanged += BufferChangedCallback;
    }

    private void OnDisable()
    {
        Elevator.OnElevatorArrived -= ElevatorArrivedCallback;
        Elevator.OnTargetFloorChanged -= BufferChangedCallback;
    }

    private void ElevatorArrivedCallback(Floor floor)
    {
       
    }

    private void BufferChangedCallback(Floor floor)
    {

    }
    
    
}