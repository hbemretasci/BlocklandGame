using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject piece;
    [HideInInspector] public int pieceAmount;

    private const float cellOffset = 2f;

    private Vector3 shapeStoreScaleSize = new Vector3(.6f, .6f, .6f);
    private Vector3 pieceStoreScaleSize = Vector3.one;

    private Vector3 shapeDragScaleSize = Vector3.one;
    private Vector3 pieceDragScaleSize = new Vector3(.85f, .85f, .85f);

    private Vector2 dragOffset = new Vector2(0, 400f);

    private RectTransform shapeAreaRectTransform;
    private RectTransform shapeRectTransform;
    private Vector3 shapePositionAtStore;

    private float cellWidth;
    private float cellHeight;
    private float offsetX;
    private float offsetY;

    private bool needUpdate = false;

    private GameManager gm;

    public ShapeData data;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        InitializeShape();
    }

    private void Update()
    {
        if (CompareTag("Game") && needUpdate) GameEvents.UpdateTable();
    }

    private void InitializeShape()
    {
        shapeRectTransform = GetComponent<RectTransform>();
        shapeAreaRectTransform = GameObject.Find("ShapeStoreArea").GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GameEvents.MoveShapeToStore += MoveShapeToStore;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStore -= MoveShapeToStore;        
    }

    public void CreateShape(ShapeData shapeData)
    {
        GameObject tempGameObject;
        
        int colCount = 0;
        int rowCount = 0;

        data = shapeData;
        pieceAmount = 0;

        cellHeight = GetSquareImageHeight();
        cellWidth = GetSquareImageWidth();

        for (int row = 0; row < shapeData.rows; row++)
            if ((shapeData.board[row].items[0]) || (shapeData.board[row].items[1]) || (shapeData.board[row].items[2])) rowCount++;

        for (int col = 0; col < shapeData.columns; col++)
            if ((shapeData.board[0].items[col]) || (shapeData.board[1].items[col]) || (shapeData.board[2].items[col])) colCount++;

        offsetX = ((cellWidth + cellOffset) * (colCount - 1) * .5f);
        offsetY = ((cellHeight + cellOffset) * (rowCount -1 ) * .5f);

        for (int row = 0; row < shapeData.rows; row++)
        {
            for (int col = 0; col < shapeData.columns; col++)
            {
                if (shapeData.board[row].items[col])
                {
                    pieceAmount++;
                    tempGameObject = Instantiate(piece, transform);
                    tempGameObject.tag = gameObject.name;
                    tempGameObject.gameObject.transform.position = Vector3.zero;
                    tempGameObject.GetComponent<RectTransform>().localPosition = GetPositionForShape(row, col);
                }
            }
        }
        shapeRectTransform.localScale = shapeStoreScaleSize;
        shapePositionAtStore = shapeRectTransform.localPosition;
    }

    public void DeactiveShape()
    {
        Piece[] pieceList = gameObject.GetComponentsInChildren<Piece>();
        for (int i = 0; i < pieceList.Length; i++)
            Destroy(pieceList[i].gameObject);
    }

    private Vector2 GetPositionForShape(int currentRow, int currentCol)
    {
        float cellPositionX = ((cellWidth + cellOffset) * currentCol) - offsetX;
        float cellPositionY = ((cellHeight + cellOffset) * -currentRow) + offsetY;

        return new Vector2(cellPositionX, cellPositionY);
    }

    private float GetSquareImageWidth()
    {
        RectTransform imageRect = piece.GetComponent<RectTransform>();
        return imageRect.rect.width;
    }

    private float GetSquareImageHeight()
    {
        RectTransform imageRect = piece.GetComponent<RectTransform>();
        return imageRect.rect.height;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gm.isPaused) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(shapeAreaRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 position);
        SetShapeForDrag(position);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gm.isPaused) return;

        needUpdate = true;
        tag = "Game";
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (gm.isPaused) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(shapeAreaRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 position);
        shapeRectTransform.localPosition = position + dragOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (gm.isPaused) return;

        MoveShapeToStore();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (gm.isPaused) return;

        needUpdate = false;
        GameEvents.GameCycleProgress();
    }

    private void SetShapeForDrag(Vector2 position)
    {
        shapeRectTransform.localPosition = position + dragOffset;
        shapeRectTransform.localScale = shapeDragScaleSize;

        Piece[] pieceList = gameObject.GetComponentsInChildren<Piece>();
        for (int i = 0; i < pieceList.Length; i++)
            pieceList[i].storeImage.rectTransform.localScale = pieceDragScaleSize; 
    }

    private void MoveShapeToStore()
    {
        shapeRectTransform.localPosition = shapePositionAtStore;
        shapeRectTransform.localScale = shapeStoreScaleSize;

        if (gm.soundStatus) AudioManager.Instance.PlaySound("Tick");

        Piece[] pieceList = gameObject.GetComponentsInChildren<Piece>();
        for (int i = 0; i < pieceList.Length; i++)
            pieceList[i].storeImage.rectTransform.localScale = pieceStoreScaleSize;
    }
}
