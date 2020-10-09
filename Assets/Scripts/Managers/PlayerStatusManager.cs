using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; private set; }
    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Player Status Manager...");


        status = ManagerStatus.Started;
    }

    [SerializeField]
    TextMesh candyCounterText;
    public int candyCount = 0;

    public int totalScore = 0;

    private void Update() {
        candyCounterText.text = candyCount.ToString();
    }

    public void PickUpCandy(CandyManager.CandyTypes type) {
        candyCount++;
    }

    public void ClearPlayerStats() {
        candyCount = 0;
        totalScore = 0;
    }

    public void StartGame() {
        ClearPlayerStats();
    }
}
