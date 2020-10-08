using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    [SerializeField]
    GameObject PlaceholderEnemyPrefab;
    [SerializeField]
    GameObject SpawnPointPrefab;
    [SerializeField]
    GameObject WallPrefab;

    public List<Enemy> Enemies = new List<Enemy>();

    public List<Wall> Walls = new List<Wall>();

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
        int x;
        int y;
        //create walls
        List<Vector2> innerWallPositions = BoardManager.GetInnerRingCoords();
        List<Vector2> outerWallPositions = BoardManager.GetOuterRingCoords();
        outerWallPositions = outerWallPositions.OrderBy(w => Random.value).ToList();
        innerWallPositions = innerWallPositions.OrderBy(w => Random.value).ToList();

        for (int i = 0; i < 3; i++) {
            Wall newWall = Instantiate(WallPrefab).GetComponent<Wall>();
            newWall.SetWallPosition((int)innerWallPositions[i].x, (int)innerWallPositions[i].y);
            Walls.Add(newWall);
        }
        for (int i = 0; i < 3; i++) {
            Wall newWall = Instantiate(WallPrefab).GetComponent<Wall>();
            newWall.SetWallPosition((int)outerWallPositions[i].x, (int)outerWallPositions[i].y);
            Walls.Add(newWall);
        }


        //create starting spawn point
        x = 2;
        y = 2;
        while ((x == 2 && y == 2)
            || (x == 2 && y == 3)
            || (x == 3 && y == 2)
            || (x == 3 && y == 3)) {
            
            x = Random.Range(0, 6);
            y = Random.Range(0, 6);
            
            if (Walls.FindIndex(w => w.xPos == x && w.yPos == y) != -1) continue;
        }

        CreateNewSpawnPoint(x, y);
    }

    public void CreateRandomSpawnPoint() {
        int x = Random.Range(0, 6);
        int y = Random.Range(0, 6);
        while (Walls.FindIndex(w => w.xPos == x && w.yPos == y) != -1) {
            x = Random.Range(0, 6);
            y = Random.Range(0, 6);
        }

        CreateNewSpawnPoint(x, y);
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
                if (playerPos != spawnPoint.GetPos() && Enemies.Find(e => e.GetPos() == spawnPoint.GetPos()) == null) {
                    //spawn enemy 
                    SpawnEnemy((int)spawnPoint.xPos, (int)spawnPoint.yPos);

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
        Walls.ForEach(w => Destroy(w.gameObject));

        //clear lists
        Enemies.Clear();
        SpawnPoints.Clear();
        Walls.Clear();
    }

    public bool CheckIfWallOnTile(int x, int y) {
        return Walls.Find(w => w.xPos == x && w.yPos == y) == null;
    }
}
