using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Header("References")]
    private AudioSource audioSource;
    
    [Header("Clips")]
    [SerializeField] private AudioClip elevatorArrivedClip;
    [SerializeField] private AudioClip failClip;
    [SerializeField] private AudioClip newFloorClip;
    [SerializeField] private AudioClip elevatorButtonClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Elevator.OnElevatorArrived += ElevatorArrivedCallback;
        Passenger.OnPassengerExitElevator += PassengerArrivedCallback;
        Elevator.OnImpossibleButtonPressed += ImpossibleButtonPressedCallback;
    } 
    private void OnDisable()
    {
        Elevator.OnElevatorArrived         -= ElevatorArrivedCallback;
        Passenger.OnPassengerExitElevator  -= PassengerArrivedCallback;
        Elevator.OnImpossibleButtonPressed -= ImpossibleButtonPressedCallback;
    }

    private void ImpossibleButtonPressedCallback()
    {
        audioSource.PlayOneShot(failClip);
    }

    private void PassengerArrivedCallback(Passenger arg1, Floor arg2, Floor arg3, float arg4)
    {
        
    }

    private void ElevatorArrivedCallback(Floor obj)
    {
        audioSource.PlayOneShot(elevatorArrivedClip);
        
    }
}
