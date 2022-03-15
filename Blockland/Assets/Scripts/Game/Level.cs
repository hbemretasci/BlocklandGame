using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public GameObject piece;
    public Sprite[] imageList;
    public int currentLevel;
    public int leftToBonus;

    private Sprite newLevelSprite;

    [SerializeField] Image scoreImage;
    [SerializeField] Image bestScoreImage;
    [SerializeField] Image nextLevelImage;

    private GameManager gm;
    private Piece pieceScript;
    private ChangeBackground changeBackgroundScript;

    private void OnEnable()
    {
        GameEvents.SetLevel += SetLevel;
    }

    private void OnDisable()
    {
        GameEvents.SetLevel -= SetLevel;
    }

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pieceScript = piece.GetComponent<Piece>();
        changeBackgroundScript = GameObject.Find("Background").GetComponent<ChangeBackground>();

        currentLevel = 0;
        leftToBonus = 0;
        pieceScript.ChangeStoreImage(imageList[currentLevel]);
    }

    private void SetLevel()
    {
        if (gm.gameLevel != currentLevel)
        {
            currentLevel = gm.gameLevel;
            newLevelSprite = imageList[currentLevel];

            changeBackgroundScript.Change();

            if (gm.soundStatus) AudioManager.Instance.PlaySound("Level");

            pieceScript.ChangeStoreImage(newLevelSprite);

            scoreImage.sprite = newLevelSprite;
            bestScoreImage.sprite = newLevelSprite;

            if (currentLevel != 7) nextLevelImage.sprite = imageList[currentLevel + 1];
            else nextLevelImage.sprite = imageList[0];

            leftToBonus = LeftToBonus();

            Invoke("ChangeEmptyCells", 1f);
            Invoke("ChangeActiveCells", 1f);
        }
    }

    private void ChangeEmptyCells()
    {
        GameObject[] findedCells;
        Cell cellScript;

        //empty cells found
        findedCells = GameObject.FindGameObjectsWithTag("Cell");
        for (int i = 0; i < findedCells.Length; i++)
        {
            cellScript = findedCells[i].GetComponent<Cell>();
            cellScript.busyImage.sprite = newLevelSprite;
            cellScript.activeImage.sprite = newLevelSprite;
            cellScript.destroyImage.sprite = newLevelSprite;
        }
    }

    private void ChangeActiveCells()
    {
        GameObject[] findedCells;
        Cell cellScript;

        //active cells found
        findedCells = GameObject.FindGameObjectsWithTag("Active");
        for (int i = 0; i < findedCells.Length; i++)
        {
            cellScript = findedCells[i].GetComponent<Cell>();
            cellScript.isBonusCell = true;
            cellScript.busyImage.sprite = newLevelSprite;
            cellScript.destroyImage.sprite = newLevelSprite;
        }
    }

    private int LeftToBonus()
    {
        GameObject[] findedCells = GameObject.FindGameObjectsWithTag("Active");
        return findedCells.Length;
    }

}