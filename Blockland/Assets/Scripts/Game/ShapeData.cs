using UnityEngine;

[CreateAssetMenu(fileName = "New Shape", menuName = "Shape")]
public class ShapeData : ScriptableObject
{
    [System.Serializable]
    public class Sequence
    {
        public bool[] items;

        public Sequence() { }

        public Sequence(int size)
        {
            items = new bool[size];
            ClearRow();

        }

        public void ClearRow()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = false;
            }
        }
    }

    public int columns = 0;
    public int rows = 0;

    public Sequence[] board;

    public void Clear()
    {
        for(int i = 0; i < rows; i++)
        {
            board[i].ClearRow();
        }
    }

    public void CreateNewBoard()
    {
        board = new Sequence[rows];

        for (int i = 0; i < rows; i++)
        {
            board[i] = new Sequence(columns);
        }
    }
}