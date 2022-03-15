using UnityEngine;
using UnityEngine.UI;

public class GameOverBG : MonoBehaviour
{
    [SerializeField] private RectTransform gameOverPopUp;
    [SerializeField] private Text textScoreValue;
    [SerializeField] private Text textBestScoreValue;

    private Table tableScript;

    private void OnFadeOutComplete()
    {        
        gameObject.SetActive(false);
    }

    private void OnFadeInComplete()
    {
        tableScript = GameObject.Find("Table").GetComponent<Table>();
        textBestScoreValue.text = GameDataPrefs.GetBestScore().ToString();
        textScoreValue.text = tableScript.gameScore.ToString();
        gameOverPopUp.gameObject.SetActive(true);
    }

}