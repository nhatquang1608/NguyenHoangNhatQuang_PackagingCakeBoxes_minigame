using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Levels")]
public class ListLevels : ScriptableObject
{
    [System.Serializable]
    public class LevelDetails
    {
        public int levelId;
        public List<Vector2Int> candies = new List<Vector2Int>();
        public List<Vector2Int> cakes = new List<Vector2Int>();
        public Vector2Int box;
        public bool isLock;
        public bool isCompleted;
    }

    public List<LevelDetails> listLevelDetails = new List<LevelDetails>();
}
