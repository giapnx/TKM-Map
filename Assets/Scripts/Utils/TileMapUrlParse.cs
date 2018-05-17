using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileMapUrlParse : IUrlToPathParse
{
    protected string tileMapFolder;

    public TileMapUrlParse(string tileMapFolder)
    {
        this.tileMapFolder = tileMapFolder;
    }

    public string GetFilePath(string url)
    {
        int index = url.IndexOf("dem_png");
        if (index == -1)
        {
            index = url.IndexOf("dem");
            if (index == -1)
            {
                throw new ArgumentException("url :" + url + " invalid");
            }
            index += 4;
        } else
        {
            index += 8;
        }

        string fileName = url.Substring(index);
        fileName = fileName.Replace("/", "_");
        return tileMapFolder + fileName;
    }
}
