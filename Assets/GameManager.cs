using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Vehicle;
    public GameState State;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetGameState(GameState.Building);
    }

    public void SetGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Building:
                // Pause simulation
                Time.timeScale = 0f;
                break;
            case GameState.Flying:
                // Resume simulation
                Time.timeScale = 1f;
                break;
        }
    }
}

public enum GameState
{
    Building,
    Flying
}