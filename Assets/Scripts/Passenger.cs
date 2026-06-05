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
    [SerializeField] private TextMeshPro neededFloorText;

    [Header("Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float arriveDistance = 0.2f;

    [Header("Runtime References")]
    private Elevator elevator;
    private Floor currentFloor;
    private Vector2 target;
    
    [Header("Runtime Data")]
    private int currentFloorIndex;
    private int targetFloorNumber;
    private Vector2 queuePosition;
    private EPassengerMoveState currentState;

    [Header("Actions / Events")]
    public static Action<Passenger> OnPassengerEnteredElevator;
    public static Action<Passenger> OnPassengerExitElevator;

    private void Start()
    {
        elevator = FindFirstObjectByType<Elevator>();
        gameObject.name = "Passenger_" + Random.Range(1, 999999);
        InitPassenger();
        target = Vector2.zero;
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
        OnPassengerExitElevator    -= PassengerGotOffElevatorCallback;
    }

    private void PassengerGotInsideElevatorCallback(Passenger passengerMoved)
    {
        if (passengerMoved != this && passengerMoved.currentFloor ==  currentFloor) // if a passenger in this floor moved than requeue as visual
        {
            if (currentState == EPassengerMoveState.WAITING_IN_QUEUE)
            {
                target = currentFloor.GetQueuePosition();
            }
        }
    }

    private void PassengerGotOffElevatorCallback(Passenger obj)
    {
        //no op
    }

    private void ElevatorArrivedOnFloorCallback(Floor floor)
    {
        int floorNumber = floor.FloorNumber;
        if (floorNumber == targetFloorNumber && currentState == EPassengerMoveState.RIDING_ELEVATOR)
        {
            currentState = EPassengerMoveState.EXITING_ELEVATOR;
            transform.SetParent(currentFloor.transform);
            elevator.ReleasePassenger(this);
        }
    }

    public void SetPassengerOnFloor(Floor floor, int floorIndex)
    {
        currentFloor = floor;
        currentFloorIndex = floorIndex;
    }

    private void InitPassenger()
    {
        if (currentFloor == null)
        {
            Debug.LogError($"{name}: currentFloor atanmamış. Passenger spawn edilirken SetPassengerOnFloor çağrılmalı.");
            return;
        }
        DetermineFloorToGo();

        currentState = EPassengerMoveState.WALKING_TO_QUEUE;
    }

    private void Update()
    {
        ApplyState();
    }

    private void ApplyState()
    {
        switch (currentState)
        {
            case EPassengerMoveState.WALKING_TO_QUEUE:
                if (target == Vector2.zero)
                {
                    target = currentFloor.GetQueuePosition();
                }

                if (Vector2.Distance(target, transform.position) <= arriveDistance)
                {
                    Debug.Log("QUEUEING");
                    currentState = EPassengerMoveState.WAITING_IN_QUEUE;
                    currentFloor.GetInQueue(this);
                }
                MoveTowards(target);
                break;

            case EPassengerMoveState.WAITING_IN_QUEUE:
                // Burada hiçbir şey yapmaz.
                // Floor uygun zamanda GetInsideElevator() çağırır.
                break;

            case EPassengerMoveState.WALKING_TO_ELEVATOR:
                MoveTowards(target);
                if (Vector2.Distance(target, transform.position) <= arriveDistance)
                {
                    currentState = EPassengerMoveState.RIDING_ELEVATOR;
                }
                break;

            case EPassengerMoveState.RIDING_ELEVATOR:
                transform.SetParent(elevator.transform);
                OnPassengerEnteredElevator?.Invoke(this);
                break;            
            case EPassengerMoveState.EXITING_ELEVATOR:
                target = currentFloor.GetElevatorDoor().position;
                if (target != Vector2.zero)
                {
                    MoveTowards(target);
                }
                if (Vector2.Distance(target, transform.position) <= arriveDistance)
                {
                    currentState = EPassengerMoveState.WALKING_TO_EXIT;
                    OnPassengerExitElevator?.Invoke(this);
                }
                break;

            case EPassengerMoveState.WALKING_TO_EXIT:
                target = currentFloor.GetEntranceDoor().position;
                MoveTowards(target);
                if (Vector2.Distance(target, transform.position) <= arriveDistance)
                {
                    Debug.Log("************passenger arrived successfully");
                    Destroy(gameObject);
                }
                break;
        }
    }
    
    private bool MoveTowards(Vector2 targetPosition)
    {
        if (Vector2.Distance(transform.position, targetPosition) <= arriveDistance)
            return true;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.Translate(direction * walkSpeed * Time.deltaTime);

        return false;
    }

    private void DetermineFloorToGo()
    {
        targetFloorNumber = Random.Range(0, FloorManager.Instance.FloorCount);

        if (targetFloorNumber == currentFloorIndex)
        {
            DetermineFloorToGo();
            return;
        }

        UpdatePassengerUI();
    }

    private void UpdatePassengerUI()
    {
        neededFloorText.text = targetFloorNumber.ToString();
    }


    public void GetInsideElevator(Transform elevatorSpot)
    {
        if (currentState != EPassengerMoveState.WAITING_IN_QUEUE)
            return;

        Debug.Log($"{gameObject.name}: going elevator");
        target = elevatorSpot.position;
        currentState = EPassengerMoveState.WALKING_TO_ELEVATOR;
    }
}