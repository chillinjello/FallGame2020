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
    Number totalCandyCounterNumber;
    public int totalCandyCount = 0;

    public int totalScore = 0;
    
    //candy counter sprites
    [SerializeField]
    Number teleportCandyNumber;
    private int teleportCandyCount = 0;
    [SerializeField]
    Number bombCandyNumber;
    private int bombCandyCount = 0;
    

    public bool UseCandy() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (teleportCandyCount <= 0) return false;

            Managers._candy.TeleportCandyEffect();

            teleportCandyCount--;
            SetCandyCount();
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            if (bombCandyCount <= 0) return false;

            Managers._candy.BombCandyEffect();

            bombCandyCount--;
            SetCandyCount();
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0)) {
            return true;
        }
        return false;
    }

    public void PickUpCandy(CandyManager.CandyTypes type) {

        switch (type) {
            case CandyManager.CandyTypes.TeleportCandy:
                teleportCandyCount++;
                break;
            case CandyManager.CandyTypes.BombCandy:
                bombCandyCount++;
                break;
        }


        totalCandyCount++;
        SetCandyCount();
    }

    private void SetCandyCount() {
        totalCandyCounterNumber.SetNumber(totalCandyCount);
        teleportCandyNumber.SetNumber(teleportCandyCount);
        bombCandyNumber.SetNumber(bombCandyCount);
    }

    public void ClearPlayerStats() {
        totalCandyCount = 0;
        teleportCandyCount = 0;
        bombCandyCount = 0;
        totalScore = 0;
    }

    public void StartGame() {
        ClearPlayerStats();
        SetCandyCount();
    }
}
