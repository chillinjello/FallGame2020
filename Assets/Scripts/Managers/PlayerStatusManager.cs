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
    TextMesh PlayerHealthText;

    string[] playerHealthValues = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

    private Player Player { get { return Managers._turn.Player; } }
    
    void Update()
    {
        if (Player.currentHealth < 0) {
            PlayerHealthText.text = playerHealthValues[0];
        }
        else if (Player.currentHealth > 10) {
            PlayerHealthText.text = playerHealthValues[10];
        }
        else {
            PlayerHealthText.text = playerHealthValues[Player.currentHealth];
        }
    }
}
