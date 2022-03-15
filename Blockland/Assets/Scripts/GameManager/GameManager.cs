using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isPaused;
    public bool isGameOver;
    public bool musicStatus;
    public bool soundStatus;
    public int bestScore;
    public int gameLevel;

    private void OnEnable()
    {
        GameDataPrefs.CreateData();
    }

    private void Awake()
    {
        isPaused = false;
        isGameOver = false;
        bestScore = GameDataPrefs.GetBestScore();
        soundStatus = GameDataPrefs.GetSoundStatus();
        musicStatus = GameDataPrefs.GetMusicStatus();
        gameLevel = 0;
    }

}
