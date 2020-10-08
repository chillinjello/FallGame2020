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

    public void PickUpCandy(CandyManager.CandyTypes type) {
    }
}
