using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isGameOver;
    [SerializeField] private GameObject board33;
    [SerializeField] private GameObject board34;

    [SerializeField] private GameObject completedParticle;

    [SerializeField] private GameObject completedPanel;
    [SerializeField] private GameObject failedPanel;
    [SerializeField] private TileBoard board;
    [SerializeField] private CanvasGroup gameOver;
    [SerializeField] private int time;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] listRestartButtons;
    [SerializeField] private Button[] listHomeButtons;
    [SerializeField] private Transform[] listPositionsCompletedEffect;

    private void Start()
    {
        foreach(Button button in listRestartButtons)
        {
            button.onClick.AddListener(NewGame);
        }
        foreach(Button button in listHomeButtons)
        {
            button.onClick.AddListener(BackToHome);
        }
        nextButton.onClick.AddListener(NextGame);

        NewGame();
    }

    IEnumerator CountDown()
    {
        time = 45;
        timeText.text = "00 : " + time.ToString("00");
        isGameOver = false;

        while(!isGameOver)
        {
            yield return new WaitForSeconds(1);
            time--;
            if(time <= 0) 
            {
                time = 0;
                isGameOver = true;
                GameOver(false);
            }
            timeText.text = "00 : " + time.ToString("00");
        }
    }

    public void NewGame()
    {
        gameOver.alpha = 0f;
        gameOver.interactable = false;
        completedPanel.SetActive(false);
        failedPanel.SetActive(false);
        nextButton.interactable = SaveLoadData.Instance.level < SaveLoadData.Instance.listLevels.listLevelDetails.Count - 1;
        SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].isCompleted = false;

        if(SaveLoadData.Instance.level > 2)
        {
            board33.SetActive(false);
            board34.SetActive(true);

            board = board34.GetComponent<TileBoard>();
        }
        else
        {
            board33.SetActive(true);
            board34.SetActive(false);

            board = board33.GetComponent<TileBoard>();
        }

        board.ClearBoard();
        if(SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].candies.Count > 0)
        {
            board.CreateCandies(SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].candies);
        }
        board.CreateCakes(SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].cakes);
        board.CreateBox(SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].box);
        board.enabled = true;

        StartCoroutine(CountDown());
    }

    public void NextGame()
    {
        if(SaveLoadData.Instance.level < SaveLoadData.Instance.listLevels.listLevelDetails.Count - 1)
        {
            SaveLoadData.Instance.level++;
        }
        NewGame();
    }

    public void BackToHome()
    {
        if(!isGameOver) SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].isCompleted = false;
        SceneManager.LoadScene("SelectScene");
    }

    public void GameOver(bool completed)
    {
        board.enabled = false;
        gameOver.interactable = true;

        if(completed)
        {
            completedPanel.SetActive(true);
            completedPanel.transform.DOShakeScale(1.5f, 0.5f, 6, 0, true);
            SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].isCompleted = true;
            if(SaveLoadData.Instance.level < SaveLoadData.Instance.listLevels.listLevelDetails.Count - 1)
            {
                SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level + 1].isLock = false;
            }
        }
        else 
        {
            failedPanel.SetActive(true);
            failedPanel.transform.DOShakeScale(1.5f, 0.5f, 6, 0, true);
            SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].isCompleted = false;
        }

        SaveLoadData.Instance.SaveData();

        StartCoroutine(Fade(gameOver, 1f, 1f, completed));
    }

    private IEnumerator Effect(Transform transform)
    {
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = -1;
        GameObject newEffect = Instantiate (completedParticle, worldPosition, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Destroy(newEffect);
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f, bool completed = false)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;

        if(completed) 
        {
            foreach(Transform transform in listPositionsCompletedEffect)
            {
                StartCoroutine(Effect(transform));
            }
            SoundManager.Instance.PlaySound(SoundManager.Instance.completedSound);
        }
        else SoundManager.Instance.PlaySound(SoundManager.Instance.failedSound);
    }
}
