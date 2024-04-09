using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates;
    public Tile tile;

    public bool Empty => tile == null;
    public bool Occupied => tile != null;
}
