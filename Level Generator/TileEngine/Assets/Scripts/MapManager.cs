using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int maxTiles = 1000;
    public string filename = "MapOut";
    public Tilemap tilemap;
    public TileBase[] assets;

    private string path;

    public void ExportMapToFile()
    {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] tiles = tilemap.GetTilesBlock(bounds);

        path = Application.dataPath + "/Maps/" + filename + ".txt";

        if(!File.Exists(path))
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
                if (tile != null)
                {
                    tileInfo = "(" + x + ", " + y + ", " + tile.name + ")\n";
                    File.AppendAllText(path, tileInfo);
                }
            }
        }
    }

    public void ImportMapFromFile()
    {
        path = Application.dataPath + "/Maps/" + filename + ".txt";

        if (!File.Exists(path))
        {
            Debug.LogError("File " + filename + ".txt doesn't exist. Please create it or provide another filename.");
        }
        else
        {
            try
            {
                using(StreamReader sr = new StreamReader(path))
                {
                    string input = sr.ReadToEnd();
                    string[] lines = input.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                    int x, y;

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] data = lines[i].Split(new[] { ',', '(', ')', ' '}, System.StringSplitOptions.RemoveEmptyEntries);

                        foreach(TileBase asset in assets)
                        {
                            if(asset.name == data[2])
                            {
                                int.TryParse(data[0], out x);
                                int.TryParse(data[1], out y);
                                tilemap.SetTile(new Vector3Int(x, y, 0), asset);
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

    public int TileCount()
    {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] tiles = tilemap.GetTilesBlock(bounds);

        return tiles.Length;
    }
}

/*
 * 
 *                   string[] guid =  AssetDatabase.FindAssets(tile.name, new[] { "Assets/Mario Sprites/Assets" });
 *                   Debug.Log(AssetDatabase.GUIDToAssetPath(guid[0]));
 *                   
 *                   foreach(TileBase asset in assets)
                    {
                        if (asset.name == tile.name)
                        {
                            Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);

                            tilemap.SetTile(new Vector3Int(0, 0, 0), asset);
                        }
                    }
 */
