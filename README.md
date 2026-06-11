# Elevator Game

A small 2D elevator management game prototype made in Unity.

The player controls an elevator and tries to transport passengers to their desired floors before their patience runs out. Passengers spawn on different floors, wait in queues, enter the elevator, and leave when they reach their target floor.

This project was developed as a short prototype and reached a playable `v0.1` state.

---

## Gameplay

The main goal is to keep passengers satisfied by moving them efficiently between floors.

The player needs to:

- Move the elevator between floors
- Pick up passengers from the left or right side
- Drop passengers off at their target floors
- Manage passenger patience
- Prevent overall satisfaction from dropping too low

---

## Features

- 2D elevator movement
- Multiple floors
- Left and right passenger queues
- Passenger target floor system
- Passenger patience system
- Overall satisfaction system
- Basic UI feedback
- Event-based communication between systems
- Expandable prototype structure

---

## Main Systems

### Elevator

Handles elevator movement, current floor tracking, target floor selection, passenger pickup, and passenger drop-off.

### Floor

Manages passenger spawning, queue positions, entrance points, and elevator door positions.

### Passenger

Controls passenger movement and state changes such as waiting, entering the elevator, riding, exiting, and leaving the floor.

### Passenger Patience

Tracks how long passengers can wait before affecting the overall satisfaction.

### Floor Manager

Creates and tracks floors on both sides of the elevator.

### Satisfaction Manager

Tracks the overall performance of the player based on passenger satisfaction.

---

## Built With

- Unity 6
- C#
- Unity 2D Physics
- TextMeshPro
- Unity UI

---

## Project Status

Current version: `v0.1`

The game is playable as a prototype. Core systems are functional, but visuals, UI, balance, audio feedback, and code structure can be improved in future versions.

---

## Possible Future Improvements

- Upgrade system
- More passenger types
- Better UI/UX
- More audio and visual feedback
- Difficulty scaling
- Random events
- Object pooling
- Cleaner level/floor generation
- Improved game balancing

---

## How to Run

1. Open the project in Unity.
2. Load the main scene.
3. Press Play.

Recommended Unity version:

```txt
Unity 6000.0.x
