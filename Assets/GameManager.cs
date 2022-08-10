using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Vehicle;
    public GameState State;
    public static PlayerInputActions playerControls;
    InputAction switchBuildFly;

    float previousCameraSize;

    private void Awake()
    {
        Instance = this;
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        switchBuildFly = playerControls.ManagementControls.SwitchBuildFly;
        switchBuildFly.Enable();
        switchBuildFly.performed += ToggleBuildFly;
    }

    private void OnDisable()
    {
        switchBuildFly.performed -= ToggleBuildFly;
        switchBuildFly.Disable();
    }

    private void Start()
    {
        previousCameraSize = Camera.main.orthographicSize;
        StartBuilding();
    }

    private void StartBuilding()
    {
        State = GameState.Building;

        // Pause simulation
        VehiclePhysics.Instance.StartActivePause();

        // Zoom in to building mode
        previousCameraSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize = VehicleStatic.Instance.Length * 1.5f * Screen.height / (2f * Screen.width);
    }

    private void StartFlying()
    {
        State = GameState.Flying;

        // Zoom back out to flying mode
        Camera.main.orthographicSize = previousCameraSize;

        // Resume simulation
        VehiclePhysics.Instance.StopActivePause();
    }

    private void ToggleBuildFly(InputAction.CallbackContext context)
    {
        switch (State)
        {
            case GameState.Building:
                StartFlying();
                break;
            case GameState.Flying:
                StartBuilding();
                break;
            default:
                break;
        }
    }
    
}

public enum GameState
{
    Building,
    Flying
}