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

    private void ElevatorArrivedCallback(int val)
    {
       
    }

    private void BufferChangedCallback(int val)
    {

    }
}