using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MapManager manager = (MapManager)target;

        GUILayout.Label("Tiles Used: " + manager.TileCount());

        manager.maxTiles = EditorGUILayout.IntField("Max Tiles: ", manager.maxTiles);
        manager.filename = EditorGUILayout.TextField("Filename: ", manager.filename);

        SerializedProperty tilemap = serializedObject.FindProperty("tilemap");
        SerializedProperty sprites = serializedObject.FindProperty("assets");

        EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(tilemap, false);
            EditorGUILayout.PropertyField(sprites, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Export Map"))
        {
            manager.ExportMapToFile();
        }

        if(GUILayout.Button("Import Map"))
        { 
            manager.ImportMapFromFile();
        }

        //DrawDefaultInspector();
    }
}
