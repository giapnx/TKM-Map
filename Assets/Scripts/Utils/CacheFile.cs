using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CacheFile
{
    public static WWW LoadOrDownLoad(string url, IUrlToPathParse parse, MonoBehaviour monoInstance)
    {

        //get file path with url

        string filePath = parse.GetFilePath(url);

        //check file name exist

        WWW www;
        if (File.Exists(filePath))
        {
            //if existed then load file
            www = new WWW("file://" + filePath);
            Debug.Log("Load file: " + filePath);
        }
        else
        {
            //if not download
            www = new WWW(url);
            Debug.Log("Download file: " + url);
        }
        monoInstance.StartCoroutine(DoLoad(www, filePath));
        return www;
    }

    public static IEnumerator DoLoad(WWW www, string filePath)
    {
        yield return www;
        if (www.error == null)
        {
            //save to file
            File.WriteAllBytes(filePath, www.bytes);
        } else
        {
            Debug.Log("Error while download file : " + www.error + " - url - " + www.url);
        }
    }
}
