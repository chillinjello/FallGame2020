using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CandyManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; private set; }

    const int CANDY_TYPES = 1;
    public enum CandyTypes {
        PlaceholderCandy,
    }
    [SerializeField]
    GameObject PlaceholderCandyPrefab;

    public List<Candy> Candies = new List<Candy>();

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

    private void SpawnCandy() {
        Vector2? emptySpot = BoardItem.FindEmptySpot();
        if (!emptySpot.HasValue) return;
        Vector2 position = emptySpot.Value;

        GameObject candyToSpawn = PlaceholderCandyPrefab;
        switch(Random.Range(0, CANDY_TYPES)) {
            case 0:
                candyToSpawn = PlaceholderCandyPrefab;
                break;
        }

        var newCandy = Instantiate(candyToSpawn).GetComponent<Candy>();
        Candies.Add(newCandy);
        newCandy.SetPosition((int)position.x, (int)position.y);
    }

    private void SpawnCandy(int x, int y) {
        GameObject candyToSpawn = PlaceholderCandyPrefab;
        switch (Random.Range(0, CANDY_TYPES)) {
            case 0:
                candyToSpawn = PlaceholderCandyPrefab;
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

        Candies.Clear();
    }

}
