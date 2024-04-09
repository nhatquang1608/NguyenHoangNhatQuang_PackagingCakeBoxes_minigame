using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemSelect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject completedStars;
    [SerializeField] private GameObject failedStars;
    [SerializeField] private Button selectButton;

    public void InitData(ListLevels.LevelDetails levelDetails)
    {
        if(levelDetails.isLock)
        {
            levelText.gameObject.SetActive(false);
            lockImage.SetActive(true);
            completedStars.SetActive(false);
            failedStars.SetActive(true);
        }
        else
        {
            levelText.gameObject.SetActive(true);
            lockImage.SetActive(false);

            if(levelDetails.isCompleted)
            {
                completedStars.SetActive(true);
                failedStars.SetActive(false);
            }
            else
            {
                completedStars.SetActive(false);
                failedStars.SetActive(true);
            }

            levelText.text = (levelDetails.levelId + 1).ToString();
        }

        if(!levelDetails.isLock) selectButton.onClick.AddListener(() => OnSelect(levelDetails.levelId));
    }

    private void OnSelect(int levelId)
    {
        SaveLoadData.Instance.level = levelId;
        SceneManager.LoadScene("GameScene");
    }
}
