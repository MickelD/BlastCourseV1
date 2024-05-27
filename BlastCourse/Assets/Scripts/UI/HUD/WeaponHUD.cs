using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CustomMethods;

public class WeaponHUD : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Object References"), Space(3)]
    public Image gRpgIcon;
    public RpgStats remoteStats;
    public Image[] gRemoteIcons;

    [Space(5), Header("Remote Count Tweener Values"), Space(3)]
    public Color outColor;
    public Vector3 outSizeD;
    public float outColorTime;
    public float outSizeDTime;
    public float outFadeTime;
    [Space(3)]
    public Color inColor;
    public Vector3 inSizeD;
    public float inColorTime;
    public float inSizeDTime;
    public float inFadeTime;

    private float _idleIconAlpha;
    private int _remoteCount;

    #endregion

    #region UnityFunctions

    private void Awake()
    {
        EventManager.OnChangeRpg += UpdateRpg;
        EventManager.OnFireOrDetonateRemote += UpdateRemoteCount;
    }

    private void Start()
    {
        _idleIconAlpha = gRemoteIcons[0].color.a;

        foreach (Image icon in gRemoteIcons) icon.color = Color.clear;
    }

    private void OnDestroy()
    {
        EventManager.OnChangeRpg -= UpdateRpg;
        EventManager.OnFireOrDetonateRemote -= UpdateRemoteCount;
    }

    #endregion

    #region Methods

    private void UpdateRpg(RpgData rpg)
    {
        if (rpg != null && rpg._rpgStats.Unlocked)
        {
            gRpgIcon.enabled = true;
            gRpgIcon.sprite = rpg._rpgStats.AssociatedVisuals.RpgIcon;
        }
        else
        {
            gRpgIcon.enabled = false;
        }
    }

    private void UpdateRemoteCount(bool newRemote)
    {
        if (newRemote) //SPAWNED NEW REMOTE ROCKET
        {
            _remoteCount = Mathf.Clamp(_remoteCount + 1, 0, remoteStats.ActiveRocketCap-1);
            if (_remoteCount > 0 && _remoteCount <= remoteStats.ActiveRocketCap-1) SetRemoteIcon(gRemoteIcons[_remoteCount - 1], newRemote);
        }
        else //DETONATED OR DIFFUSED NEW REMOTE ROCKET
        {
            if (_remoteCount > 0 && _remoteCount <= remoteStats.ActiveRocketCap-1) SetRemoteIcon(gRemoteIcons[_remoteCount - 1], newRemote);
            _remoteCount = Mathf.Clamp(_remoteCount - 1, 0, remoteStats.ActiveRocketCap-1);
        }
    }

    private void SetRemoteIcon(Image icon, bool set)
    {
        if (set)
        {
            icon.DOKill();

            inColor.a = 0;
            icon.color = inColor;
            icon.rectTransform.localScale = inSizeD;

            icon.DOColor(Color.white, inColorTime);
            icon.DOFade(_idleIconAlpha, inFadeTime);
            icon.rectTransform.DOScale(Vector3.one, inSizeDTime);
        }
        else
        {
            icon.DOKill();

            icon.DOColor(outColor, outColorTime);
            icon.DOFade(0f, outFadeTime);
            icon.rectTransform.DOScale(outSizeD, outSizeDTime);
        }
    }


    #endregion
}


