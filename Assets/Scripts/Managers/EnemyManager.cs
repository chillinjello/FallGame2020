using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    [SerializeField]
    GameObject PlaceholderEnemyPrefab;

    [SerializeField]
    GameObject SpawnPointPrefab;

    public List<Enemy> Enemies = new List<Enemy>();

    public List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Enemy Manager...");
        
        status = ManagerStatus.Started;
    }

    public bool moving = false;
    public bool attacking = false;
    public bool movingOrAttacking { get { return moving || attacking; } }

    private void Awake() {
        //GameObject testEnemy = Instantiate(PlaceholderEnemyPrefab);
        //Enemies.Add(testEnemy.GetComponent<Enemy>());
        //testEnemy.GetComponent<Enemy>().MoveCharacter(0, 0);
    }

    private void Update() {
        if (movingOrAttacking) {
            //move enemies and see if any are still moving
            attacking = false;
            moving = false;
            foreach(var enemy in Enemies) {
                attacking = enemy.attacking || attacking;
                moving = enemy.moving || moving;
            }
        }
    }
    

    public void EnemyAttack() {
        foreach (var enemy in Enemies) {
            enemy.EnemyAttack();
        }
        attacking = true;
    }

    public void EnemyMove() {
        foreach (var enemy in Enemies) {
            enemy.EnemyMove();
        }
        moving = true;
    }

    public void StartGame() {
        //create starting spawn point
        int x = 2;
        int y = 2;
        while ((x == 2 && y == 2)
            || (x == 2 && y == 3)
            || (x == 3 && y == 2)
            || (x == 3 && y == 3)) {
            x = Random.Range(0, 6);
            y = Random.Range(0, 6);
        }

        CreateNewSpawnPoint(x, y);
    }

    public void CreateRandomSpawnPoint() {
        CreateNewSpawnPoint(Random.Range(0, 6), Random.Range(0, 6));
    }

    public void CreateNewSpawnPoint(int x, int y) {
        if (!BoardManager.CheckValidCoord(x, y)) {
            Debug.Log("Not valid coord!!!");
            return;
        }

        var newSpawn = Instantiate(SpawnPointPrefab).GetComponent<SpawnPoint>();
        SpawnPoints.Add(newSpawn);
        newSpawn.SetPosition(x, y);
    }
    
    public void SpawnEnemy(int x, int y) {
        GameObject testEnemy = Instantiate(PlaceholderEnemyPrefab);
        Enemies.Add(testEnemy.GetComponent<Enemy>());
        testEnemy.GetComponent<Enemy>().MoveCharacter(x, y);
    }

    public void TickSpawnPoints() {
        for (int i = SpawnPoints.Count - 1; i >= 0; i--) {
            var spawnPoint = SpawnPoints[i];
            spawnPoint.SubtractTimer(1);

            if (spawnPoint.GetTime() <= 0) {
                // check to see if an enemy or play is on it
                var playerPos = Managers._turn.Player.GetPos();
                if (playerPos != spawnPoint.coord && Enemies.Find(e => e.GetPos() == spawnPoint.coord) == null) {
                    //spawn enemy 
                    SpawnEnemy((int)spawnPoint.coord.x, (int)spawnPoint.coord.y);

                    //remove point
                    SpawnPoints.RemoveAt(i);
                    Destroy(spawnPoint.gameObject);

                }

                if (spawnPoint.GetTime() == 0) {
                    //create new Spawn Point
                    CreateRandomSpawnPoint();
                }
            }
        }
    }
    
    public void ClearGame() {
        //remove enemies and spawn points
        Enemies.ForEach(e => Destroy(e.gameObject));
        SpawnPoints.ForEach(s => Destroy(s.gameObject));

        //clear lists
        Enemies.Clear();
        SpawnPoints.Clear();
    }
}
