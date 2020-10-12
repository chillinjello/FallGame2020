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
    [SerializeField]
    Number crossCandyNumber;
    private int crossCandyCount = 0;
    [SerializeField]
    Number stunLipsNumber;
    private int stunLipsCount = 0;
    [SerializeField]
    Number duplicateTootsieNumber;
    private int duplicateTootsieCount = 0;
    [SerializeField]
    Number increaseAttackCandyNumber;
    private int increaseAttackCandyCount = 0;
    [SerializeField]
    Number batWingsNumber;
    private int batWingsCount = 0;
    [SerializeField]
    Number vampireCandyNumber;
    private int vampireCandyCount = 0;
    [SerializeField]
    Number teleportEnemiesCandyNumber;
    private int teleportEnemiesCandyCount = 0;
    [SerializeField]
    Number turnIntoCandyNumber;
    private int turnIntoCandyCount = 0;



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
            if (crossCandyCount <= 0) return false;

            Managers._candy.CrossCandyEffect();

            crossCandyCount--;
            SetCandyCount();
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            if (stunLipsCount <= 0) return false;

            Managers._candy.StunLipsEffect();

            stunLipsCount--;
            SetCandyCount();
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            if (duplicateTootsieCount <= 0) return false;

            duplicateTootsieCount--;
            Managers._candy.DuplicateTootsieEffect();

            SetCandyCount();
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            if (increaseAttackCandyCount <= 0) return false;

            Managers._candy.IncreaseAttackCandyEffect();

            increaseAttackCandyCount--;
            SetCandyCount();
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            if (batWingsCount <= 0) return false;

            Managers._candy.BatWingsEffect();

            batWingsCount--;
            SetCandyCount();
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            if (vampireCandyCount <= 0) return false;

            Managers._candy.VampireCandyEffect();

            vampireCandyCount--;
            SetCandyCount();
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            if (teleportEnemiesCandyCount <= 0) return false;

            Managers._candy.TeleportEnemiesCandyEffect();

            teleportEnemiesCandyCount--;
            SetCandyCount();
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0)) {
            if (turnIntoCandyCount <= 0) return false;

            Managers._candy.TurnIntoCandyEffect();

            turnIntoCandyCount--;
            SetCandyCount();
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
            case CandyManager.CandyTypes.StunLips:
                stunLipsCount++;
                break;
            case CandyManager.CandyTypes.CrossCandy:
                crossCandyCount++;
                break;
            case CandyManager.CandyTypes.DuplicateToosie:
                duplicateTootsieCount++;
                break;
            case CandyManager.CandyTypes.IncreasedAttackCandy:
                increaseAttackCandyCount++;
                break;
            case CandyManager.CandyTypes.BatWings:
                batWingsCount++;
                break;
            case CandyManager.CandyTypes.VampireCandy:
                vampireCandyCount++;
                break;
            case CandyManager.CandyTypes.TeleportEnemiesCandy:
                teleportEnemiesCandyCount++;
                break;
            case CandyManager.CandyTypes.TurnIntoCandy:
                turnIntoCandyCount++;
                break;
        }


        totalCandyCount++;
        SetCandyCount();
    }

    public List<CandyManager.CandyTypes> GetCurrentCandyTypes() {
        List<CandyManager.CandyTypes> candyTypes = new List<CandyManager.CandyTypes>();

        if (teleportCandyCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.TeleportCandy);
        }
        if (bombCandyCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.BombCandy);
        }
        if (stunLipsCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.StunLips);
        }
        if (crossCandyCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.CrossCandy);
        }
        if (duplicateTootsieCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.DuplicateToosie);
        }
        if (increaseAttackCandyCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.IncreasedAttackCandy);
        }
        if (batWingsCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.BatWings);
        }
        if (vampireCandyCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.VampireCandy);
        }
        if (teleportEnemiesCandyCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.TeleportEnemiesCandy);
        }
        if (turnIntoCandyCount > 0) {
            candyTypes.Add(CandyManager.CandyTypes.TurnIntoCandy);
        }

        return candyTypes;
    }

    private void SetCandyCount() {
        totalCandyCounterNumber.SetNumber(totalCandyCount);
        teleportCandyNumber.SetNumber(teleportCandyCount);
        bombCandyNumber.SetNumber(bombCandyCount);
        crossCandyNumber.SetNumber(crossCandyCount);
        stunLipsNumber.SetNumber(stunLipsCount);
        duplicateTootsieNumber.SetNumber(duplicateTootsieCount);
        increaseAttackCandyNumber.SetNumber(increaseAttackCandyCount);
        batWingsNumber.SetNumber(batWingsCount);
        vampireCandyNumber.SetNumber(vampireCandyCount);
        teleportEnemiesCandyNumber.SetNumber(teleportEnemiesCandyCount);
        turnIntoCandyNumber.SetNumber(turnIntoCandyCount);
    }

    public void ClearPlayerStats() {
        totalCandyCount = 0;
        teleportCandyCount = 0;
        bombCandyCount = 0;
        totalScore = 0;
        crossCandyCount = 0;
        stunLipsCount = 0;
        duplicateTootsieCount = 0;
        increaseAttackCandyCount = 0;
        batWingsCount = 0;
        vampireCandyCount = 0;
        teleportEnemiesCandyCount = 0;
        turnIntoCandyCount = 0;
    }

    public void StartGame() {
        ClearPlayerStats();
        SetCandyCount();
    }
}
