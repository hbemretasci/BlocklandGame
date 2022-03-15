using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataCreator : Editor
{
    private ShapeData ShapeDataInstance => target as ShapeData;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GetBoardsFields();
        EditorGUILayout.Space();
        if (ShapeDataInstance.board != null && ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0) DrawBoardTable();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(ShapeDataInstance);
    }

    private void GetBoardsFields()
    {
        int tempColumnValue = ShapeDataInstance.columns;
        int tempRowValue = ShapeDataInstance.rows;

        ShapeDataInstance.columns = EditorGUILayout.IntField("Columns", ShapeDataInstance.columns);
        ShapeDataInstance.rows = EditorGUILayout.IntField("Rows", ShapeDataInstance.rows);

        if ((ShapeDataInstance.columns != tempColumnValue || ShapeDataInstance.rows != tempRowValue) &&
            (ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)) ShapeDataInstance.CreateNewBoard();
    }

    private void DrawBoardTable()
    {
        GUIStyle tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        GUIStyle columnStyle= new GUIStyle();
        columnStyle.fixedWidth = 65;
        columnStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

        for (int row = 0; row < ShapeDataInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(columnStyle);

            for (int column = 0; column < ShapeDataInstance.columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle);
                bool data = EditorGUILayout.Toggle(ShapeDataInstance.board[row].items[column], dataFieldStyle);
                ShapeDataInstance.board[row].items[column] = data;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}