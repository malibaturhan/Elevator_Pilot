using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EPassengerMoveState
{
    WALKING_TO_QUEUE,
    WAITING_IN_QUEUE,
    WALKING_TO_ELEVATOR,
    RIDING_ELEVATOR,
    EXITING_ELEVATOR,
    WALKING_TO_EXIT
}

public class Passenger : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshPro wantedFloorText;

    [Header("Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float arriveDistance = 0.2f;

    [Header("Runtime References")]
    private Elevator elevator;
    private Floor currentFloor;
    private Vector2 target;

    [Header("Runtime Data")]
    private int currentFloorIndex;
    private Floor targetFloor;
    private Vector2 queuePosition;
    private EPassengerMoveState currentState;

    // İlk state girişinde enum default değerinden dolayı ChangeState'in return etmesini engeller.
    private bool hasStateInitialized = false;

    [Header("Actions / Events")]
    public static Action<Passenger> OnPassengerEnteredElevator;
    public static Action<Passenger> OnPassengerExitElevator;

    private void Start()
    {
        elevator = FindFirstObjectByType<Elevator>();
        gameObject.name = "Passenger_" + Random.Range(1, 999999);

        InitPassenger();
    }

    private void OnEnable()
    {
        Elevator.OnElevatorArrived += ElevatorArrivedOnFloorCallback;
        OnPassengerEnteredElevator += PassengerGotInsideElevatorCallback;
        OnPassengerExitElevator += PassengerGotOffElevatorCallback;
    }

    private void OnDisable()
    {
        Elevator.OnElevatorArrived -= ElevatorArrivedOnFloorCallback;
        OnPassengerEnteredElevator -= PassengerGotInsideElevatorCallback;
        OnPassengerExitElevator -= PassengerGotOffElevatorCallback;
    }

    private void InitPassenger()
    {
        if (currentFloor == null)
        {
            Debug.LogError($"{name}: currentFloor atanmamış. Passenger spawn edilirken SetPassengerOnFloor çağrılmalı.");
            return;
        }

        DetermineFloorToGo();
        
        ChangeState(EPassengerMoveState.WALKING_TO_QUEUE);
    }

    public void SetPassengerOnFloor(Floor floor, int floorIndex)
    {
        currentFloor = floor;
        currentFloorIndex = floorIndex;
    }

    private void Update()
    {
        ApplyState();
    }

    private void ChangeState(EPassengerMoveState newState)
    {
        // Aynı state'e tekrar girmeyi engeller.
        // Ama ilk girişte bu kontrol çalışmaz; çünkü hasStateInitialized false.
        if (hasStateInitialized && currentState == newState)
            return;

        currentState = newState;
        hasStateInitialized = true;

        switch (newState)
        {
            case EPassengerMoveState.WALKING_TO_QUEUE:
                target = currentFloor.GetQueuePosition();
                break;

            case EPassengerMoveState.WAITING_IN_QUEUE:
                currentFloor.GetInQueue(this);
                break;

            case EPassengerMoveState.WALKING_TO_ELEVATOR:
                break;

            case EPassengerMoveState.RIDING_ELEVATOR:
                transform.SetParent(elevator.transform);
                OnPassengerEnteredElevator?.Invoke(this);
                break;

            case EPassengerMoveState.EXITING_ELEVATOR:
                transform.SetParent(currentFloor.transform);
                target = currentFloor.GetElevatorDoor().position;
                elevator.PassengerGettingOut(this);
                break;

            case EPassengerMoveState.WALKING_TO_EXIT:
                OnPassengerExitElevator?.Invoke(this);
                target = currentFloor.GetEntranceDoor().position;
                break;
        }
    }

    private void ApplyState()
    {
        switch (currentState)
        {
            case EPassengerMoveState.WALKING_TO_QUEUE:
                MoveTowards(target);

                if (HasArrived(target))
                {
                    ChangeState(EPassengerMoveState.WAITING_IN_QUEUE);
                }

                break;

            case EPassengerMoveState.WAITING_IN_QUEUE:
                break;

            case EPassengerMoveState.WALKING_TO_ELEVATOR:
                MoveTowards(target);

                if (HasArrived(target))
                {
                    ChangeState(EPassengerMoveState.RIDING_ELEVATOR);
                }

                break;

            case EPassengerMoveState.RIDING_ELEVATOR:
                break;

            case EPassengerMoveState.EXITING_ELEVATOR:
                MoveTowards(target);

                if (HasArrived(target))
                {
                    ChangeState(EPassengerMoveState.WALKING_TO_EXIT);
                }

                break;

            case EPassengerMoveState.WALKING_TO_EXIT:
                MoveTowards(target);

                if (HasArrived(target))
                {
                    Destroy(gameObject);
                }

                break;
        }
    }

    private bool MoveTowards(Vector2 targetPosition)
    {
        if (HasArrived(targetPosition))
            return true;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.Translate(direction * walkSpeed * Time.deltaTime);

        return false;
    }

    private bool HasArrived(Vector2 targetPosition)
    {
        return Vector2.Distance(transform.position, targetPosition) <= arriveDistance;
    }

    private void ElevatorArrivedOnFloorCallback(Floor floor)
    {
        int floorNumber = floor.FloorNumber;

        if (floorNumber == targetFloor.FloorNumber && currentState == EPassengerMoveState.RIDING_ELEVATOR)
        {
            currentFloor = targetFloor;
            currentFloorIndex = currentFloor.FloorNumber;

            ChangeState(EPassengerMoveState.EXITING_ELEVATOR);
        }
    }

    private void PassengerGotInsideElevatorCallback(Passenger passengerMoved)
    {
        if (passengerMoved != this && passengerMoved.currentFloor == currentFloor)
        {
            if (currentState == EPassengerMoveState.WAITING_IN_QUEUE)
            {
                target = currentFloor.GetQueuePosition();
            }
        }
    }

    private void PassengerGotOffElevatorCallback(Passenger passenger)
    {
        // no op
    }

    private void DetermineFloorToGo()
    {
        bool isAtTopFloor = FloorManager.Instance.TopFloorIndex == currentFloor.FloorNumber;
        bool isAtBottomFloor = FloorManager.Instance.BottomFloorIndex == currentFloor.FloorNumber;
        bool goingUp = Random.Range(0, 2) == 0;
        if (isAtBottomFloor)
        {
            goingUp = true;
        }
        else if (isAtTopFloor)
        {
            goingUp = false;
        }
        bool goingLeft = Random.Range(0, 2) == 0;
        

        int floorNumberToGo = 0;
        if (goingUp)
        {
            floorNumberToGo = Random.Range(currentFloor.FloorNumber + 1, FloorManager.Instance.TopFloorIndex + 1);
        }
        else
        {
            floorNumberToGo = Random.Range(FloorManager.Instance.BottomFloorIndex, currentFloor.FloorNumber);
        }
        
        targetFloor = FloorManager.Instance.GetFloorByStoreyAndSide(floorNumberToGo, goingLeft ? EFloorSide.LEFT : EFloorSide.RIGHT);
        UpdatePassengerUI();
    }

    private void UpdatePassengerUI()
    {
        wantedFloorText.text = targetFloor.ToString();
    }

    public void GetInsideElevator(Transform elevatorSpot)
    {
        if (currentState != EPassengerMoveState.WAITING_IN_QUEUE)
            return;

        target = elevatorSpot.position;
        ChangeState(EPassengerMoveState.WALKING_TO_ELEVATOR);
    }
}