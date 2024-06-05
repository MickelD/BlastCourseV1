using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    #region Data

    public static void DataSave(int scene, float[] spawnPoint, List<string> collectibles, List<string> keys, bool[] rpg, List<string> boxes, List<float> boxesX, List<float> boxesY, List<float> boxesZ, List<string> usedBoxes)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.data";
        FileStream stream = new FileStream(path, FileMode.Create);
        //Debug.Log("xDataOpenS");
        SaveData data = new SaveData(scene, spawnPoint, collectibles, keys, rpg, boxes, boxesX, boxesY, boxesZ, usedBoxes);

        Debug.Log("Saving data to " + path);

        formatter.Serialize(stream, data);
        stream.Close();
        //Debug.Log("xDataCloseS");
    }

    public static SaveData DataLoad()
    {
        string path = Application.persistentDataPath + "/save.data";
        if (File.Exists(path))
        {
            Debug.Log("Loading data from " + path);

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            //Debug.Log("xDataOpenL");
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            //Debug.Log("xDataCloseL");

            return data;
        }
        else
        {
            Debug.Log("Saved Data not found at " + path);
            return null;
        }
    }

    public static void DataDelete()
    {
        string path = Application.persistentDataPath + "/save.data";
        if (File.Exists(path))
        {
            Debug.Log("Data deleted from " + path);
            File.Delete(path);
        }
    }

    public static bool DataCheck()
    {
        string path = Application.persistentDataPath + "/save.data";
        return File.Exists(path);
    }

    #endregion



    #region Options

    public static void OptionsSave(float sense, float mstr, float sfx, float music, float dial, bool fs, KeyCode[] iK, bool hG)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/options.data";
        FileStream stream = new FileStream(path, FileMode.Create);
        //Debug.Log("xOptionsOpenS");
        OptionsData data = new OptionsData(sense, mstr, sfx, music, dial, fs, iK, hG);;

        Debug.Log("Saving data to " + path);

        formatter.Serialize(stream, data);
        stream.Close();
        //Debug.Log("xOptionsCloseS");
    }

    public static OptionsData OptionsLoad()
    {
        string path = Application.persistentDataPath + "/options.data";
        if (File.Exists(path))
        {
            Debug.Log("Loading data from " + path);

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            //Debug.Log("xOptionsOpenL");


            OptionsData data = formatter.Deserialize(stream) as OptionsData;

            stream.Close();
            //Debug.Log("xOptionsCloseL");
            return data;
        }
        else
        {
            Debug.Log("Saved Data not found at " + path);
            return null;
        }
    }

    public static bool OptionsCheck()
    {
        string path = Application.persistentDataPath + "/options.data";
        return File.Exists(path);
    }

    #endregion
}
