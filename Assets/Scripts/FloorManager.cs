using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorManager : Singleton<FloorManager>
{
    public static FloorManager Instance;
    
    [Header("References")]
    private List<Floor> floors = new List<Floor>();

    [Header("Temporary things - to make stuff work")]
    [SerializeField] private int topFloorCount = 4; 
    [SerializeField] private int bottomFloorCount = 0; // can be minus floors later 
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitFloors();
    }

    private void InitFloors()
    {
        floors.AddRange(transform.GetComponentsInChildren<Floor>());
    }

    public Floor GetFloorByStoreyAndSide(int floorIndex, EFloorSide side)
    {
        return floors[floorIndex];
    }
    
    public int FloorCount => floors.Count;
    
    public int TopFloorIndex => topFloorCount;
    public int BottomFloorIndex => bottomFloorCount;
}
