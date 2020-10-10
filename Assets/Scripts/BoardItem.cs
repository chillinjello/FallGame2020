using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardItem : MonoBehaviour {
    public enum MoveDirections {
        up,
        down,
        left,
        right,
        none
    }

    public int xPos = -1;
    public int yPos = -1;
    public Vector2 GetPos() {
        return new Vector2(xPos, yPos);
    }
    
    protected List<Character> GetAdjacentCharacters() {
        List<Character> adjacentCharacters = new List<Character>();
        adjacentCharacters.AddRange(Managers._enemy.Enemies);
        adjacentCharacters.Add(Managers._turn.Player);

        adjacentCharacters = adjacentCharacters.FindAll(c => {
            if (c.xPos == xPos) {
                if (c.yPos == yPos + 1 || c.yPos == yPos - 1)
                    return true;
            }
            if (c.yPos == yPos) {
                if (c.xPos == xPos + 1 || c.xPos == xPos - 1)
                    return true;
            }
            return false;
        });

        return adjacentCharacters;
    }
    protected List<BoardItem> GetAdjacentBoardItems(bool includePlayer = true, bool includeWalls = true) {
        List<BoardItem> adjacentBoardItems = new List<BoardItem>();
        adjacentBoardItems.AddRange(Managers._enemy.Enemies);
        if (includePlayer) adjacentBoardItems.Add(Managers._turn.Player);
        if (includeWalls) adjacentBoardItems.AddRange(Managers._enemy.Walls);

        adjacentBoardItems = adjacentBoardItems.FindAll(c => {
            if (c.xPos == xPos) {
                if (c.yPos == yPos + 1 || c.yPos == yPos - 1)
                    return true;
            }
            if (c.yPos == yPos) {
                if (c.xPos == xPos + 1 || c.xPos == xPos - 1)
                    return true;
            }
            return false;
        });

        return adjacentBoardItems;
    }
    public static List<BoardItem> GetAdjacentBoardItems(int x, int y, bool includePlayer = true) {
        List<BoardItem> adjacentBoardItems = new List<BoardItem>();
        adjacentBoardItems.AddRange(Managers._enemy.Enemies);
        if (includePlayer) adjacentBoardItems.Add(Managers._turn.Player);
        adjacentBoardItems.AddRange(Managers._enemy.Walls);

        adjacentBoardItems = adjacentBoardItems.FindAll(c => {
            if (c.xPos == x) {
                if (c.yPos == y + 1 || c.yPos == y - 1)
                    return true;
            }
            if (c.yPos == y) {
                if (c.xPos == x + 1 || c.xPos == x - 1)
                    return true;
            }
            return false;
        });

        return adjacentBoardItems;
    }
    protected List<Vector2> GetAdjacentEmptySpaces(bool includePlayer = true, bool includeWalls = true) {
        List<Vector2> emptySpace = new List<Vector2>();

        emptySpace.Add(new Vector2(xPos + 1, yPos));
        emptySpace.Add(new Vector2(xPos - 1, yPos));
        emptySpace.Add(new Vector2(xPos, yPos + 1));
        emptySpace.Add(new Vector2(xPos, yPos - 1));

        var characterSpaces = GetAdjacentBoardItems(includePlayer, includeWalls);
        emptySpace = emptySpace.FindAll(e => {
            return -1 == characterSpaces.FindIndex(c => e.x == c.xPos && e.y == c.yPos) && BoardManager.CheckValidCoord((int)e.x, (int)e.y);
        });

        return emptySpace;
    }
    public static List<Vector2> GetAdjacentEmptySpaces(int x, int y, bool includePlayer = true) {
        List<Vector2> emptySpace = new List<Vector2>();

        emptySpace.Add(new Vector2(x + 1, y));
        emptySpace.Add(new Vector2(x - 1, y));
        emptySpace.Add(new Vector2(x, y + 1));
        emptySpace.Add(new Vector2(x, y - 1));

        var characterSpaces = GetAdjacentBoardItems(x,y, includePlayer);
        emptySpace = emptySpace.FindAll(e => {
            return -1 == characterSpaces.FindIndex(c => e.x == c.xPos && e.y == c.yPos) && BoardManager.CheckValidCoord((int)e.x, (int)e.y);
        });

        return emptySpace;
    }

    static public List<BoardItem> GetAllBoardItems(bool candies = true) {
        List<BoardItem> boardItems = new List<BoardItem>();
        boardItems.AddRange(Managers._enemy.Enemies);
        boardItems.Add(Managers._turn.Player);
        boardItems.AddRange(Managers._enemy.Walls);
        if (candies) boardItems.AddRange(Managers._candy.Candies);

        return boardItems;
    }

    static public List<Vector2> FindEmptySpaces(bool candies = true) {
        List<Vector2> emptySpaces = new List<Vector2>();
        var boardItems = GetAllBoardItems(candies);

        for (int x = 0; x < 6; x++) {
            for (int y = 0; y < 6; y++) {
                if (boardItems.Find(i => i.xPos == x && i.yPos == y) == null) {
                    emptySpaces.Add(new Vector2(x, y));
                }
            }
        }
        emptySpaces = emptySpaces.OrderBy(w => Random.value).ToList();
        return emptySpaces;
    }

    static public Vector2? FindEmptySpot() {
        var emptySpaces = FindEmptySpaces();

        if (emptySpaces.Count <= 0) return null;
        else return emptySpaces[0];
    }

    protected MoveDirections GetDirectionFromCoords(int xi, int yi, int xf, int yf) {
        if (xi == xf) {
            if (yi == yf + 1) {
                return MoveDirections.up;
            }
            if (yi == yf - 1) {
                return MoveDirections.down;
            }
        }
        if (yi == yf) {
            if (xi == xf + 1) {
                return MoveDirections.left;
            }
            if (xi == xf - 1) {
                return MoveDirections.right;
            }
        }
        return MoveDirections.none;
    }

    public int ManhattanDistance(int x, int y) {
        return Mathf.Abs(xPos - x) + Mathf.Abs(yPos - y);
    }
}
