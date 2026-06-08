using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    private float floorHeight = 0;
    
    [Header("References")]
    private BoxCollider2D entranceTrigger;
    private TextMeshPro floorText;
    [SerializeField] private SpriteRenderer floorSpriteRenderer;
    [SerializeField] private Passenger PassengerPrefab;
    [SerializeField] private Transform leftSideDoorTransform;
    [SerializeField] private Transform rightSideDoorTransform;    
    [SerializeField] private ElevatorPoint leftElevatorPoint;
    [SerializeField] private ElevatorPoint rightElevatorPoint;
    
    [Header("Floor Attributes")]
    [SerializeField] private int floorNumber = 0;
    private EFloorSide currentSide;

    [Header("Floor Settings")]
    [SerializeField] [Range(0.4f, 15f)] private float intervalsBetweenPassengerSpawns;
    [SerializeField] [Range(0.1f, 5f)] private float deviationBetweenPassengerSpawns;
    [SerializeField] private int maxPassengers = 1;
    [SerializeField] private float distanceBetweenQueuedPassenger = 1.5f;
    [SerializeField] private float distanceToGetInQueue = 0.2f;
    
    [Header("Runtime")]
    private float passengerSpawnTimer;
    private int passengersOnTheFloorCount;
    private Queue<Passenger> PassengerQueue;
    private Transform entranceDoor;
    private Transform elevatorDoor;

    void Awake()
    {
        
    }



    void Start()
    {
        gameObject.name = $"Floor {floorNumber} {currentSide}";
        floorText = GetComponentInChildren<TextMeshPro>();
        floorText.text = floorNumber.ToString();
        ResetPassengerSpawnTimer();
        PassengerQueue = new Queue<Passenger>();
        
    }

    // Update is called once per frame
    void Update()
    {
        passengerSpawnTimer -= Time.deltaTime;
        if (passengerSpawnTimer <= 0)
        {
            if (maxPassengers > passengersOnTheFloorCount)
            {
                SpawnPassenger();
            }
        }
        
    }

    #region Spawn Passengers

    private void SpawnPassenger()
    {
        var passenger = Instantiate(PassengerPrefab, entranceDoor.position, Quaternion.identity);
        passenger.SetPassengerOnFloor(this, floorNumber);
        passengersOnTheFloorCount++;
        ResetPassengerSpawnTimer();
    }

    private void ResetPassengerSpawnTimer() => passengerSpawnTimer = GetPassengerSpawnTime();

    private float GetPassengerSpawnTime() =>
        intervalsBetweenPassengerSpawns + Random.Range(0, deviationBetweenPassengerSpawns);

    #endregion

    public void GetInQueue(Passenger passenger)
    {
        PassengerQueue.Enqueue(passenger);
    }

    public Transform GetEntranceDoor() => entranceDoor.transform;
    public Transform GetElevatorDoor() => elevatorDoor.transform;

    public int FloorNumber => floorNumber;
    public Transform FloorsElevatorDoor => elevatorDoor.transform;


    public Passenger GetPassenger()
    {
        if (PassengerQueue.Count == 0)
        {
            Debug.Log("NO PASSENGER FOUND ON FLOOR " + floorNumber);
            return null;
        }
        Passenger passengerToRideElevator =  PassengerQueue.Dequeue();
        passengersOnTheFloorCount--;
        return passengerToRideElevator;
    }

    public Vector2 GetQueuePosition()
    {
        Vector2 getInQueuePosition = new Vector2(
            (elevatorDoor.position.x - distanceBetweenQueuedPassenger * PassengerQueue.Count),
            elevatorDoor.position.y);
        // Debug.Log("Get In Queue Pos: " +getInQueuePosition.ToString());
        return getInQueuePosition;
    }

    public float Height
    {
        get
        {
            if (floorSpriteRenderer == null)
            {
                Debug.LogError($"{name}: floorSpriteRenderer atanmamış.");
                return 0f;
            }

            return floorSpriteRenderer.bounds.size.y;
        }
    }
    public void SetInitialValues(int floorNumber, EFloorSide floorSide = EFloorSide.LEFT)
    {
        this.floorNumber = floorNumber;
        currentSide = floorSide;
        SetElementsAccordingToSide();
    }
    private void SetElementsAccordingToSide()
    {
        if (currentSide == EFloorSide.LEFT)
        {
            entranceDoor = leftSideDoorTransform;
            elevatorDoor = rightSideDoorTransform;
            leftElevatorPoint.enabled = false;
        }   
        else if (currentSide == EFloorSide.RIGHT)
        {
            entranceDoor = rightSideDoorTransform;
            elevatorDoor = leftSideDoorTransform;
            rightElevatorPoint.enabled = false;
        }
    }

    public override string ToString()
    {
        string sideStr = currentSide == EFloorSide.LEFT ? "L" : "R";
        return $"{floorNumber} {sideStr}";
    }
}