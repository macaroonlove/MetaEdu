using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum TileState { Hidden, Revealed, Mine, Flag }

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public int x, y;
    public bool isMine;
    public TileState state = TileState.Hidden;
    public Sprite[] sprites;
    public Sprite mineSprite, hiddenSprite;

    private Image sr;
    public int adjacentMines;

    private void Start()
    {
        sr = GetComponent<Image>();
        if (isMine)
        {
            state = TileState.Mine;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (state == TileState.Revealed) return;
            state = state == TileState.Flag ? TileState.Hidden : TileState.Flag;
            sr.sprite = state == TileState.Flag ? sprites[9] : hiddenSprite;
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (state == TileState.Revealed || state == TileState.Flag) return;
            Reveal();
        }
    }

    public void Reveal()
    {
        if (state == TileState.Revealed) return;

        state = TileState.Revealed;

        if (isMine) // 폭탄이 있으면
        {
            sr.sprite = mineSprite;
            GameController.instance.GameOver(false);
        }
        else // 폭탄이 없으면
        {
            if (adjacentMines > 0) // 주위에 폭탄이 있으면
            {
                sr.sprite = sprites[adjacentMines];
            }
            else // 주위에 폭탄이 없으면
            {
                sr.sprite = sprites[0];
                foreach (Tile tile in GetAdjacentTiles())
                {
                    if (tile.state == TileState.Hidden)
                    {
                        tile.Reveal();
                    }
                }
            }
            GameController.instance.CheckWin();
        }
    }

    private List<Tile> GetAdjacentTiles()
    {
        List<Tile> adjacentTiles = new List<Tile>();
        for (int xi = x - 1; xi <= x + 1; xi++)
        {
            for (int yi = y - 1; yi <= y + 1; yi++)
            {
                if (xi == x && yi == y) continue;

                if (xi >= 0 && xi < GameController.instance.width && yi >= 0 && yi < GameController.instance.height)
                {
                    Tile tile = GameController.instance.tiles[xi, yi];

                    // 변경된 부분: Hidden 또는 Flag 상태인 경우에만 추가
                    if (tile.state == TileState.Hidden || tile.state == TileState.Flag) adjacentTiles.Add(tile);
                }
            }
        }

        return adjacentTiles;
    }
}
