using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;

//Custom Editor class
//This allows for various extensions to the editor to allow for a nicer tool (using it primarily for buttons)
[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MapManager manager = (MapManager)target;

        GUILayout.Label("Tiles Used: " + manager.TileCount());                                              //Output the amount of tiles currently in use (to make sure nobody goes over the limit)

        manager.maxTiles = EditorGUILayout.IntField("Max Tiles: ", manager.maxTiles);                       //A field for the user to input the max tiles value
        manager.filename = EditorGUILayout.TextField("Filename: ", manager.filename);                       //A field for the user to input the filename

        //Create serialized properties for objects that don't get handled automatically by the unity editor
        SerializedProperty tilemap = serializedObject.FindProperty("tilemap");
        SerializedProperty sprites = serializedObject.FindProperty("assets");

        //Check if the data has been changed, if so update it
        EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(tilemap, false);
            EditorGUILayout.PropertyField(sprites, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        //Button to call the export map function
        if (GUILayout.Button("Export Map"))
        {
            manager.ExportMapToFile();
        }

        //Button to call the import map function
        if (GUILayout.Button("Import Map"))
        {
            manager.ImportMapFromFile();
        }

        //Button to generate the training data file
        if (GUILayout.Button("Generate Training Data"))
        {
            manager.GenerateTrainingData();
        }
    }
}
public class MapManager : MonoBehaviour
{
    public int maxTiles = 1000;
    public string filename = "MapOut";
    public Tilemap tilemap;
    public TileBase[] assets;

    private string path;

    //Export Map to file function
    //Creates a text file and fills it with data in the form (x, y, tiledata)
    public void ExportMapToFile()
    {
        //Necessary to compress the tilemap bounds as it isn't dealt with automatically by unity and this reduces the tilemap to the minimum necessary size
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] tiles = tilemap.GetTilesBlock(bounds);

        path = Application.dataPath + "/Maps/" + filename + ".txt";

        //If the file doesn't exist create it, otherwise we throw an error (this could be dealt with but it's easier to let the user change the filename)
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Map: " + filename + "\n\n");
        }
        else
        {
            Debug.LogError("File " + filename + ".txt already exists. Please provide a new filename.");
        }

        string tileInfo;

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = tiles[x + y * bounds.size.x];
                
                //As long as the tile isn't empty we add it to the file
                if (tile != null)
                {
                    tileInfo = "(" + x + ", " + y + ", " + tile.name + ")\n";
                    File.AppendAllText(path, tileInfo);
                }
            }
        }
    }

    //Import Map function
    //Reads the data from the previously exported txt file and fills a tilemap with the information
    public void ImportMapFromFile()
    {
        path = Application.dataPath + "/Maps/" + filename + ".txt";

        //Throw an error if the file doesn't exist
        if (!File.Exists(path))
        {
            Debug.LogError("File " + filename + ".txt doesn't exist. Please create it or provide another filename.");
        }
        else
        {
            //Using a try catch statement to make sure nothing goes wrong (This may make the previous if statement redundant but it's nice to give a useful output if we can)
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string input = sr.ReadToEnd();
                    string[] lines = input.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);                             //Split every new line into it's own string and remove empty lines
                    int x, y;

                    //Loop through every line of data (we ignore first line as it is the filename)
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] data = lines[i].Split(new[] { ',', '(', ')', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);         //Split the line into their corresponding data segments (x, y, name)

                        //Loop through assets and find the asset with the matching name
                        foreach (TileBase asset in assets)
                        {
                            if (asset.name == data[2])
                            {
                                int.TryParse(data[0], out x);
                                int.TryParse(data[1], out y);
                                tilemap.SetTile(new Vector3Int(x, y, 0), asset);                                                            //Set the tile with the given data
                            }
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    public void GenerateTrainingData()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Maps");
        FileInfo[] info = dir.GetFiles("*.txt");

        path = Application.dataPath + "/TrainingData.txt";

        if (!File.Exists(path))
        {
            foreach (FileInfo f in info)
            {
                try
                {
                    using (StreamReader sr = f.OpenText())
                    {
                        string input = sr.ReadToEnd();
                        string[] lines = input.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 2; i < lines.Length; i++)
                        {
                            File.AppendAllText(path, lines[i]);
                        }
                    }
                }
                catch (IOException e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
        else
        {
            Debug.LogError("Training data file already exists please delete it.");
        }
    }

    //Simple getter for current tile count (used to display in inspector)
    public int TileCount()
    {
        if (tilemap != null)
        {
            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] tiles = tilemap.GetTilesBlock(bounds);

            return tiles.Length;
        } else
        {
            return 0;
        }
    }
}