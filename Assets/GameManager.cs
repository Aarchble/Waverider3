using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Vehicle;
    public GameState State;

    float previousCameraSize;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        previousCameraSize = Camera.main.orthographicSize;
        SetGameState(GameState.Flying);
    }

    public void SetGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Building:
                // Pause simulation
                Time.timeScale = 0f;

                // Zoom in to building mode
                previousCameraSize = Camera.main.orthographicSize;
                Camera.main.orthographicSize = VehicleStatic.Instance.Length * 1.1f * Screen.height / (2f * Screen.width);

                break;
            case GameState.Flying:
                // Resume simulation
                Time.timeScale = 1f;

                // Zoom back out to flying mode
                Camera.main.orthographicSize = previousCameraSize;

                break;
        }

        State = newState;
    }
}

public enum GameState
{
    Building,
    Flying
}