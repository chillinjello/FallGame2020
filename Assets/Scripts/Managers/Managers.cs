using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurnManager))]
[RequireComponent(typeof(BoardManager))]
[RequireComponent(typeof(EnemyManager))]
[RequireComponent(typeof(PlayerStatusManager))]
public class Managers : MonoBehaviour {
    public static TurnManager _turn { get; private set; }
    public static BoardManager _board { get; private set; }
    public static EnemyManager _enemy { get; private set; }
    public static PlayerStatusManager _player { get; private set; }

    private List<IGameManager> _startSequence;

    private void Awake() {
        _turn = GetComponent<TurnManager>();
        _board = GetComponent<BoardManager>();
        _enemy = GetComponent<EnemyManager>();
        _player = GetComponent<PlayerStatusManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(_turn);
        _startSequence.Add(_board); 
        _startSequence.Add(_enemy); 
        _startSequence.Add(_player); 

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers() {
        foreach (IGameManager manager in _startSequence) {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules) {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence) {
                if (manager.status == ManagerStatus.Started) {
                    numReady++;
                }
            }

            if (numReady > lastReady) {
                Debug.Log("Progress: " + numReady + "/" + numModules);
            }

            yield return null;
        }

        Debug.Log("All managers started up");
    }
}
