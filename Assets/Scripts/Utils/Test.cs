using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Test : MonoBehaviour
{

	// Use this for initialization
	void Start () {
        double lon = 139.820550;
        double lat = 35.665557;
        int zoomLevel = 14;
        Debug.Log(CoordinatesConverter.LonToX(lon, zoomLevel));
        Debug.Log(CoordinatesConverter.LatToY(lat, zoomLevel));
    }

    public IEnumerator test()
    {
        string url = "http://cyberjapandata.gsi.go.jp/xyz/dem/14/14535/6452.txt";

        IUrlToPathParse urlParse = new TileMapUrlParse(Settings.TILE_MAP_FILE_FOLDER);
        WWW www = CacheFile.LoadOrDownLoad(url, urlParse, this);
        yield return www;
        if (www.error == null)
        {
            Debug.Log(www.text);
        }
    }
}
