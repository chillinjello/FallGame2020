using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; private set; }

    [SerializeField]
    public Player Player;

    bool playerTurn = true;

    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Turn Manager...");


        status = ManagerStatus.Started;
    }

    public void Update() {
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

    }
}
