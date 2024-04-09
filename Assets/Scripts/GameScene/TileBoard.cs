using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public int count;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Tile candyPrefab;
    [SerializeField] private Tile cakePrefab;
    [SerializeField] private Tile boxPrefab;

    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting;
    private bool isTouch;
    private int cakeCount;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(count);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells) {
            cell.tile = null;
        }

        foreach (var tile in tiles) {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateCandies(List<Vector2Int> candyCoordinates)
    {
        foreach(Vector2Int coordinate in candyCoordinates)
        {
            Tile candy = Instantiate(candyPrefab, grid.transform);
            candy.Spawn(grid.GetCell(coordinate));
            tiles.Add(candy);
            candy.isCandy = true;
        }
    }

    public void CreateCakes(List<Vector2Int> cakeCoordinates)
    {
        cakeCount = cakeCoordinates.Count;
        foreach(Vector2Int coordinate in cakeCoordinates)
        {
            Tile cake = Instantiate(cakePrefab, grid.transform);
            cake.Spawn(grid.GetCell(coordinate));
            tiles.Add(cake);
            cake.isCake = true;
        }
    }

    public void CreateBox(Vector2Int boxCoordinates)
    {
        Tile box = Instantiate(boxPrefab, grid.transform);
        box.Spawn(grid.GetCell(boxCoordinates));
        tiles.Add(box);
        box.isBox = true;
    }

    private void Update()
    {
        if (!waiting)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    isTouch = true;
                }
                if (touch.phase == TouchPhase.Moved && isTouch)
                {
                    if (touch.deltaPosition.y > 5 && touch.deltaPosition.y > touch.deltaPosition.x) 
                    {
                        Move(Vector2Int.up, 0, 1, 1, 1);
                    } 
                    else if (touch.deltaPosition.x < -5 && touch.deltaPosition.y > touch.deltaPosition.x) 
                    {
                        Move(Vector2Int.left, 1, 1, 0, 1);
                    } 
                    else if (touch.deltaPosition.y < -5  && touch.deltaPosition.y < touch.deltaPosition.x) 
                    {
                        Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
                    } 
                    else if (touch.deltaPosition.x > 5 && touch.deltaPosition.y < touch.deltaPosition.x) 
                    {
                        Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isTouch = false;
                }
            }
        }
    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        isTouch = false;
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied && !cell.tile.isCandy) 
                {
                    changed = MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed) 
        {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return (a.cell.coordinates.x == b.cell.coordinates.x && a.cell.coordinates.y < b.cell.coordinates.y && a.isCake && b.isBox) ||
        (a.cell.coordinates.x == b.cell.coordinates.x && a.cell.coordinates.y > b.cell.coordinates.y && a.isBox && b.isCake);
    }

    private void MergeTiles(Tile a, Tile b)
    {
        if(a.isCake && b.isBox) tiles.Remove(a);
        else tiles.Remove(b);
        
        a.Merge(b.cell);
        cakeCount--;

        SoundManager.Instance.PlaySound(SoundManager.Instance.mergeSound);

        if(cakeCount == 0) gameManager.isGameOver = true;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        if (gameManager.isGameOver) 
        {
            gameManager.GameOver(true);
        }
    }
}
