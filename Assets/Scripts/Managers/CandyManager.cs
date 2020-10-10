using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CandyManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; private set; }

    const int CANDY_TYPES = 2;
    public enum CandyTypes {
        TeleportCandy,
        BombCandy
    }
    [SerializeField]
    GameObject TeleportCandyPrefab;
    [SerializeField]
    GameObject BombCandyPrefab;

    public List<Candy> Candies = new List<Candy>();

    [SerializeField]
    GameObject BombPrefab;
    List<Bomb> bombs = new List<Bomb>();
    List<Explosion> explosions = new List<Explosion>();

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
        if (Candies.Count < 1) {
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

    public bool IsExploding() {
        if (explosions.Count <= 0) return false;

        if (explosions.FindIndex(e => e != null) == -1) {
            explosions.Clear();
            return false;
        }

        return true;
    }

    private void SpawnCandy() {
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
        }

        var newCandy = Instantiate(candyToSpawn).GetComponent<Candy>();
        Candies.Add(newCandy);
        newCandy.SetPosition((int)position.x, (int)position.y);
    }

    private void SpawnCandy(int x, int y) {
        GameObject candyToSpawn = TeleportCandyPrefab;
        switch (Random.Range(0, CANDY_TYPES)) {
            case 0:
                candyToSpawn = TeleportCandyPrefab;
                break;
        }

        var newCandy = Instantiate(candyToSpawn).GetComponent<Candy>();
        Candies.Add(newCandy);
        Debug.Log("Candy Position: " + x + " " + y);
        newCandy.SetPosition(x, y);
    }

    public void PickUpCandy(Candy candy) {
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
    }

    public void BombCandyEffect() {
        Bomb bomb = Instantiate(BombPrefab).GetComponent<Bomb>();
        bomb.SetPosition(Managers._turn.Player.xPos, Managers._turn.Player.yPos);
        bombs.Add(bomb);
    }
}
