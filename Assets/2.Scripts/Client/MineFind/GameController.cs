using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public int width, height, mineCount;
    public GameObject tilePrefab;
    public Tile[,] tiles;

    public TextMeshProUGUI gameResultText;
    public GameObject gameOverPanel;

    private int minesLeft;
    private List<Vector2Int> minePositions;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GenerateTiles();
        PlaceMines();
        SetAdjacentMineCount();
    }

    private void GenerateTiles()
    {
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileObj = Instantiate(tilePrefab, transform);
                Tile tile = tileObj.GetComponent<Tile>();
                tile.x = x;
                tile.y = y;
                tiles[x, y] = tile;
            }
        }
    }

    private void PlaceMines()
    {
        minePositions = new List<Vector2Int>();
        int minesPlaced = 0;
        while (minesPlaced < mineCount)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            Vector2Int position = new Vector2Int(x, y);
            if (!minePositions.Contains(position))
            {
                minePositions.Add(position);
                minesPlaced++;
            }
        }

        foreach (Vector2Int position in minePositions)
        {
            tiles[position.x, position.y].isMine = true;
        }
    }

    private void SetAdjacentMineCount()
    {
        foreach (Tile tile in tiles)
        {
            if (!tile.isMine)
            {
                tile.adjacentMines = GetAdjacentMines(tile.x, tile.y);
            }
        }
    }

    private int GetAdjacentMines(int x, int y)
    {
        int count = 0;
        for (int xi = x - 1; xi <= x + 1; xi++)
        {
            for (int yi = y - 1; yi <= y + 1; yi++)
            {
                if (xi >= 0 && xi < width && yi >= 0 && yi < height && (xi != x || yi != y))
                {
                    if (tiles[xi, yi].isMine)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    public void CheckWin()
    {
        foreach (Tile tile in tiles)
        {
            if (tile.state != TileState.Revealed && !tile.isMine)
            {
                return;
            }
        }
        GameOver(true);
    }

    public void GameOver(bool isWin)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.isMine || tile.state.Equals(TileState.Hidden))
            {
                tile.Reveal();
            }
        }

        gameOverPanel.SetActive(true);
        if (isWin)
        {
            gameResultText.text = "You Win!";
        }
        else
        {
            gameResultText.text = "You Lose!";
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("6.MineFind");
    }
}