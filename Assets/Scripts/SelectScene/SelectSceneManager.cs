using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSceneManager : MonoBehaviour
{
    [SerializeField] private Transform listLevelsPanel;
    [SerializeField] private GameObject itemSelectPrefabs;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        for(int i=0; i<SaveLoadData.Instance.listLevels.listLevelDetails.Count; i++)
        {
            GameObject item = Instantiate(itemSelectPrefabs, listLevelsPanel);
            ItemSelect itemSelect = item.GetComponent<ItemSelect>();
            itemSelect.InitData(SaveLoadData.Instance.listLevels.listLevelDetails[i]);
        }

        backButton.onClick.AddListener(() => SceneManager.LoadScene("TopScene"));
    }
}
