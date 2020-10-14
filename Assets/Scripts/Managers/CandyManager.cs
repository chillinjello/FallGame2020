using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CandyManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; private set; }

    const int CANDY_TYPES = 10;
    public enum CandyTypes {
        TeleportCandy,
        BombCandy,
        CrossCandy,
        StunLips,
        DuplicateToosie,
        IncreasedAttackCandy,
        BatWings,
        VampireCandy,
        TeleportEnemiesCandy,
        TurnIntoCandy
    }
    [SerializeField]
    GameObject TeleportCandyPrefab;
    [SerializeField]
    GameObject BombCandyPrefab;
    [SerializeField]
    GameObject CrossCandyPrefab;
    [SerializeField]
    GameObject StunLipsPrefab;
    [SerializeField]
    GameObject DuplicateTootsiePrefab;
    [SerializeField]
    GameObject IncreasedAttackCandyPrefab;
    [SerializeField]
    GameObject BatWingsPrefab;
    [SerializeField]
    GameObject VampireCandyPrefab;
    [SerializeField]
    GameObject TeleportEnemiesCandyPrefab;
    [SerializeField]
    GameObject TurnIntoCandyPrefab;


    public List<Candy> Candies = new List<Candy>();
    Candy mainCandy = null;

    [SerializeField]
    GameObject BombPrefab;
    public List<Bomb> bombs = new List<Bomb>();
    public List<Explosion> explosions = new List<Explosion>();

    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Candy Manager...");

        status = ManagerStatus.Started;
    }

    public void StartGame() {
        List<Vector2> outerPositions = BoardManager.GetOuterRingCoords();
        List<Vector2> emptySpaces = BoardItem.FindEmptySpaces();
        List<Vector2> potentialSpaces = new List<Vector2>();
        outerPositions.ForEach(o => {
            if (emptySpaces.FindIndex(e => e.x == o.x && e.y == o.y) != -1) {
                potentialSpaces.Add(o);
            }
        });
        potentialSpaces = potentialSpaces.OrderBy(w => Random.value).ToList();

        SpawnCandy((int)potentialSpaces[0].x, (int)potentialSpaces[0].y);
    }

    public void CandyUpdate() {
        if (mainCandy == null) {
            SpawnCandy();
        }
    }

    public void TickBombs() {
        for (int i = bombs.Count - 1;  i >= 0; i--) {
            if (bombs[i] == null) continue;
            bombs[i].Tick();
        }
    }

    public void RemoveBomb(Bomb bomb) {
        bombs.Remove(bomb);
    }

    public void AddExplosions(List<Explosion> e) {
        explosions.AddRange(e);
    }
    public void AddExplosions(Explosion e) {
        explosions.Add(e);
    }

    public bool IsExploding() {
        if (explosions.Count <= 0) return false;

        if (explosions.FindIndex(e => e != null) == -1) {
            explosions.Clear();
            return false;
        }

        return true;
    }

    private void SpawnCandy(bool main = true) {
        Vector2? emptySpot = BoardItem.FindEmptySpot();
        if (!emptySpot.HasValue) return;
        Vector2 position = emptySpot.Value;

        GameObject candyToSpawn = TeleportCandyPrefab;
        switch(Random.Range(0, CANDY_TYPES)) {
            case 0:
                candyToSpawn = TeleportCandyPrefab;
                break;
            case 1:
                candyToSpawn = BombCandyPrefab;
                break;
            case 2:
                candyToSpawn = CrossCandyPrefab;
                break;
            case 3:
                candyToSpawn = StunLipsPrefab;
                break;
            case 4:
                candyToSpawn = DuplicateTootsiePrefab;
                break;
            case 5:
                candyToSpawn = IncreasedAttackCandyPrefab;
                break;
            case 6:
                candyToSpawn = BatWingsPrefab;
                break;
            case 7:
                candyToSpawn = VampireCandyPrefab;
                break;
            case 8:
                candyToSpawn = TeleportEnemiesCandyPrefab;
                break;
            case 9:
                candyToSpawn = TurnIntoCandyPrefab;
                break;
        }

        var newCandy = Instantiate(candyToSpawn).GetComponent<Candy>();
        Candies.Add(newCandy);
        if (main) mainCandy = newCandy;
        newCandy.SetPosition((int)position.x, (int)position.y);
    }

    private void SpawnCandy(int x, int y, bool main = true) {
        GameObject candyToSpawn = TeleportCandyPrefab;
        switch (Random.Range(0, CANDY_TYPES)) {
            case 0:
                candyToSpawn = TeleportCandyPrefab;
                break;
            case 1:
                candyToSpawn = BombCandyPrefab;
                break;
            case 2:
                candyToSpawn = CrossCandyPrefab;
                break;
            case 3:
                candyToSpawn = StunLipsPrefab;
                break;
            case 4:
                candyToSpawn = DuplicateTootsiePrefab;
                break;
            case 5:
                candyToSpawn = IncreasedAttackCandyPrefab;
                break;
            case 6:
                candyToSpawn = BatWingsPrefab;
                break;
            case 7:
                candyToSpawn = VampireCandyPrefab;
                break;
            case 8:
                candyToSpawn = TeleportEnemiesCandyPrefab;
                break;
            case 9:
                candyToSpawn = TurnIntoCandyPrefab;
                break;
        }

        var newCandy = Instantiate(candyToSpawn).GetComponent<Candy>();
        if (main) mainCandy = newCandy;
        Candies.Add(newCandy);
        Debug.Log("Candy Position: " + x + " " + y);
        newCandy.SetPosition(x, y);
    }

    public void PickUpCandy(Candy candy) {
        if (candy == mainCandy) {
            mainCandy = null;
        }
        Candies.Remove(candy);
        Destroy(candy.gameObject);
    }

    public void ClearGame() {
        Candies.ForEach(c => Destroy(c.gameObject));
        bombs.ForEach(b => Destroy(b.gameObject));
        explosions.ForEach(e => {
            if (e != null)
                Destroy(e.gameObject);
            });

        Candies.Clear();
        bombs.Clear();
        explosions.Clear();
    }
    

    public void TeleportCandyEffect() {
        var emptySpace = BoardItem.FindEmptySpaces();
        if (emptySpace.Count <= 0) return;

        var player = Managers._turn.Player;
        player.MoveCharacter((int)emptySpace[0].x, (int)emptySpace[0].y);

        Managers._turn.SortCharacterLayers();
    }

    public void BombCandyEffect() {
        Bomb bomb = Instantiate(BombPrefab).GetComponent<Bomb>();
        bomb.SetPosition(Managers._turn.Player.xPos, Managers._turn.Player.yPos);
        bombs.Add(bomb);
    }

    [SerializeField]
    GameObject crossExplosion;
    const int CROSS_EXPLOSION_DAMAGE = 5;
    public void CrossCandyEffect() {
        List<Explosion> newExplosions = new List<Explosion>();

        var playerPos = Managers._turn.Player.GetPos();

        for (int x = 0; x < 6; x++) {
            if (x == playerPos.x) continue;

            var newExplosion = Instantiate(crossExplosion).GetComponent<Explosion>();
            newExplosions.Add(newExplosion);
            newExplosion.SetPosition(x, (int)playerPos.y);
        }

        for (int y = 0; y < 6; y++) {
            if (y == playerPos.y) continue;

            var newExplosion = Instantiate(crossExplosion).GetComponent<Explosion>();
            newExplosions.Add(newExplosion);
            newExplosion.SetPosition((int)playerPos.x, y);
        }

        Managers._enemy.Enemies.ForEach(e => {
            if (e.xPos == playerPos.x || e.yPos == playerPos.y) {
                if (e.Attack(CROSS_EXPLOSION_DAMAGE)) {
                    Managers._player.AddScore(PlayerStatusManager.KILL_WITH_COTTON_SCORE);
                }
            }

        });

        AddExplosions(newExplosions);
    }

    public void StunLipsEffect() {
        Managers._turn.Player.AddStunLips();
    }

    public void DuplicateTootsieEffect() {
        List<CandyTypes> potentialCandies = Managers._player.GetCurrentCandyTypes();
        if (potentialCandies.Count > 0)
            Managers._player.PickUpCandy(potentialCandies[Random.Range(0,potentialCandies.Count)]);
    }

    public void IncreaseAttackCandyEffect() {
        Managers._turn.Player.AddIncreasedAttack();
    }

    public void BatWingsEffect() {
        Managers._turn.Player.AddBatWings();
    }

    public void VampireCandyEffect() {
        Managers._turn.Player.AddVampire();
    }

    public void TeleportEnemiesCandyEffect() {
        List<Vector2> emptySpaces = BoardItem.FindEmptySpaces(false, false);
        List<Enemy> enemies = Managers._enemy.Enemies;
        enemies.ForEach(e => {
            Vector2 newSpace = emptySpaces[Random.Range(0, emptySpaces.Count)];
            emptySpaces.Remove(newSpace);
            e.MoveCharacterSmooth((int)newSpace.x, (int)newSpace.y);
        });

        Managers._turn.SortCharacterLayers();
    }

    public void TurnIntoCandyEffect() {
        List<Enemy> enemies = Managers._enemy.Enemies;
        if (enemies.Count <= 0) return;
        Enemy chosenEnemy = enemies[Random.Range(0, enemies.Count)];
        chosenEnemy.Attack(int.MaxValue);
        //if not on wall
        if (Managers._enemy.Walls.FindIndex(w => w.xPos == chosenEnemy.xPos && w.yPos == chosenEnemy.yPos) == -1) {
            SpawnCandy(chosenEnemy.xPos, chosenEnemy.yPos, false);
        }
        Managers._player.AddScore(PlayerStatusManager.TURN_INTO_CANDY_SCORE);
    }
}
