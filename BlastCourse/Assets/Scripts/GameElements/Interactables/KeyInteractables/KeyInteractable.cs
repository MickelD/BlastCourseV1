using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class KeyInteractable : ActivableBase
{
    #region Variables

    public string _index;
    [Tooltip("Only add Triggers")] public ActivableBase _keyTrigger;
    private bool active; //Debug Only

    #endregion

    #region Methods

    [ActivableAction]
    public void Save(bool save)
    {
        active = save;
        if (active 
            && SaveLoader.Instance != null 
            && SaveLoader.Instance.KeysReached != null
            && !SaveLoader.Instance.KeysReached.Contains(_index)) SaveLoader.Instance.KeysReached.Add(_index);
        else if(!active
            && SaveLoader.Instance != null
            && SaveLoader.Instance.KeysReached != null
            && SaveLoader.Instance.KeysReached.Contains(_index)) SaveLoader.Instance.KeysReached.Remove(_index);

        SaveLoader.Instance.Save();
    }

    public IEnumerator StartUpActivate()
    {
        yield return new WaitForFixedUpdate();
        _keyTrigger.SendAllActivations(true);

        if (_keyTrigger is PushButton)
        {
            if ((_keyTrigger as PushButton).LockAfterFirstPress) (_keyTrigger as PushButton).Locked = true;
        }
        else if (_keyTrigger is UraniumConsumer)
        {
            (_keyTrigger as UraniumConsumer).MarkAsFed();
        }
    }

    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(KeyInteractable))]
public class KeyEditor: Editor
{
    KeyInteractable key;

    private void OnEnable()
    {
        key = (KeyInteractable)target;
    }

    public override void OnInspectorGUI()
    {
        if(key.Type != ActivableType.Action) key.Type = ActivableType.Action;
        GUILayout.Label("Index    " + key._index);

        ActivableBase t = EditorGUILayout.ObjectField("Trigger", key._keyTrigger, typeof(ActivableBase), true) as ActivableBase;
        if(t != key._keyTrigger)
        {
            key._keyTrigger = t;
            if (key._keyTrigger != null && key._keyTrigger.Type == ActivableType.Action)
            {
                Debug.LogError("Key Trigger can't be Action only activables.");
                key._keyTrigger = null;
            }
            else if (key._keyTrigger != null)
            {
                bool shouldAdd = true;
                foreach (Activation n in key._keyTrigger.RegisteredActions)
                {
                    if (n.Sender == key) shouldAdd = false;
                }
                if (shouldAdd)
                {
                    Activation Activate = new Activation(key._keyTrigger, key);
                    
                    MethodInfo[] 
                    methods = Activate.GetReceiverMethods();
                    Activate.CallingMethod = Activate.GetReceiverMethods()[0];
                    Activate.Style = ActivationStyle.Dynamic;
                    Activate.Delay = 0;

                    key._keyTrigger.RegisteredActions.Add(Activate);
                }
            }
        }
        
    }
}
#endif
