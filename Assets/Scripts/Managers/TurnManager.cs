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
        tickBombs,
        playerTurn,
        enemyAttack,
        enemyMove,
        spawnEnemies
    }
    Turn turn = Turn.playerTurn;

    [SerializeField]
    GameObject gameOverScreen;
    bool gameOver = false;

    [SerializeField]
    GameObject gameWonScreen;
    bool gameWon = false;

    [SerializeField]
    Number gameEndScoreNumber;
    [SerializeField]
    GameObject gameEndScoreParent;

    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Turn Manager...");


        status = ManagerStatus.Started;
    }

    public void Update() {
        if (HandleGameStart()) return;
        if (HandleGameOver()) return;
        if (HandleWin()) return;
        if (DebugMode()) return;

        if (Player == null) {
            Player = FindObjectOfType<Player>();
            return;
        }
        
        if (turn == Turn.tickBombs) {
            Managers._candy.TickBombs();
            PostBombTurn();
            turn = Turn.playerTurn;
        }
        else if (turn == Turn.playerTurn && !Managers._enemy.movingAttackingOrShaking && !Managers._candy.IsExploding() && HandlePlayerInput()) {
            PostPlayerTurn();
            turn = Turn.enemyAttack;
        }
        else if (turn == Turn.enemyAttack && !Player.movingOrAttacking && !Managers._enemy.movingAttackingOrShaking) {
            Managers._enemy.EnemyAttack();
            PostEnemyAttack();
            turn = Turn.enemyMove;
        }
        else if (turn == Turn.enemyMove && !Player.movingOrAttacking && !Managers._enemy.movingAttackingOrShaking) {
            Managers._enemy.EnemyMove();
            PostEnemyMove();
            turn = Turn.spawnEnemies;
        }
        else if (turn == Turn.spawnEnemies && !Player.movingOrAttacking && !Managers._enemy.movingAttackingOrShaking) {
            Managers._enemy.TickSpawnPoints();
            PostEveryTurn();
            turn = Turn.tickBombs;
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
            return Managers._player.UseCandy();
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

    private void PostBombTurn() {

        PostEveryTurn();
    }

    private void PostPlayerTurn() {

        Player.TickVampire();

        PostEveryTurn();
    }

    private void PostEnemyAttack() {
        PostEveryTurn();
    }

    private void PostEnemyMove() {
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
            PostEveryTurn();
        }
        
        if (gameStarted) return false;
        
        startGameScreen.SetActive(true);
        return true;
    }

    private void CheckGameState() {
        if (!Player.isAlive()) {
            gameOver = true;
            gameOverScreen.SetActive(true);
            gameEndScoreNumber.SetNumber(Managers._player.totalScore);
            gameEndScoreParent.SetActive(true);
        }
        else if (Managers._player.totalCandyCount >= 100) {
            gameWon = true;
            gameWonScreen.SetActive(true);
            Managers._player.AddScore(PlayerStatusManager.FINISH_GAME_SCORE);
            gameEndScoreNumber.SetNumber(Managers._player.totalScore);
            gameEndScoreParent.SetActive(true);
        }
    }

    private bool HandleGameOver() {
        if (!gameOver) return false;

        if (Input.GetKeyDown(KeyCode.Space)) {
            gameStarted = true;
            gameOver = false;
            gameWon = false;
            gameOverScreen.SetActive(false);
            gameEndScoreNumber.SetNumber(0);
            gameEndScoreParent.SetActive(false);
            RestartGame();
        }

        if (!gameOver) return false;

        gameOverScreen.SetActive(true);
        return true;
    }

    private bool HandleWin() {
        if (!gameWon) return false;

        if (Input.GetKeyDown(KeyCode.Space)) {
            gameStarted = true;
            gameOver = false;
            gameWon = false;
            gameWonScreen.SetActive(false);
            gameEndScoreNumber.SetNumber(0);
            gameEndScoreParent.SetActive(false);
            RestartGame();
        }

        if (!gameWon) return false;

        gameWonScreen.SetActive(true);
        return true;
    }

    private void SortCharacterLayers() {
        List<Wall> walls = Managers._enemy.Walls;
        Player player = Player;
        List<Enemy> enemies = Managers._enemy.Enemies;
        List<SpawnPoint> spawns = Managers._enemy.SpawnPoints;
        List<Candy> candies = Managers._candy.Candies;
        List<Bomb> bombs = Managers._candy.bombs;
        List<Explosion> explosions = Managers._candy.explosions;
        int currentZ = 0;
        float currentOffset = 0;
        for (int y = 5; y >= 0; y--) {
            for (int x = 0; x < 6; x++) {
                currentOffset = 0;
                //explosions
                var explosion = explosions.Find(e => e.yPos == y && e.xPos == x);
                if (explosion != null) {
                    explosion.gameObject.transform.position = new Vector3(explosion.gameObject.transform.position.x, explosion.gameObject.transform.position.y, currentZ + currentOffset);
                }
                currentOffset += 0.1f;
                //bombs
                var bomb = bombs.Find(b => b.yPos == y && b.xPos == x);
                if (bomb != null) {
                    bomb.gameObject.transform.position = new Vector3(bomb.gameObject.transform.position.x, bomb.gameObject.transform.position.y, currentZ + currentOffset);
                }
                currentOffset += 0.1f;
                //wall
                var wall = walls.Find(w => w.yPos == y && w.xPos == x);
                if (wall != null) {
                    wall.gameObject.transform.position = new Vector3(wall.gameObject.transform.position.x, wall.gameObject.transform.position.y, currentZ + currentOffset);
                }
                currentOffset += 0.1f;

                //player
                if (player.xPos == x && player.yPos == y) {
                    player.gameObject.transform.position = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y, currentZ + currentOffset);
                }
                currentOffset += 0.1f;

                //enemy
                var enemy = enemies.Find(e => e.yPos == y && e.xPos == x);
                if (enemy != null) {
                    enemy.gameObject.transform.position = new Vector3(enemy.gameObject.transform.position.x, enemy.gameObject.transform.position.y, currentZ + currentOffset);
                }
                currentOffset += 0.1f;

                //spawn
                var spawn = spawns.Find(s => s.yPos == y && s.xPos == x);
                if (spawn != null) {
                    spawn.gameObject.transform.position = new Vector3(spawn.gameObject.transform.position.x, spawn.gameObject.transform.position.y, currentZ + currentOffset);
                }
                currentOffset += 0.1f;

                //candy
                var candy = candies.Find(c => c.yPos == y && c.xPos == x);
                if (candy != null) {
                    candy.gameObject.transform.position = new Vector3(candy.gameObject.transform.position.x, candy.gameObject.transform.position.y, currentZ + currentOffset);
                }

                currentZ++;
            }
        }
    }

    private bool DebugMode() {
        return false;

        if (Input.GetKeyDown(KeyCode.R)) {
            RestartGame();
            return true;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            Managers._player.PickUpCandy(CandyManager.CandyTypes.CrossCandy);
            Managers._player.PickUpCandy(CandyManager.CandyTypes.StunLips);
            Managers._player.PickUpCandy(CandyManager.CandyTypes.DuplicateToosie);
            Managers._player.PickUpCandy(CandyManager.CandyTypes.IncreasedAttackCandy);
            Managers._player.PickUpCandy(CandyManager.CandyTypes.BatWings);
            Managers._player.PickUpCandy(CandyManager.CandyTypes.VampireCandy);
            Managers._player.PickUpCandy(CandyManager.CandyTypes.TeleportEnemiesCandy);
            Managers._player.PickUpCandy(CandyManager.CandyTypes.TurnIntoCandy);
            return true;
        }

        return false;
    }
}
