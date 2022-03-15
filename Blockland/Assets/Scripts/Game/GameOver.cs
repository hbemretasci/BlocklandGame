using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private GameManager gm;

    [SerializeField] private RectTransform gameOverBG;
    [SerializeField] private Animator gameOverBGAnimator;

    private void OnEnable()
    {
        GameEvents.GameFinish += GameFinish;
    }

    private void OnDisable()
    {
        GameEvents.GameFinish -= GameFinish;
    }

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameOverBGAnimator.SetTrigger("FadeOut");
    }

    private void GameFinish()
    {
        gm.isGameOver = true;
        gameOverBG.gameObject.SetActive(true);
        gameOverBGAnimator.SetTrigger("FadeIn");

        if (gm.musicStatus)
        {
            AudioManager.Instance.Stop("Game");
            AudioManager.Instance.Play("Finish");
        }

        if (gm.soundStatus) AudioManager.Instance.PlaySound("Finish");
    }

    public void Replay()
    {
        if (gm.soundStatus) AudioManager.Instance.PlaySound("Button");
        SceneManager.LoadScene(1);
    }

    public void Home()
    {
        if (gm.soundStatus) AudioManager.Instance.PlaySound("Button");
        SceneManager.LoadScene(0);
    }

}