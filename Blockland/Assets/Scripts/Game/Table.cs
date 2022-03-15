using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Table : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI nextValueText;

    public int gameScore;

    private int levelPoint;
    private int remainToNextLevel;

    private const int columnCount = 8;
    private const int rowCount = 8;
    private const float cellOffset = 2f;
    private const int levelRange = 250;

    private float tableStartPositionX;
    private float tableStartPositionY;

    private float cellWidth;
    private float cellHeight;

    private int checkRowStart;
    private int checkRowEnd;
    private int checkColStart;
    private int checkColEnd;

    private Vector2[,] cellsLocation = new Vector2[rowCount, columnCount];
    private Cell[,] cells = new Cell[rowCount, columnCount];
    
    [SerializeField] GameObject cellPrefab;
    [SerializeField] RectTransform[] bonusTexts;

    private GameManager gm;
    private Level levelScript;
    private BonusArea bonusAreaScript;

    private void OnEnable()
    {
        GameEvents.GameCycleProgress += GameCycleProgress;
        GameEvents.UpdateTable += UpdateTable;
    }

    private void OnDisable()
    {
        GameEvents.GameCycleProgress -= GameCycleProgress;
        GameEvents.UpdateTable -= UpdateTable;
    }

    void Start()
    {
        InitializeData();
        UpdateScoreGUI();
        InitializeTable();
        CreateTable();
        Invoke("InitializeMusic", .5f);
    }

    private void InitializeData()
    {
        gameScore = 0;
        levelPoint = levelRange;
        remainToNextLevel = levelPoint - gameScore;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        levelScript = GameObject.Find("LevelSystem").GetComponent<Level>();
        bonusAreaScript = GameObject.Find("BonusArea").GetComponent<BonusArea>();
    }

    private void UpdateScoreGUI()
    {
        //update score values
        scoreText.text = gameScore.ToString();
        bestText.text = gm.bestScore.ToString();

        //update next level informations
        nextValueText.text = remainToNextLevel.ToString();
    }

    private void InitializeTable()
    {
        GameObject tempCell;
        RectTransform canvasRect;
        RectTransform cellRect;
        float canvasWidth;
        float tableWidth;
        float distance;

        canvasRect = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<RectTransform>();
        canvasWidth = canvasRect.rect.width;

        tempCell = Instantiate(cellPrefab);
        cellRect = tempCell.GetComponent<RectTransform>();
        cellWidth = cellRect.rect.width;
        cellHeight = cellRect.rect.height;
        tableWidth = columnCount * (cellWidth + cellOffset);
        distance = (canvasWidth - tableWidth) * .5f;
        tableStartPositionX = distance + canvasWidth * -.5f;
        tableStartPositionY = 600f;

        Destroy(tempCell);
    }

    private void CreateTable()
    {
        GameObject cell;
        float cellPositionX;
        float cellPositionY;

        cellPositionY = tableStartPositionY;

        for (int row = 0; row < rowCount; row++)
        {
            cellPositionX = tableStartPositionX + cellWidth * .5f;
            for (int col = 0; col < columnCount; col++)
            {
                cell = Instantiate(cellPrefab);
                cell.transform.SetParent(transform);
                cellsLocation[row, col] = transform.TransformPoint(cellPositionX, (cellPositionY + cellHeight * .25f), 0);
                cell.transform.localPosition = new Vector2(cellPositionX, cellPositionY);
                cell.transform.localScale = Vector3.one;
                cells[row, col] = cell.GetComponent<Cell>();
                cells[row, col].cellRow = row;
                cells[row, col].cellCol = col;
                cellPositionX += (cellWidth + cellOffset);
            }
            cellPositionY -= (cellHeight + cellOffset);
        }
    }
    
    private void GameCycleProgress()
    {
        if (PlaceShapeOnTable())
        {
            CheckScore();

            CheckBonusToHide();

            GameEvents.GetNewShapeSet();

            CheckBonusToShow();

            if (CheckGameOver())
            {
                GameDataPrefs.SetBestScore(gm.bestScore);
                GameEvents.GameFinish();
            }
        }
        else GameEvents.MoveShapeToStore();
    }

    private bool PlaceShapeOnTable()
    {
        Shape shapeInGame;
        GameObject[] busyCells;
        Cell cellScript;

        busyCells = GameObject.FindGameObjectsWithTag("Busy");
        shapeInGame = GameObject.FindGameObjectWithTag("Game").GetComponent<Shape>();

        if (shapeInGame.pieceAmount == busyCells.Length)
        {
            shapeInGame.tag = "Over";
            shapeInGame.DeactiveShape();

            if (gm.soundStatus) AudioManager.Instance.PlaySound("Place");

            cellScript = busyCells[0].GetComponent<Cell>();
            checkRowStart = cellScript.cellRow;
            checkRowEnd = cellScript.cellRow;
            checkColStart = cellScript.cellCol;
            checkColEnd = cellScript.cellCol;

            for (int i = 0; i < busyCells.Length; i++)
            {
                cellScript = busyCells[i].GetComponent<Cell>();
                if (cellScript.cellRow < checkRowStart) checkRowStart = cellScript.cellRow;
                if (cellScript.cellRow > checkRowEnd) checkRowEnd = cellScript.cellRow;
                if (cellScript.cellCol < checkColStart) checkColStart = cellScript.cellCol;
                if (cellScript.cellCol > checkColEnd) checkColEnd = cellScript.cellCol;
                cellScript.SetCellActive();
            }

            return true;
        }
        else
        {
            shapeInGame.tag = "Store";
            return false;
        }
    }

    private bool CheckShapeAllocation(int x, int y, ShapeData shapeData)
    {
        for (int shapeRow = 0; shapeRow < shapeData.rows; shapeRow++)
        {
            for (int shapeCol = 0; shapeCol < shapeData.columns; shapeCol++)
            {
                if ((shapeData.board[shapeRow].items[shapeCol]) && (cells[x + shapeRow, y + shapeCol].GetComponent<Cell>().CompareTag("Active"))) return false; 
            }
        }
        return true;
    }

    private bool CheckGameOver()
    {
        GameObject[] shapesInStore;
        bool shapeStatus = false;
        Shape currentShape;

        shapesInStore = GameObject.FindGameObjectsWithTag("Store");
        if (shapesInStore.Length == 0) return false;

        foreach(GameObject shape in shapesInStore)
        {
            shapeStatus = false;
            currentShape = shape.GetComponent<Shape>();
            int horizontalBound = columnCount - currentShape.data.columns + 1;
            int verticalBound = rowCount - currentShape.data.rows + 1;

            for (int tableRow = 0; tableRow < verticalBound; tableRow++)
            {
                for (int tableCol = 0; tableCol < horizontalBound; tableCol++)
                {
                    shapeStatus = CheckShapeAllocation(tableRow, tableCol, currentShape.data);
                    if (shapeStatus) break;
                }
                if (shapeStatus) break;
            }
            if (shapeStatus) break;
        }
        if (shapeStatus) return false; else return true;
    }

    private void CheckScore()
    {
        DestroyCell destroyCellScript;
        bool lineCompletedStatus;
        int effectIndex;

        List<int> completedRows = new List<int>();
        List<int> completedCols = new List<int>();

        destroyCellScript = GameObject.Find("EffectSystem").GetComponent<DestroyCell>();
        effectIndex = levelScript.currentLevel;

        for (int row = checkRowStart; row <= checkRowEnd; row++)
        {
            lineCompletedStatus = true;
            for (int col = 0; col < columnCount; col++)
                if (!cells[row, col].CompareTag("Active"))
                {
                    lineCompletedStatus = false;
                    break;
                }
            if (lineCompletedStatus) completedRows.Add(row);
        }

        for (int col = checkColStart; col <= checkColEnd; col++)
        {
            lineCompletedStatus = true;
            for (int row = 0; row < rowCount; row++)            
                if (!cells[row, col].CompareTag("Active"))
                {
                    lineCompletedStatus = false;
                    break;
                }
                                
            if (lineCompletedStatus) completedCols.Add(col);

        }

        if ((completedRows.Count > 0) || (completedCols.Count > 0))
        {
            ShowScoreMessages(completedRows.Count, completedCols.Count);

            gameScore += GetScore(completedRows.Count, completedCols.Count);
            if (gameScore > gm.bestScore) gm.bestScore = gameScore;
            if (gameScore >= levelPoint)
            {
                gm.gameLevel++;
                if (gm.gameLevel > 7) gm.gameLevel = 0;
                levelPoint += levelPoint;

                GameEvents.EmptyShapeStore();
            }

            remainToNextLevel = levelPoint - gameScore;
            UpdateScoreGUI();
        }

        if ((completedRows.Count > 0) || (completedCols.Count > 0))
        {
            for (int i = 0; i < completedRows.Count; i++)
                ViewDestroyRow(completedRows[i], effectIndex);

            for (int i = 0; i < completedCols.Count; i++)
                ViewDestroyCol(completedCols[i], effectIndex);

            destroyCellScript.StartDestroyLines();

            if (gm.soundStatus) AudioManager.Instance.PlaySound("Destroy");

            for (int i = 0; i < completedRows.Count; i++)
                SetEmptyRow(completedRows[i]);

            for (int i = 0; i < completedCols.Count; i++)
                SetEmptyCol(completedCols[i]);
        }
    }

    private void ShowScoreMessages(int row, int col)
    {
        if ((row > 0) && (col > 0))
        {
            if (gm.soundStatus) AudioManager.Instance.PlaySound("Great");
            Instantiate(bonusTexts[4], new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if ((row > 1) || (col > 1))
        {
            if (gm.soundStatus) AudioManager.Instance.PlaySound("Great");
            Instantiate(bonusTexts[3], new Vector3(0, 0, 0), Quaternion.identity);
        }                       
    }

    private void CheckBonusToHide()
    {
        if (bonusAreaScript.state)
        {
            if (levelScript.leftToBonus == 0)
            {
                bonusAreaScript.HideBonusArea();
                TakeBonus();
            }
            else bonusAreaScript.ShowBonusValue((levelScript.leftToBonus));
        }                 
    }

    private void CheckBonusToShow()
    {
        if (!bonusAreaScript.state)
        {
            if (levelScript.leftToBonus != 0)
            {
                bonusAreaScript.ShowBonusArea();
                bonusAreaScript.ShowBonusValue((levelScript.leftToBonus));
            }
        }
        else bonusAreaScript.ShowBonusValue((levelScript.leftToBonus));
    }

    private void TakeBonus()
    {
        int bonusPoint;
        int bonusLevel = 0;

        if (levelScript.currentLevel <= 2) bonusLevel = 0;
        else if (levelScript.currentLevel <= 4) bonusLevel = 1;
        else bonusLevel = 2;

        switch (bonusLevel)
        {
            case 0:
                bonusPoint = 200;
                break;
                
            case 1:
                bonusPoint = 400;
                break;

            case 2:
                bonusPoint = 1000;
                break;

            default:
                bonusPoint = 0;
                break;
        }

        EffectManager.Instance.Play(8, Vector3.zero);

        if (gm.soundStatus) AudioManager.Instance.PlaySound("Firework");

        Instantiate(bonusTexts[bonusLevel], new Vector3(0, 0, 0), Quaternion.identity);

        gameScore += bonusPoint;
        if (gameScore > gm.bestScore) gm.bestScore = gameScore;
        if (gameScore >= levelPoint)
        {
            gm.gameLevel++;
            if (gm.gameLevel > 7) gm.gameLevel = 0;
            levelPoint += levelPoint;
        }

        remainToNextLevel = levelPoint - gameScore;
        UpdateScoreGUI();
    }

    private void ViewDestroyRow(int rowNumber, int effect)
    {
        for (int col = 0; col < columnCount; col++)
        {
            EffectManager.Instance.Play(effect, cellsLocation[rowNumber, col]);
            cells[rowNumber, col].ViewCellDestroy();
        }
    }

    private void ViewDestroyCol(int colNumber, int effect)
    {
        for (int row = 0; row < rowCount; row++)
        {
            EffectManager.Instance.Play(effect, cellsLocation[row, colNumber]);
            cells[row, colNumber].ViewCellDestroy();
        }

    }

    private void SetEmptyRow(int rowNumber)
    {
        int bonusCell = 0;

        for (int col = 0; col < columnCount; col++)
        {
            if (cells[rowNumber, col].isBonusCell) bonusCell++;
            cells[rowNumber, col].SetCellEmpty();
        }
        levelScript.leftToBonus -= bonusCell;
    }

    private void SetEmptyCol(int colNumber)
    {
        int bonusCell = 0;

        for (int row = 0; row < rowCount; row++)
        {
            if (cells[row, colNumber].isBonusCell) bonusCell++;
            cells[row, colNumber].SetCellEmpty();
        }
        levelScript.leftToBonus -= bonusCell;
    }

    private int GetScore(int row, int col)
    {
        int score;
        int basePoint;

        int bonusPoint = 10;
        int doublePoint = 20;

        basePoint = 5 + levelScript.currentLevel;

        score = (row * 8 * basePoint) + (col * 8 * basePoint);
        score += (row * row * bonusPoint) + (col * col * bonusPoint);
        score += (col * row * doublePoint);

        return score;
    }

    private void InitializeMusic()
    {
        if (gm.musicStatus) AudioManager.Instance.Play("Game");
        else AudioManager.Instance.Stop("Game");
    }

    private void UpdateTable()
    {
        List<int> completedRows = new List<int>();
        List<int> completedCols = new List<int>();
        bool allocStatus = false;
        bool lineStatus;
        int row;
        int col;

        string tagName;

        GameObject[] busyCells;
        GameObject shape;
        Shape shapeInGame;

        busyCells = GameObject.FindGameObjectsWithTag("Busy");
        shape = GameObject.FindGameObjectWithTag("Game");

        if (shape == null)  return;

        shapeInGame = shape.GetComponent<Shape>();

        if (shapeInGame.pieceAmount == busyCells.Length)
        {
            allocStatus = true;

            for (int cellIndex = 0; cellIndex < busyCells.Length; cellIndex++)
            {
                row = busyCells[cellIndex].GetComponent<Cell>().cellRow;
                col = busyCells[cellIndex].GetComponent<Cell>().cellCol;

                lineStatus = true;
                for (int i = 0; i < columnCount; i++)
                {
                    if (cells[row, i].gameObject.CompareTag("Cell"))
                    {
                        lineStatus = false;
                        break;
                    }
                }
                if ((lineStatus) && (!completedRows.Contains(row))) completedRows.Add(row);
                
                //

                lineStatus = true;
                for (int i = 0; i < rowCount; i++)
                {
                    if (cells[i, col].gameObject.CompareTag("Cell"))
                    {
                        lineStatus = false;
                        break;
                    }
                }
                if ((lineStatus) && (!completedCols.Contains(col))) completedCols.Add(col);
            
            }

            for (int i = 0; i < completedRows.Count; i++)
            {
                for (int j = 0; j < columnCount; j++)
                    cells[completedRows[i], j].ViewCellDestroy();
            }

            for (int i = 0; i < completedCols.Count; i++)
            {
                for (int j = 0; j < rowCount; j++)
                    cells[j, completedCols[i]].ViewCellDestroy();
            }
        }

        for (int i = 0; i < rowCount; i++)
        {
            if (!completedRows.Contains(i))
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (!completedCols.Contains(j))
                    {
                        tagName = cells[i, j].tag;

                        switch (tagName)
                        {
                            case "Cell":
                                cells[i, j].ViewCellEmpty();
                                break;

                            case "Active":
                                cells[i, j].ViewCellActive();
                                break;

                            case "Busy":
                                if (allocStatus) cells[i, j].ViewCellBusy();
                                else cells[i, j].ViewCellEmpty();
                                break;

                            default:
                                Debug.Log("internal error");
                                break;
                        }
                    }
                }
            }
        }
    }

}