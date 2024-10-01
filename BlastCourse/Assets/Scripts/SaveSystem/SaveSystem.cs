using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    #region Data

    public static void DataSave(int scene, float[] spawnPoint, List<string> collectibles, List<string> keys, bool[] rpg, List<string> boxes, List<float> boxesX, List<float> boxesY, List<float> boxesZ, List<string> usedBoxes, List<string> dialoguesId, List<int> dialoguesCount)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/saveV1_0_0.data";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveData data = new SaveData(scene, spawnPoint, collectibles, keys, rpg, boxes, boxesX, boxesY, boxesZ, usedBoxes, dialoguesId, dialoguesCount);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData DataLoad()
    {
        string path = Application.persistentDataPath + "/saveV1_0_0.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning("Saved Data not found at " + path);
            return null;
        }
    }

    public static void DataDelete()
    {
        string path = Application.persistentDataPath + "/saveV1_0_0.data";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static bool DataCheck()
    {
        string path = Application.persistentDataPath + "/saveV1_0_0.data";
        return File.Exists(path);
    }

    #endregion



    #region Options

    public static void OptionsSave(float sense, float mstr, float sfx, float music, float dial, bool fs, KeyCode[] iK, bool hG, float cS, float fov, bool extraHud)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/optionsV1_0_0.data";
        FileStream stream = new FileStream(path, FileMode.Create);
        OptionsData data = new OptionsData(sense, mstr, sfx, music, dial, fs, iK, hG, cS, fov, extraHud);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static OptionsData OptionsLoad()
    {
        string path = Application.persistentDataPath + "/optionsV1_0_0.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);


            OptionsData data = formatter.Deserialize(stream) as OptionsData;

            stream.Close();
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
        string path = Application.persistentDataPath + "/optionsV1_0_0.data";
        return File.Exists(path);
    }

    #endregion
}
