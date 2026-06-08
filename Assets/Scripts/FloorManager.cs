using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorManager : Singleton<FloorManager>
{
    [Header("Settings")]
    [SerializeField] private int numberOfFloorsToGenerate = 4;

    [Header("References")]
    private List<Floor> floors = new List<Floor>();
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private Transform leftBankRoot;
    [SerializeField] private Transform rightBankRoot;

    private int bottomFloorCount = 0; // can be minus floors later 

    [Header("Runtime")]
    private float leftNextFloorY;
    private float rightNextFloorY;

    void Start()
    {
        leftNextFloorY = leftBankRoot.position.y;
        rightNextFloorY = rightBankRoot.position.y;
        Debug.Log("FIRST NEXT FLOOR Y: " + leftNextFloorY);
        InitFloors();
    }


    private void InitFloors()
    {
        for (int i = 0; i < numberOfFloorsToGenerate; i++)
        {
            CreateFloorAt(i, EFloorSide.LEFT);
        }

        for (int i = 0; i < numberOfFloorsToGenerate; i++)
        {
            CreateFloorAt(i, EFloorSide.RIGHT);
        }

    }

    private void CreateFloorAt(int i, EFloorSide side = EFloorSide.LEFT)
    {
        GameObject newFloorInstance = Instantiate(floorPrefab, transform);
        if (side == EFloorSide.LEFT)
        {
            newFloorInstance.transform.SetParent(leftBankRoot);
        }
        else
        {
            newFloorInstance.transform.SetParent(rightBankRoot);
        }
        Floor newFloor = newFloorInstance.GetComponent<Floor>();

        newFloor.SetInitialValues(i,  side);
        float xPos = side == EFloorSide.LEFT ? leftBankRoot.position.x : rightBankRoot.position.x;
        float yPos = side == EFloorSide.LEFT ? leftNextFloorY : rightNextFloorY;
        newFloorInstance.transform.position = new Vector2(
            xPos,
            yPos
        );

        floors.Add(newFloor);

        if (side == EFloorSide.LEFT)
        {
            leftNextFloorY += newFloor.Height;
        }
        else if (side == EFloorSide.RIGHT)
        {
            rightNextFloorY += newFloor.Height;
        }

        Debug.Log("created and added");
    }


    public Floor GetFloorByStoreyAndSide(int floorIndex, EFloorSide side)
    {
        if (side == EFloorSide.LEFT)
        {
            foreach (Floor floor in leftBankRoot.GetComponentsInChildren<Floor>())
            {
                if (floor.FloorNumber == floorIndex) return floor;
            }
        }
        else if (side == EFloorSide.RIGHT)
        {
            foreach (Floor floor in rightBankRoot.GetComponentsInChildren<Floor>())
            {
                if (floor.FloorNumber == floorIndex) return floor;
            }
        }
    
        return null;
    }

    public int FloorCount => floors.Count;

    public int TopFloorIndex =>  numberOfFloorsToGenerate - 1;
    public int BottomFloorIndex => bottomFloorCount;
}