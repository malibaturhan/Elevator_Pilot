using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum EElevatorMoveStatus
{
    MOVING,
    ON_FLOOR
}

public class Elevator : MonoBehaviour
{
    [Header("Passenger Transport Settings")]
    [SerializeField] private List<Transform> elevatorPoints = new List<Transform>();

    private List<Passenger> PassengersInsideElevator = new List<Passenger>();
    private List<Passenger> PassengersGettingInside = new List<Passenger>();
    private List<Passenger> PassengersGoingFloor = new List<Passenger>();
    private int maxPassengers;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1.0f;

    [Header("References")]
    private BoxCollider2D col;

    private Rigidbody2D rb;

    [Header("Runtime")]
    private Floor currentFloor;
    private int currentFloorIndex = 0;
    private int targetFloorIndex = 0;
    private int topFloorIndex;
    private int bottomFloor;
    private EElevatorMoveStatus movementStatus;
    private bool canMove = true;

    [Header("Invoke check")]
    private Floor arrivalLatestInvokedFloor;

    [Header("Events")]
    public static Action<Floor> OnElevatorArrived;
    public static Action<Floor> OnTargetFloorChanged;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currentFloorText;

    [SerializeField] private TextMeshProUGUI bufferText;

    private void Awake()
    {
    }

    void Start()
    {
        maxPassengers = elevatorPoints.Count;
        SetMinMaxFloor();
        movementStatus = EElevatorMoveStatus.ON_FLOOR;
        col = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Passenger.OnPassengerEnteredElevator += PassengerGettingInsideElevatorCallback;
        Passenger.OnPassengerExitElevator += PassengerGettingOffElevatorCallback;
    }

    private void OnDisable()
    {
        Passenger.OnPassengerEnteredElevator -= PassengerGettingInsideElevatorCallback;
        Passenger.OnPassengerExitElevator -= PassengerGettingOffElevatorCallback;
    }

    private void PassengerGettingInsideElevatorCallback(Passenger passengerRiding)
    {
        if (PassengersGettingInside.Contains(passengerRiding))
        {
            PassengersGettingInside.Remove(passengerRiding);
        }
        ReconsiderCanMove();
    }

    private void PassengerGettingOffElevatorCallback(Passenger passengerGettingOff)
    {
        if (PassengersGoingFloor.Contains(passengerGettingOff))
        {
            PassengersGoingFloor.Remove(passengerGettingOff);
        }

        ReconsiderCanMove();
    }

    private void ReconsiderCanMove()
    {
        if (PassengersGettingInside.Count == 0 && PassengersGoingFloor.Count == 0)
        {
            canMove = true;
        }
    }

    private void SetMinMaxFloor()
    {
        topFloorIndex = FloorManager.Instance.TopFloorIndex;
        bottomFloor = FloorManager.Instance.BottomFloorIndex;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!canMove) return;
        if (targetFloorIndex > currentFloorIndex)
        {
            transform.Translate(Vector2.up * movementSpeed * Time.deltaTime);
            movementStatus = EElevatorMoveStatus.MOVING;
        }
        else if (targetFloorIndex < currentFloorIndex)
        {
            transform.Translate(Vector2.down * movementSpeed * Time.deltaTime);
            movementStatus = EElevatorMoveStatus.MOVING;
        }
        else
        {
            Floor floorGotInto = FloorManager.Instance.GetFloorByStoreyAndSide(targetFloorIndex, EFloorSide.LEFT);
            if (arrivalLatestInvokedFloor != floorGotInto)
            {
                OnElevatorArrived?.Invoke(floorGotInto);
                Debug.Log("ELEVATOR INVOKED ARRIVAL AT " +  floorGotInto.FloorNumber);
                arrivalLatestInvokedFloor = floorGotInto;
            }
            
            movementStatus = EElevatorMoveStatus.ON_FLOOR;
        }
    }

    // UI Button controls
    public void ChangeTargetFloor(int floorAddition)
    {
        targetFloorIndex += floorAddition;
        if (targetFloorIndex > topFloorIndex)
        {
            targetFloorIndex = topFloorIndex;
        }
        else if (targetFloorIndex < bottomFloor)
        {
            targetFloorIndex = bottomFloor;
        }

        OnTargetFloorChanged?.Invoke(FloorManager.Instance.GetFloorByStoreyAndSide(targetFloorIndex, EFloorSide.LEFT));
    }

    // UI Button controls
    public void RequestPassengerFromFloor()
    {
        if (PassengersInsideElevator.Count == maxPassengers)
        {
            Debug.Log("Elevator is full");
            return;
        }

        if (movementStatus == EElevatorMoveStatus.MOVING)
        {
            Debug.Log("Cannot get passengers while moving");
            return;
        }

        Passenger passengerToGetInside = currentFloor.GetPassenger();

        if (passengerToGetInside != null)
        {
            canMove = false;
            passengerToGetInside.GetInsideElevator(elevatorPoints[PassengersInsideElevator.Count]);
            PassengersGettingInside.Add(passengerToGetInside);
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.TryGetComponent(out ElevatorPoint elevatorPoint))
        {
            Debug.LogWarning("This is not an ElevatorPoint: " + collider.name);
            return;
        }

        // no need to control if currentfloor is null since physics interaction prevented
        currentFloor = elevatorPoint.GetCurrentFloor();
        currentFloorIndex = elevatorPoint.GetThisFloorNumber();
        UpdateUI();
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ElevatorPoint"))
        {
            currentFloor = null;
        }
    }

    public bool Arrived => movementStatus == EElevatorMoveStatus.ON_FLOOR;
    public Floor CurrentFloor => currentFloor;

    private void UpdateUI()
    {
        currentFloorText.text = $"Current: {currentFloorIndex}";
    }

    public void ReleasePassenger(Passenger passenger)
    {
        
    }
}