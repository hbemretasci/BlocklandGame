using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] Sprite[] buttonImages;
    [SerializeField] GameObject soundButton;
    [SerializeField] GameObject musicButton;
    public RectTransform pauseMenu;

    private GameManager gm;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        SetAudioButtons();
        pauseMenu.gameObject.SetActive(false);
    }

    private void SetAudioButtons()
    {
        if (gm.musicStatus) musicButton.GetComponent<Image>().sprite = buttonImages[1];
        else musicButton.GetComponent<Image>().sprite = buttonImages[0];

        if (gm.soundStatus) soundButton.GetComponent<Image>().sprite = buttonImages[3];
        else soundButton.GetComponent<Image>().sprite = buttonImages[2];
    }

    public void Pause()
    {
        gm.isPaused = !gm.isPaused;

        if (gm.isPaused)
        {
            AudioListener.pause = true;
            Time.timeScale = 0;
            pauseMenu.gameObject.SetActive(true);
            GameDataPrefs.SetBestScore(gm.bestScore);
        }
        else
        {
            AudioListener.pause = false;
            Time.timeScale = 1;
            pauseMenu.gameObject.SetActive(false);
        }
    }

    public void Quit()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
        pauseMenu.gameObject.SetActive(false);
        GameEvents.GameFinish();
    }

    public void SwitchAudioButtons(string buttonName)
    {
        if (buttonName == "Music")
        {
            gm.musicStatus = !gm.musicStatus;
            GameDataPrefs.SetMusicStatus(gm.musicStatus);
        }
        if (buttonName == "Sound")
        {
            gm.soundStatus = !gm.soundStatus;
            GameDataPrefs.SetSoundStatus(gm.soundStatus);
        }

        SetAudioButtons();
        InitializeMusic();
    }

    private void InitializeMusic()
    {
        if (gm.musicStatus) AudioManager.Instance.Play("Game");
        else AudioManager.Instance.Stop("Game");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) Pause();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus) Pause();
    }

}
