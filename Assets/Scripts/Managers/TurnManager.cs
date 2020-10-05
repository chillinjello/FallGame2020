using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; private set; }

    [SerializeField]
    GameObject PlayerPrefab;
    public Player Player;

    [SerializeField]
    GameObject startGameScreen;
    bool gameStarted = false;

    bool playerTurn = true;

    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Turn Manager...");


        status = ManagerStatus.Started;
    }

    public void Update() {
        if (HandleGameStart()) return;

        if (Player == null) {
            Player = FindObjectOfType<Player>();
            return;
        }
        
        if (playerTurn && HandlePlayerInput()) {
            PostPlayerTurn();
            playerTurn = false;
        }
        else if (!playerTurn && !Player.moving) {
            Managers._enemy.EnemyTurn();
            PostEnemyTurn();
            playerTurn = true;
        }
        else if (!playerTurn && GettingPlayerInput()) {
            Managers._enemy.EnemyTurn();
            PostEnemyTurn();
            playerTurn = true;
            HandlePlayerInput();
            PostPlayerTurn();
            playerTurn = false;
        }
    }

    private bool HandlePlayerInput() {
        Character.MoveDirections direction = Character.MoveDirections.none;
        if (Input.GetKeyDown(KeyCode.A)) {
            direction = Character.MoveDirections.left;
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            direction = Character.MoveDirections.down;
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            direction = Character.MoveDirections.right;
        }
        else if (Input.GetKeyDown(KeyCode.W)) {
            direction = Character.MoveDirections.up;
        }
        else {
            return false;
        }

        return Player.MoveCharacter(direction);
    }

    private bool GettingPlayerInput() {
        if (Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.D)
            || Input.GetKeyDown(KeyCode.W)) {
            return true;
        }
        else {
            return false;
        }
    }

    private void PostPlayerTurn() {
        var Enemies = Managers._enemy.Enemies;
        for (int i = Enemies.Count - 1; i >= 0; i--) {
            var currentEnemy = Enemies[i];
            if (!currentEnemy.isAlive()) {
                Destroy(currentEnemy.gameObject);
                Enemies.RemoveAt(i);
            }
        }
    }

    private void PostEnemyTurn() {
        Managers._enemy.TickSpawnPoints();
    }

    private void BeginGame() {
        Player = Instantiate(PlayerPrefab).GetComponent<Player>();

        Managers._enemy.StartGame();
    }

    private bool HandleGameStart() {
        if (gameStarted) return false;

        if (Input.GetKeyDown(KeyCode.Space)) {
            gameStarted = true;
            startGameScreen.SetActive(false);
            BeginGame();
        }
        
        if (gameStarted) return false;
        
        startGameScreen.SetActive(true);
        return true;
    }
}
