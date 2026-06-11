using System;
using TMPro;
using UnityEngine;


public class StatUIManager : Singleton<StatUIManager>
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI deliveredPassengersText;
    [SerializeField] private TextMeshProUGUI satisfiedCustomersText;
    [SerializeField] private TextMeshProUGUI currentElevatorSpeedText;

    [Header("Stat References")]
    private int satisfiedPassengers;
    private int totalPassengers;
    private float currentElevatorSpeed;

    void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        Passenger.OnPassengerExitElevator += PassengerDeliveredToFloorCallback;
    }

    private void OnDisable()
    {
        Passenger.OnPassengerExitElevator -= PassengerDeliveredToFloorCallback;
    }

    private void PassengerDeliveredToFloorCallback(Passenger passenger, Floor initialFloor, Floor targetFloor,
        float satisfactionRatio)
    {
        totalPassengers++;
        if (satisfactionRatio > PassengerPatience.SatisfactionRatioForGoodSatisfaction)
        {
            satisfiedPassengers++;
        }

        UpdateUI();
    }

    public void UpdateElevatorSpeed(float elevatorSpeed) => this.currentElevatorSpeed = elevatorSpeed;

    private void UpdateUI()
    {
        if (currentElevatorSpeed == 0)
        {
            Elevator elevator = FindFirstObjectByType<Elevator>();
            currentElevatorSpeed = elevator.ElevatorSpeed;
        }
        deliveredPassengersText.text = $"Total delivers: {totalPassengers}";
        deliveredPassengersText.text = $"Satisfied delivers: {satisfiedPassengers}";
        deliveredPassengersText.text = $"speed: {currentElevatorSpeed}";
    }
}