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

    [SerializeField]
    bool debugMode = false;

    private enum Turn {
        playerTurn,
        enemyAttack,
        enemyMove
    }
    Turn turn = Turn.playerTurn;

    [SerializeField]
    GameObject gameOverScreen;
    bool gameOver = false;

    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Turn Manager...");


        status = ManagerStatus.Started;
    }

    public void Update() {
        if (HandleGameStart()) return;
        if (HandleGameOver()) return;
        if (DebugMode()) return;

        if (Player == null) {
            Player = FindObjectOfType<Player>();
            return;
        }
        
        if (turn == Turn.playerTurn && !Managers._enemy.movingOrAttacking && HandlePlayerInput()) {
            PostPlayerTurn();
            turn = Turn.enemyAttack;
        }
        else if (turn == Turn.enemyAttack && !Player.movingOrAttacking && !Managers._enemy.movingOrAttacking) {
            Managers._enemy.EnemyAttack();
            PostEnemyAttack();
            turn = Turn.enemyMove;
        }
        else if (turn == Turn.enemyMove && !Player.movingOrAttacking && !Managers._enemy.movingOrAttacking) {
            Managers._enemy.EnemyMove();
            PostEnemyMove();
            turn = Turn.playerTurn;
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

        PostEveryTurn();
    }

    private void PostEnemyAttack() {

        PostEveryTurn();
    }

    private void PostEnemyMove() {
        Managers._enemy.TickSpawnPoints();

        PostEveryTurn();
    }

    private void PostEveryTurn() {
        Managers._candy.CandyUpdate();
        CheckGameState();
        SortCharacterLayers();
    }

    private void BeginGame() {
        Player = Instantiate(PlayerPrefab).GetComponent<Player>();

        Managers._enemy.StartGame();
        Managers._candy.StartGame();
        Managers._player.StartGame();

        SortCharacterLayers();
    }

    private void ClearGame() {
        //Remove Player
        Destroy(Player.gameObject);
        //Remove Enemies
        Managers._enemy.ClearGame();
        //Remove Candies
        Managers._candy.ClearGame();
    }

    private void RestartGame() {
        ClearGame();
        BeginGame();
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

    private void CheckGameState() {
        if (!Player.isAlive()) {
            gameOver = true;
            gameOverScreen.SetActive(true);
        }
    }

    private bool HandleGameOver() {
        if (!gameOver) return false;

        if (Input.GetKeyDown(KeyCode.Space)) {
            gameStarted = true;
            gameOver = false;
            gameOverScreen.SetActive(false);
            RestartGame();
        }

        if (!gameOver) return false;

        gameOverScreen.SetActive(true);
        return true;
    }

    private void SortCharacterLayers() {
        List<GameObject> characters = new List<GameObject>();

        Managers._enemy.Enemies.ForEach(e => characters.Add(e.gameObject));
        Managers._enemy.Walls.ForEach(w => characters.Add(w.gameObject));
        characters.Add(Player.gameObject);

        characters.Sort((e1, e2) => {
            if (e1.transform.position.y < e2.transform.position.y) {
                return -1;
            }
            else if (e1.transform.position.y > e2.transform.position.y) {
                return 1;
            }
            else {
                if (e1.transform.position.x > e2.transform.position.x)
                    return 1;
                else if (e1.transform.position.x < e2.transform.position.x)
                    return -1;
                else
                    return 0;
            }
        });

        float z = 0;
        float zIncrement = 0.1f;
        for (int i = 0; i < characters.Count; i++) {
            var position = characters[i].transform.position;
            characters[i].transform.position = new Vector3(position.x, position.y, z);
            z += zIncrement;
        }
    }

    private bool DebugMode() {
        if (Input.GetKeyDown(KeyCode.R)) {
            RestartGame();
            return true;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            //Player.SeeIfCharacterCanReachAllSpaces();
            return true;
        }

        return false;
    }
}
