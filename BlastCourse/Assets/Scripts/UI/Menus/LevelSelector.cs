using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    #region Fields

    [Space(3), Header("Image"), Space(3)]
    public Image TutoImg;
    public Image WareImg;
    public Image CityImg;
    public Image LabImg;
    public Sprite TutoSpr;
    public Sprite WareSpr;
    public Sprite CitySpr;
    public Sprite LabSpr;
    public Sprite LockSpr;

    [Space(3), Header("Time"), Space(3)]
    public TextMeshProUGUI BestTimeTuto;
    public TextMeshProUGUI BestTimeWare;
    public TextMeshProUGUI BestTimeCity;
    public TextMeshProUGUI BestTimeLab;

    [Space(3), Header("Toggle"), Space(3)]
    public Toggle SpeedrunMode;
    public Toggle AllRockets;

    [Space(3), Header("Screens"), Space(3)]
    public GameObject SelectScreen;
    public GameObject ConfirmScreen;
    public TextMeshProUGUI SelLevText;

    [Space(3), Header("Buttons"), Space(3)]
    public Button TutoButton;
    bool tLock = false;
    public Button WareButton;
    bool wLock = false;
    public Button CityButton;
    bool cLock = false;
    public Button LabButton;
    bool lLock = false;

    [Space(3), Header("MenuSFX"), Space(3)]
    [SerializeField] private AudioCue ButtonSound;

    #endregion

    #region UnityFunctions

    private void Start()
    {
        SaveLoader.Instance.LoadDataWithoutSceneChange();

        if (SaveLoader.Instance != null && SaveLoader.Instance.CompletedLevels != null && SaveLoader.Instance.CompletedLevels.Length == 4)
        {
            tLock = SaveLoader.Instance.CompletedLevels[0];
            wLock = SaveLoader.Instance.CompletedLevels[1];
            cLock = SaveLoader.Instance.CompletedLevels[2];
            lLock = SaveLoader.Instance.CompletedLevels[3];
        }

        SaveLoader.Instance._levelSelect = false;
        SaveLoader.Instance._speedrunMode = false;


        SpeedLoader.Instance.ResetLevelTimers();
        SpeedLoader.Instance.Load();
    }

    #endregion

    #region Methods

    public void Open(bool open){ StartCoroutine(OpenC(open)); }
    private IEnumerator OpenC(bool open)
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        SelectScreen.SetActive(open);

        if (open)
        {
            CheckAllUnlocks();
        }
        else
        {
            ConfirmScreen.SetActive(false);
            SpeedrunMode.isOn = false;
            AllRockets.isOn = false;

            SaveLoader.Instance._levelSelect = false;
            SaveLoader.Instance._speedrunMode = false;
        }
    }

    public void OpenConfirm(bool open) { StartCoroutine(OpenConfirmC(open)); }
    private IEnumerator OpenConfirmC(bool open)
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        ConfirmScreen.SetActive(open);
    }


    public void StartLevel() { StartCoroutine(StartLevelC()); }
    private IEnumerator StartLevelC()
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        SaveLoader.Instance._levelSelect = true;
        SaveLoader.Instance.Load();
    }


    public void PlayTuto() 
    {
        //Create Level Save
        CreateLevelSave(2,new float[4] {-3f, 0.5f, -12f, 0f}, new bool[4] {false, false, false, false});

        //Show Confirm Screen
        OpenConfirm(true);
        SelLevText.text = "Reception";
    }
    public void PlayWare() 
    { 
        //Create Level Save
        CreateLevelSave(3, new float[4] { -15f, 0f, -4f, 0f}, new bool[4] { true, false, false, false });

        //Show Confirm Screen
        OpenConfirm(true);
        SelLevText.text = "Warehouse";
    }
    public void PlayCity() 
    { 
        //Create Level Save
        CreateLevelSave(4, new float[4] { -64f, -5f, -54f, 0f}, new bool[4] { true, false, true, false });

        //Show Confirm Screen
        OpenConfirm(true);
        SelLevText.text = "Sulfur Valley";
    }
    public void PlayLab() 
    { 
        //Create Level Save
        CreateLevelSave(5, new float[4] { -21f, -13.5f, 11.5f, 90f}, new bool[4] { true, true, true, false });

        //Show Confirm Screen
        OpenConfirm(true);
        SelLevText.text = "The Lab";
    }


    private void CheckUnlock(Button b, Image i, Sprite s, TextMeshProUGUI t,bool lvLock, float time)
    {
        if (lvLock)
        {
            b.interactable = true;
            i.sprite = s;
            t.text = UpdateTime(time);
        }
        else
        {
            b.interactable = false;
            i.sprite = LockSpr;
            t.text = "PB: --:--:--";
        }
    }
    private void CheckAllUnlocks()
    {
        CheckUnlock(TutoButton, TutoImg, TutoSpr, BestTimeTuto, tLock, SpeedLoader.Instance.tutoT);
        CheckUnlock(WareButton, WareImg, WareSpr, BestTimeWare, wLock, SpeedLoader.Instance.wareT);
        CheckUnlock(CityButton, CityImg, CitySpr, BestTimeCity, cLock, SpeedLoader.Instance.cityT);
        CheckUnlock(LabButton, LabImg, LabSpr, BestTimeLab, lLock, SpeedLoader.Instance.labT);
    }
    private string UpdateTime(float t)
    {
        int mm = Mathf.FloorToInt(t / 60);
        int ss = Mathf.FloorToInt(t % 60);
        int ms = Mathf.FloorToInt((t * 100) % 100);
        return "PB: " + string.Format("{0:00}:{1:00},{2:00}", mm, ss, ms);
    }


    private void CreateLevelSave(int index, float[] posRot, bool[] unlocks)
    {
        if(AllRockets.isOn) unlocks = new bool[4] {true, true, true, true};
        SaveSystem.LevelDataSave(index //Level Index
            , posRot //Position & Rotation
            #region Unused
            , new List<string>(), new List<string>(),
        #endregion
            unlocks //RPG Unlocked
            #region Unused
            , new List<string>(), new List<float>(), new List<float>(), new List<float>(), new List<string>(), new List<string>(), new List<int>(), new bool[4]);
        #endregion
    }

    public void AllRpgsToggle()
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
        }
    }
    public void SpeedrunToggle()
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
        }

        SaveLoader.Instance._speedrunMode = SpeedrunMode.isOn;
    }

    #endregion
}


