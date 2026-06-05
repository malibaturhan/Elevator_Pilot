using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    [Header("References")]
    private BoxCollider2D entranceTrigger;

    private TextMeshPro floorText;
    [SerializeField] private Passenger PassengerPrefab;
    [SerializeField] private Transform entranceDoor;
    [SerializeField] private Transform elevatorDoor;

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

    void Start()
    {
        floorText = GetComponentInChildren<TextMeshPro>();
        floorText.text = floorNumber.ToString();
        currentSide = EFloorSide.LEFT; // FOR NOW ALL OF THEM ARE IN LEFT BANK
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

    public int FloorNumber => floorNumber;
    public Transform FloorsElevatorDoor => elevatorDoor.transform;


    public void GetPassenger()
    {
        if (PassengerQueue.Count == 0)
        {
            Debug.Log("NO PASSENGER FOUND ON FLOOR " + floorNumber);
            return;
        }
        Passenger passengerToRideElevator =  PassengerQueue.Dequeue();
        passengerToRideElevator.GetInsideElevator();
    }

    public Vector2 GetQueuePosition()
    {
        Vector2 getInQueuePosition = new Vector2(
            (elevatorDoor.position.x - distanceBetweenQueuedPassenger * PassengerQueue.Count),
            elevatorDoor.position.y);
        Debug.Log("Get In Queue Pos: " +getInQueuePosition.ToString());
        return getInQueuePosition;
    }
}