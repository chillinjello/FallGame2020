using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    [SerializeField]
    AudioSource EnemyMoveSound;
    [SerializeField]
    AudioSource BubblingSound;
    [SerializeField]
    List<AudioSource> HitSounds;

    [SerializeField]
    SpriteRenderer witch;
    [SerializeField]
    Sprite movingWitchSprite;
    [SerializeField]
    Sprite nonMovingWitchSprite;
    float witchTime = 0;
    [SerializeField]
    Number witchNumber;
    public void SetWitchSpriteOn() {
        witchTime = .75f;
    }
    private void WitchSprite() {
        if (witchTime <= 0) {
            witchTime = 0;
            witch.sprite = nonMovingWitchSprite;
            return;
        }

        witchTime -= Time.deltaTime;
        witch.sprite = movingWitchSprite;
    }
    public void SetWitchNumber() {
        int max = 0;
        SpawnPoints.ForEach(s => {
            max = Mathf.Max(max, s.GetTime());
        });
        witchNumber.SetNumber(max);
    }

    [SerializeField]
    GameObject PlaceholderEnemyPrefab;
    [SerializeField]
    GameObject VampirePrefab;
    [SerializeField]
    GameObject ZombiePrefab;
    [SerializeField]
    GameObject FrankensteinPrefab;
    [SerializeField]
    GameObject WerewolfPrefab;
    [SerializeField]
    GameObject SpawnPointPrefab;
    [SerializeField]
    GameObject WallPrefab;
    [SerializeField]
    public GameObject EnemyPuffPrefab;

    public List<Enemy> Enemies = new List<Enemy>();

    public List<Wall> Walls = new List<Wall>();

    public List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Enemy Manager...");
        
        status = ManagerStatus.Started;
    }

    public bool moving { get { return Enemies.Find(e => e.moving) != null; } }
    public bool attacking { get { return Enemies.Find(e => e.attacking) != null; } }
    public bool shaking { get { return Enemies.Find(e => e.shaking) != null; } }
    public bool movingFromCauldron { get { return Enemies.Find(e => e.movingFromCauldron) != null; } }
    public bool movingAttackingOrShaking { get { return moving || attacking || shaking || movingFromCauldron; } }

    private void Awake() {
    }

    private void LateUpdate() {
        WitchSprite();
        
        if (SpawnPoints.Count <= 0 && Managers._turn.gameStarted) {
            CreateRandomSpawnPoint();
        }
    }
    

    public void EnemyAttack() {
        //sort enemies
        Enemies.Sort((e1,e2) => (e1.yPos * 10 + e1.xPos) - (e2.yPos * 10 + e2.xPos));
        foreach (var enemy in Enemies) {
            enemy.EnemyAttack();
        }
        if (attacking)
            EnemyHitSounds();
    }
    public void EnemyHitSounds() {
        HitSounds[Random.Range(0, HitSounds.Count)].Play();
    }

    public void EnemyMove() {
        //sort enemies
        Enemies.Sort((e1, e2) => (e1.yPos * 10 + e1.xPos) - (e2.yPos * 10 + e2.xPos));
        foreach (var enemy in Enemies) {
            enemy.EnemyMove();
        }
        if (moving)
            EnemyMoveSound.Play();
    }

    public void StartGame() {
        do {
            ClearWalls();

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
        } while (!Managers._turn.Player.SeeIfCharacterCanReachAllSpaces());


        //create starting spawn point
        int x = 2;
        int y = 2;
        bool inWall = false;
        while (((x == 2 && y == 2)
            || (x == 2 && y == 3)
            || (x == 3 && y == 2)
            || (x == 3 && y == 3)) || inWall) {
            inWall = false;

            x = Random.Range(0, 6);
            y = Random.Range(0, 6);
            
            if (Walls.FindIndex(w => w.xPos == x && w.yPos == y) != -1) inWall = true;
        }

        CreateNewSpawnPoint(x, y);
        int xi = x;
        int yi = y;
        while (((x == 2 && y == 2)
            || (x == 2 && y == 3)
            || (x == 3 && y == 2)
            || (x == 3 && y == 3)) 
            || inWall
            || (x == xi && y == yi)) {
            inWall = false;

            x = Random.Range(0, 6);
            y = Random.Range(0, 6);

            if (Walls.FindIndex(w => w.xPos == x && w.yPos == y) != -1) inWall = true;
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
        SetWitchNumber();
    }
    
    public void SpawnEnemy(int x, int y) {
        var enemyType = PlaceholderEnemyPrefab;
        switch (Random.Range(0,5)) {
            case 0:
                enemyType = PlaceholderEnemyPrefab;
                break;
            case 1:
                enemyType = VampirePrefab;
                break;
            case 2:
                enemyType = ZombiePrefab;
                break;
            case 3:
                enemyType = FrankensteinPrefab;
                break;
            case 4:
                enemyType = WerewolfPrefab;
                break;
        }

        GameObject testEnemy = Instantiate(enemyType);
        Enemies.Add(testEnemy.GetComponent<Enemy>());
        testEnemy.GetComponent<Enemy>().SetMoveFromCauldron(x, y);
    }

    public void TickSpawnPoints() {
        bool spawned = false;
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
                    spawned = true;
                }
            }
        }
        if (spawned) BubblingSound.Play();
        SetWitchNumber();
    }

    public void CheckSpawnPoints() {
        bool spawned = false;
        for (int i = SpawnPoints.Count - 1; i >= 0; i--) {
            var spawnPoint = SpawnPoints[i];

            if (spawnPoint.GetTime() <= 0) {
                // check to see if an enemy or play is on it
                var playerPos = Managers._turn.Player.GetPos();
                if (playerPos != spawnPoint.GetPos() && Enemies.Find(e => e.GetPos() == spawnPoint.GetPos() && e.isAlive()) == null) {
                    //spawn enemy 
                    SpawnEnemy((int)spawnPoint.xPos, (int)spawnPoint.yPos);

                    //remove point
                    SpawnPoints.RemoveAt(i);
                    Destroy(spawnPoint.gameObject);
                    spawned = true;
                }
            }
        }
        if (spawned) BubblingSound.Play();
        SetWitchNumber();
    }
    
    public void ClearGame() {
        ClearEnemies();
        ClearSpawn();
        ClearWalls();
    }
    private void ClearEnemies() {
        Enemies.ForEach(e => Destroy(e.gameObject));
        Enemies.Clear();
    }
    private void ClearSpawn() {
        SpawnPoints.ForEach(s => Destroy(s.gameObject));
        SpawnPoints.Clear();
    }
    private void ClearWalls() {
        Walls.ForEach(w => Destroy(w.gameObject));
        Walls.Clear();
    }

    public bool CheckIfWallOnTile(int x, int y) {
        return Walls.Find(w => w.xPos == x && w.yPos == y) == null;
    }
}
