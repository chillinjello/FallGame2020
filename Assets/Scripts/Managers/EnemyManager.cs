using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    [SerializeField]
    GameObject PlaceholderEnemy;

    public List<Enemy> Enemies = new List<Enemy>();

    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Enemy Manager...");


        status = ManagerStatus.Started;
    }

    public bool moving = false;

    private void Awake() {
        GameObject testEnemy = Instantiate(PlaceholderEnemy);
        Enemies.Add(testEnemy.GetComponent<Enemy>());
        testEnemy.GetComponent<Enemy>().MoveCharacter(0, 0);
    }

    private void Update() {
        if (moving) {
            //move enemies and see if any are still moving
            bool stillMoving = false;
            foreach(var enemy in Enemies) {
                stillMoving = enemy.moving;
            }
            moving = stillMoving;
        }
    }

    public void EnemyTurn() {
        foreach (var enemy in Enemies) {
            enemy.EnemyTurn();
        }
        moving = true;
    }
}
