using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FileManager
{
    public static bool WriteToFile(string a_FileName, string a_FileContents)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            File.WriteAllText(fullPath, a_FileContents);
            Debug.Log(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string a_FileName, out string result)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }

    public static bool DeleteFile(string a_FileName)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            // check if file exists
            if (!File.Exists(fullPath))
            {
                Debug.Log($"File path {fullPath} does not exist.");
                return false;
            }
            else
            {
                File.Delete(fullPath);
                return true;

            }
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to load from {fullPath} with exception {e}");
            return false;
        }
    }
}