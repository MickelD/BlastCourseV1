using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;
using UnityEngine.Events;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ActivableBase : MonoBehaviour
{
    #region Fields
    [Space(5), Header("ACTIVABLE DATA"), Space(3)]
    [SerializeField] public ActivableType Type;
    [HideInInspector] public bool Active;

    //ACTION-TYPE SPECIFIC FIELDS
    [Space(5), Header("Activable Action"), Space(3)]
    [BelongToActivableType(ActivableType.Action), SerializeField] public bool ActivateOnStart;
    [BelongToActivableType(ActivableType.Action), SerializeField] public Activation StartingAction;
    [Tooltip("Toggle Activations now ignore current state of this target, and simply alternate between activating or deactivating"), 
    OnlyShowFor(typeof(AndGate))] public bool OverrideToggleBehaviour;
    [BelongToActivableType(ActivableType.Action), SerializeField] public List<Activation> RegisteredTriggers = new();

    //TRIGGER-TYPE SPECIFIC FIELDS
    [Space(5), Header("Activable Trigger"), Space(3)]
    [BelongToActivableType(ActivableType.Trigger), SerializeField] public List<Activation> RegisteredActions = new();


    //determine if this object is mimicking the actions of a parent activable of the same type
    public bool IsMimic() 
    {
        if (transform.parent != null) 
        {
            ActivableBase parent = transform.parent.GetComponent<ActivableBase>();
            return parent != null && parent.GetType() == GetType();
        }
        else return false;
    }

    #endregion

    #region Unity Functions

    protected virtual void Start()
    {
        if(Type != ActivableType.Trigger && ActivateOnStart && StartingAction != null && !IsMimic())
        {
            TrySendActivation(StartingAction, true);
        }

        if (gameObject.TryGetComponent<Animator>(out Animator anim))
        {
            anim.keepAnimatorStateOnDisable = true;
        }

    }

    #endregion

    #region Methods

    public virtual void ReceiveActivation(bool isActive) 
    {
        if (Type == ActivableType.Trigger) return;
        Active = isActive;
    }

    public virtual void SendAllActivations(bool isActive)
    {
        if (Type == ActivableType.Action) return;

        foreach (Activation activation in RegisteredActions)
        {
            if (activation.Delay == 0)
            {
                TrySendActivation(activation, isActive);
            }
            else
            {
                StartCoroutine(DelayedActivation(activation, isActive, activation.Delay));
            }
        }
    }

    private void TrySendActivation(Activation activation, bool isActive)
    {
        if (activation.Receiver != null)
        {
            if (activation.CallingMethod != null)
            {
                bool shouldSend;
                switch (activation.Style)
                {
                    default:
                    case ActivationStyle.Dynamic:
                        shouldSend = isActive;
                        break;

                    case ActivationStyle.Toggle:
                        if (activation.Receiver.OverrideToggleBehaviour) shouldSend = activation.ToggleState = !activation.ToggleState;
                        else shouldSend = !activation.Receiver.Active;
                        break;

                    case ActivationStyle.True:
                        shouldSend = true;
                        break;

                    case ActivationStyle.False:
                        shouldSend = false;
                        break;
                }

                object[] methodParams = ExtendedDataUtility.Select(activation.CallingMethod.GetParameters().Length == 1, new object[] { shouldSend }, new object[] {shouldSend, activation});

                activation.Receiver.ReceiveActivation(shouldSend);
                activation.CallingMethod.Invoke(activation.Receiver, methodParams);

                foreach (Transform child in activation.Receiver.transform)
                {
                    ActivableBase activ = child.GetComponent<ActivableBase>();
                    if (activ != null && activ.GetType() == activation.Receiver.GetType()) activation.CallingMethod.Invoke(activ, methodParams);
                }
            }
            else Debug.LogWarning("Action of " + name + " to " + activation.Receiver.name
                                + " was not called because no " + nameof(Activation.CallingMethod) + " is defined." +
                                "\n Please make sure your object as suitable methods defined");
        }
        else Debug.LogWarning("Action of " + name + " was not called because " + nameof(Activation.Receiver) + " is missing");
    }

    private IEnumerator DelayedActivation(Activation activation, bool isActive, float delay)
    {
        yield return new WaitForSeconds(delay);
        TrySendActivation(activation,isActive);
    }

    #endregion

    #if UNITY_EDITOR

    readonly GUIStyle gizmosStyle = new();

    Color orangeCol = new (1f, 140f/255f, 0f);

    private void OnDrawGizmos()
    {
        if (Type is ActivableType.Trigger or ActivableType.Both)
        {
            Handles.color = Color.blue;

            for (int i = 0; i < RegisteredActions.Count; i++)
            {
                if (RegisteredActions[i].Receiver != null)
                {
                    Handles.DrawDottedLine(transform.position, RegisteredActions[i].Receiver.transform.position, 3f);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        int triggerLines = 0;

        gizmosStyle.alignment = TextAnchor.MiddleCenter;
        if ((Type == ActivableType.Trigger || Type == ActivableType.Both) && RegisteredActions != null)
        {
            Handles.color = gizmosStyle.normal.textColor = RegisteredActions.Count == 0 ? Color.gray : Color.blue;
            Handles.Label(transform.position, this.name + "\n" + RegisteredActions.Count + " Action" + ExtendedDataUtility.Select(RegisteredActions.Count == 1, "", "s"), gizmosStyle);

            for (int i = 0; i < RegisteredActions.Count; i++)
            {
                string space = string.Concat(Enumerable.Repeat("\n", triggerLines));
                if (RegisteredActions[i].Receiver != null)
                {
                    Handles.color = gizmosStyle.normal.textColor = Color.blue;
                    Handles.Label(RegisteredActions[i].Receiver.transform.position, space + RegisteredActions[i].ActivationStyleLabel() + RegisteredActions[i].CallingMethod.Name, gizmosStyle);
                    triggerLines++;
                }
                else
                {
                    Handles.color = gizmosStyle.normal.textColor = Color.red;
                    Handles.Label(transform.position, space + "Target missing for Action#" + (i + 1), gizmosStyle);
                    triggerLines++;
                }
            }
        }

        if ((Type == ActivableType.Action || Type == ActivableType.Both) && RegisteredTriggers != null)
        {
            Handles.color = gizmosStyle.normal.textColor = RegisteredTriggers.Count == 0 ? Color.gray : orangeCol;
            string extraSpace = Type == ActivableType.Both ? string.Concat(Enumerable.Repeat("\n", 2 + triggerLines)) : this.name;
            Handles.Label(transform.position, extraSpace + "\n" + RegisteredTriggers.Count + " Trigger" + ExtendedDataUtility.Select(RegisteredTriggers.Count == 1, "", "s"), gizmosStyle);

            int actionlines = Type == ActivableType.Both ? triggerLines : 0;
            for (int i = 0; i < RegisteredTriggers.Count; i++)
            {
                string space = string.Concat(Enumerable.Repeat("\n", actionlines));

                if (RegisteredTriggers[i].Sender != null)
                {
                    Handles.color = gizmosStyle.normal.textColor = orangeCol;
                    Handles.Label(RegisteredTriggers[i].Sender.transform.position, space + RegisteredTriggers[i].ActivationStyleLabel() + RegisteredTriggers[i].CallingMethod.Name, gizmosStyle);
                    Handles.DrawDottedLine(transform.position, RegisteredTriggers[i].Sender.transform.position, 3f);
                    actionlines++;
                }
                else
                {
                    Handles.color = gizmosStyle.normal.textColor = Color.red;
                    Handles.Label(transform.position, space + "Trigger missing for Action#" + (i + 1), gizmosStyle);
                    actionlines++;
                }
            }
        }
    }

    #endif
}

public enum ActivableType
{
    Action,
    Trigger,
    Both
}

public enum ActivationStyle
{
    Dynamic,
    Toggle,
    True,
    False
}

[System.Serializable]
public class Activation
{
    public bool UpdateSharedRegistry;

    public ActivableBase _receiver;
    public ActivableBase Receiver
    {
        get => _receiver;
        set
        {
            //If there is a sender trigger, update an action's triggers
            if (Sender != null && Sender.Type != ActivableType.Action && UpdateSharedRegistry)
            {
                //changing from current receiver to new Value
                if (value != _receiver)
                {
                    //Debug.Log("Sender " + Sender.name + " changed receiver from " + _receiver + " to " + value);
                    //try remove from old list
                    if (_receiver != null && _receiver.RegisteredTriggers.Contains(this))
                    {
                        //Debug.Log("Removed " + this + " from Receiver " + _receiver + "큦 triggers");
                        _receiver.RegisteredTriggers.Remove(this);
                    }

                    //try add to new list
                    if (value != null)
                    {
                        if (!value.RegisteredTriggers.Contains(this))
                        {
                            //Debug.Log("Added " + this + " to Receiver " + value + "큦 triggers");
                            value.RegisteredTriggers.Add(this);
                        }
                    }
                    else
                    {
                        if (Sender.RegisteredActions.Contains(this)) Sender.RegisteredActions.Remove(this);
                    }
                }
            }
            _receiver = value;
        }
    }

    public ActivableBase _sender;

    public ActivableBase Sender
    {
        get => _sender; 
        set
        {
            //If there is a receiver action, update a trigger's actions
            if (Receiver != null && Receiver.Type != ActivableType.Trigger && UpdateSharedRegistry)
            {
                //changing from targetObject to new Value
                if (value != _sender)
                {
                    //Debug.Log("Receiver " + Receiver.name + " changed sender from " + _receiver + " to " + value);
                    //try remove from old list
                    if (_sender != null && _sender.RegisteredActions.Contains(this))
                    {
                        //Debug.Log("Removed " + this + " from Sender " + _sender + "큦 actions");
                        _sender.RegisteredActions.Remove(this);
                    }

                    //try add to new list
                    if (value != null)
                    {
                        if (!value.RegisteredActions.Contains(this))
                        {
                            //Debug.Log("Added " + this + " to Sender " + value + "큦 actions");
                            value.RegisteredActions.Add(this);
                        }
                    }
                    else
                    {
                        if (Receiver.RegisteredTriggers.Contains(this)) Receiver.RegisteredTriggers.Remove(this);
                    }
                }
            }
            _sender = value;
        }
    }

    public MethodInfo _callingMethod;
    public MethodInfo CallingMethod
    {
        get 
        {
            if (_callingMethod == null) _callingMethod = GetReceiverMethods()[SelectedIndex];
            return _callingMethod; 
        }
        set 
        { 
            _callingMethod = value; 
        }
    }
    public int SelectedIndex;
    public MethodInfo[] _receiverMethods;
    public MethodInfo[] GetReceiverMethods()
    {
        if(_receiverMethods == null && Receiver != null) 
            _receiverMethods = Receiver.GetType().GetMethods()
                                                    .Where(t => Attribute.IsDefined(t, typeof(ActivableAction)) && 
                                                        ((t.GetParameters().Length == 1 && t.GetParameters()[0].ParameterType == typeof(bool)) || 
                                                        (t.GetParameters().Length == 2 && t.GetParameters()[0].ParameterType == typeof(bool) && t.GetParameters()[1].ParameterType == typeof(Activation))))
                                                .ToArray();
        return _receiverMethods;
    }
    public ActivationStyle Style;

    public Activation(ActivableBase sender, ActivableBase receiver, bool updateSharedRegistry = true) { 
        UpdateSharedRegistry = updateSharedRegistry;
        Sender = sender;
        Receiver = receiver;

        updateSharedRegistry = true;
    }

    public string ActivationStyleLabel()
    {
        return Style switch
        {
        ActivationStyle.Dynamic => "Set ",
        ActivationStyle.Toggle => "Toggle ",
        ActivationStyle.True => "Do ",
        ActivationStyle.False => "Do Not ",
        _ => "StyleEnumError"
        };
    }

    public bool ToggleState;

    public float Delay;
}

#if UNITY_EDITOR

[CustomEditor(typeof(ActivableBase), true)]
public class ActivableEditor : Editor
{
    Color darkTint = new(0, 0, 0, 0.05f);
    Color lightTint = new(1, 1, 1, 0.05f);

    private void DrawActivationField(ActivableBase thisActivable, int index, bool belongToTriggerList)
    {
        #region Format
        GUI.backgroundColor = index % 2 == 0 ? darkTint : lightTint;
        GUIStyle style = new() { normal = { background = Texture2D.whiteTexture } };

        GUILayout.BeginVertical(style);
        EditorGUILayout.Space(3);
        GUILayout.BeginHorizontal();
        #endregion

        Activation activation = belongToTriggerList ? thisActivable.RegisteredActions[index] : thisActivable.RegisteredTriggers[index];

        EditorGUILayout.LabelField((index + 1) + ". ", GUILayout.Width(15f));

        ActivableBase selection = belongToTriggerList ?
        //ACTIVATION ON TRIGGER LIST SELECTS THE RECEIVER
        EditorGUILayout.ObjectField(activation.Receiver, typeof(ActivableBase), true) as ActivableBase :
        //ACTIVATION ON ACTION LIST SELECTS THE SENDER
        EditorGUILayout.ObjectField(activation.Sender, typeof(ActivableBase), true) as ActivableBase;

        if (belongToTriggerList) activation.Receiver = selection;
        else activation.Sender = selection;

        //If there is an assigned object, check its methods and type
        if (selection != null)
        {
            bool invalidSelection = false;

            if (thisActivable.IsMimic()) //Ignore Mimic activables
            {
                if(belongToTriggerList) activation.Receiver = null;
                else activation.Sender = null;

                Debug.LogWarning("Selected Activable is currently mimicking a parent activable and is not considered an independent object!");
                invalidSelection = true;
            }
            else if (belongToTriggerList) //TRIGGERs cannot activate other triggers DELETE INVALID RECEIVER
            {
                if (activation.Receiver.Type == ActivableType.Trigger)
                {
                    activation.Receiver = null;
                    Debug.LogWarning("Activable Triggers should not try to activate other Triggers. Reference was removed." +
                                     "\nTry changing your target큦 Type to " + ActivableType.Action + " or " + ActivableType.Both);
                    invalidSelection = true;
                }
            }
            else // ACTIONS should not be activated by other actions DELETE INVALID SENDER
            {
                if (activation.Sender.Type == ActivableType.Action)
                {
                    activation.Sender = null;
                    Debug.LogWarning("Activable Actions should not be activated by other Actions. Reference was removed." +
                            "\nTry changing your target큦 Type to " + ActivableType.Trigger + " or " + ActivableType.Both);
                    invalidSelection = true;
                }
            }

            if (!invalidSelection)
            {
                //Only include the methods with the activable action attribute, that have either no parameters or take one boolean input
                MethodInfo[] methods = activation.GetReceiverMethods();

                //Notify designers when object has no selectable methods
                if (methods.Length == 0)
                {
                    Debug.LogWarning("No suitable methods were found in " + activation.Receiver +
                                     "\nRemember that all selectable methods must use the " + nameof(ActivableAction) + " attribute and take 1 bool parameter");
                }
                else
                {
                    #region Draw Method Name
                    string[] methodNames = new string[methods.Length];
                    for (int n = 0; n < methods.Length; n++)
                    {
                        string paramNames = "";
                        foreach (ParameterInfo parameter in methods[n].GetParameters())
                        {
                            paramNames += parameter.ParameterType.Name;
                        }

                        methodNames[n] = methods[n].Name + "()";
                    }
                    #endregion

                    //Assign method to call
                    activation.SelectedIndex = EditorGUILayout.Popup(activation.SelectedIndex, methodNames, GUILayout.MaxWidth(100f));
                    activation.CallingMethod = methods[activation.SelectedIndex];

                    activation.Style = (ActivationStyle)EditorGUILayout.EnumPopup(activation.Style, GUILayout.MaxWidth(100f));

                    activation.Delay = EditorGUILayout.FloatField(activation.Delay, GUILayout.MaxWidth(35f));
                }
            }
        }

        if (GUILayout.Button("x", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
        {
            if (belongToTriggerList) //TRIGGER, remove from our sender's triggers
            {
                if (activation.Receiver != null &&
                    activation.Receiver.RegisteredTriggers.Contains(activation))
                    activation.Receiver.RegisteredTriggers.Remove(activation);
                thisActivable.RegisteredActions.RemoveAt(index);
            }
            else //ACTION, remove from our receiver's actions
            {
                if (activation.Sender != null &&
                    activation.Sender.RegisteredTriggers.Contains(activation))
                    activation.Sender.RegisteredTriggers.Remove(activation);
                thisActivable.RegisteredTriggers.RemoveAt(index);
            }
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.Space(3);
        GUILayout.EndVertical();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ActivableBase act = (ActivableBase)target;
        if (act == null) return;

        if (act.IsMimic())
        {
            EditorGUILayout.HelpBox("This Activable is a Mimic because it is parented by an Activable of the same type" +
                "\n As such, it has no independent behaviour and simply imitates the parent Activable", MessageType.Info);
        }
        else
        {
            foreach (FieldInfo field in act.GetType().GetFields())
            {
                #region Check For Skips
                //Exclue HideInInspector Fields and Fields that choose to override this editor
                if (Attribute.IsDefined(field, typeof(HideInInspector)) || Attribute.IsDefined(field, typeof(OverrideBaseActivableEditor))) continue;

                //Exclude fields including the Trigger flag
                if (Attribute.IsDefined(field, typeof(BelongToActivableType))
                    && ((BelongToActivableType)Attribute.GetCustomAttribute(field, typeof(BelongToActivableType))).activableType == ActivableType.Trigger
                    && act.Type == ActivableType.Action) continue;

                //exclude fields including the Action flag
                if (Attribute.IsDefined(field, typeof(BelongToActivableType))
                    && ((BelongToActivableType)Attribute.GetCustomAttribute(field, typeof(BelongToActivableType))).activableType == ActivableType.Action
                    && act.Type == ActivableType.Trigger) continue;

                //exclude fields based on condition
                if (Attribute.IsDefined(field, typeof(ExcludeFromActivableEditor)))
                {
                    string boolName = ((ExcludeFromActivableEditor)Attribute.GetCustomAttribute(field, typeof(ExcludeFromActivableEditor))).BooleanName;
                    if ((bool)act.GetType().GetField(boolName).GetValue(act) == false) continue;
                }

                //exclude fields specific to types
                if (Attribute.IsDefined(field, typeof(OnlyShowFor))
                    && ((OnlyShowFor)Attribute.GetCustomAttribute(field, typeof(OnlyShowFor))).ActivableClass != act.GetType())
                    continue;

                #endregion

                //Starting method
                if (field.Name == nameof(act.StartingAction))
                {
                    if (act.ActivateOnStart)
                    {
                        if (act.StartingAction == null || act.StartingAction.Receiver == null) act.StartingAction = new Activation(act, act, false);

                        //Only include the methods with the activable action attribute, that have either no parameters or take one boolean input
                        MethodInfo[] methods = act.StartingAction.GetReceiverMethods();

                        //Notify designers when object has no selectable methods
                        if (methods.Length == 0)
                        {
                            Debug.LogWarning("No suitable methods were found in " + act +
                                             "\nRemember that all selectable methods must use the " + nameof(ActivableAction) + " attribute and take 1 bool parameter");
                        }
                        else
                        {
                            #region Draw Method Name
                            string[] methodNames = new string[methods.Length];
                            for (int n = 0; n < methods.Length; n++)
                            {
                                string paramNames = "";
                                foreach (ParameterInfo parameter in methods[n].GetParameters())
                                {
                                    paramNames += parameter.ParameterType.Name;
                                }

                                methodNames[n] = methods[n].Name + "()";
                            }
                            #endregion

                            //Assign method to call
                            EditorGUILayout.BeginHorizontal();
                            act.StartingAction.SelectedIndex = EditorGUILayout.Popup("On Start Method", act.StartingAction.SelectedIndex, methodNames);
                            act.StartingAction.CallingMethod = methods[act.StartingAction.SelectedIndex];

                            act.StartingAction.Style = (ActivationStyle)EditorGUILayout.EnumPopup(act.StartingAction.Style, GUILayout.MaxWidth(105f));
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                //LIST OF REGISTERED ACTIONS
                else if (field.Name == nameof(act.RegisteredActions))
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Activable Trigger", EditorStyles.boldLabel);
                    EditorGUILayout.Space(3);
                    EditorGUILayout.LabelField(act.GetType().Name + " Actions", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.Space(3);

                    #region Add/remove Action Button

                    //Add and Remove from List
                    GUI.backgroundColor = Color.white;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add new target"))
                    {
                        act.RegisteredActions.Add(new Activation(act, null)); //add new element
                    }
                    if (GUILayout.Button("Remove last target") && act.RegisteredActions.Count > 0)
                    {
                        if (act.RegisteredActions[^1].Receiver != null &&
                            act.RegisteredActions[^1].Receiver.RegisteredTriggers.Contains(act.RegisteredActions[^1]))
                            act.RegisteredActions[^1].Receiver.RegisteredTriggers.Remove(act.RegisteredActions[^1]);
                        act.RegisteredActions.RemoveAt(act.RegisteredActions.Count - 1); //remove last element
                    }
                    GUILayout.EndHorizontal();

                    #endregion

                    //skip trying to draw the array if it has not been initialized
                    if (act.RegisteredActions == null) continue;

                    //iterate through all Registered Actions
                    for (int i = 0; i < act.RegisteredActions.Count; i++) DrawActivationField(act, i, true);

                    #region DUMP
                    /*
                    //Iterate through all activable actions
                    for (int i = 0; i < act.RegisteredActions.Count; i++)
                    {
                        #region Format
                        Color darkTint = new(0, 0, 0, 0.05f);
                        Color lightTint = new(1, 1, 1, 0.05f);
                        GUI.backgroundColor = i % 2 == 0 ? darkTint : lightTint;
                        GUIStyle style = new() { normal = { background = Texture2D.whiteTexture } };
                        #endregion

                        GUILayout.BeginVertical(style);
                        EditorGUILayout.Space(3);
                        GUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField((i + 1) + ". ", GUILayout.Width(15f));

                        ActivableBase selection = EditorGUILayout.ObjectField(act.RegisteredActions[i].Receiver, typeof(ActivableBase), true) as ActivableBase;

                        act.RegisteredActions[i].Receiver = selection;

                        //If there is an assigned object, check its methods and type
                        if (selection != null)
                        {
                            //Prevent assining triggers to activate other triggers, as that makes no fucking sense
                            if (act.RegisteredActions[i].Receiver.Type == ActivableType.Trigger)
                            {
                                act.RegisteredActions[i].Receiver = null;
                                Debug.LogWarning("Activable Triggers should not try to activate other Triggers. Reference was removed." +
                                                 "\nTry changing your target큦 Type to " + ActivableType.Action + " or " + ActivableType.Both);
                                continue;
                            }

                            //Only include the methods with the activable action attribute, that have either no parameters or take one boolean input
                            MethodInfo[] methods = act.RegisteredActions[i].ReceiverMethods;

                            //Notify designers when object has no selectable methods
                            if (methods.Length == 0)
                            {
                                Debug.LogWarning("No suitable methods were found in " + act.RegisteredActions[i].Receiver +
                                                 "\nRemember that all selectable methods must use the " + nameof(ActivableAction) + " attribute and take 1 bool parameter");
                            }
                            else
                            {
                                #region Draw Method Name
                                string[] methodNames = new string[methods.Length];
                                for (int n = 0; n < methods.Length; n++)
                                {
                                    string paramNames = "";
                                    foreach (ParameterInfo parameter in methods[n].GetParameters())
                                    {
                                        paramNames += parameter.ParameterType.Name;
                                    }

                                    methodNames[n] = methods[n].Name + "()";
                                }
                                #endregion

                                //Assign method to call
                                act.RegisteredActions[i].SelectedIndex = EditorGUILayout.Popup(act.RegisteredActions[i].SelectedIndex, methodNames, GUILayout.MaxWidth(100f));
                                act.RegisteredActions[i].CallingMethod = methods[act.RegisteredActions[i].SelectedIndex];

                                act.RegisteredActions[i].Style = (ActivationStyle)EditorGUILayout.EnumPopup(act.RegisteredActions[i].Style, GUILayout.MaxWidth(100f));

                                act.RegisteredActions[i].Delay = EditorGUILayout.FloatField(act.RegisteredActions[i].Delay, GUILayout.MaxWidth(35f));
                            }
                        }

                        if (GUILayout.Button("x", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                        {
                            if (act.RegisteredActions[i].Receiver != null &&
                                act.RegisteredActions[i].Receiver.RegisteredTriggers.Contains(act.RegisteredActions[i]))
                                act.RegisteredActions[i].Receiver.RegisteredTriggers.Remove(act.RegisteredActions[i]);
                            act.RegisteredActions.RemoveAt(i);
                        }

                        GUILayout.EndHorizontal();
                        EditorGUILayout.Space(3);
                        GUILayout.EndVertical();
                    }
                    */

                    #endregion

                    EditorGUILayout.Space(3);
                }
                ////LIST OF REGISTERED TRIGGERS
                else if (field.Name == nameof(act.RegisteredTriggers))
                {
                    EditorGUILayout.Space(3);
                    EditorGUILayout.LabelField(act.GetType().Name + " Triggers", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.Space(3);

                    #region Add/remove Trigger Button

                    //Add and Remove from List
                    GUI.backgroundColor = Color.white;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add new trigger"))
                    {
                        act.RegisteredTriggers.Add(new Activation(null, act)); //add new element
                    }
                    if (GUILayout.Button("Remove last trigger") && act.RegisteredTriggers.Count > 0)
                    {
                        if (act.RegisteredTriggers[^1].Sender != null &&
                            act.RegisteredTriggers[^1].Sender.RegisteredActions.Contains(act.RegisteredTriggers[^1]))
                            act.RegisteredTriggers[^1].Sender.RegisteredActions.Remove(act.RegisteredTriggers[^1]);
                        act.RegisteredTriggers.RemoveAt(act.RegisteredTriggers.Count - 1); //remove last element
                    }
                    GUILayout.EndHorizontal();

                    #endregion

                    //skip trying to draw the array if it has not been initialized
                    if (act.RegisteredTriggers == null) continue;

                    //iterate through all Registered Trigger's
                    for (int i = 0; i < act.RegisteredTriggers.Count; i++) DrawActivationField(act, i, false);

                    #region DUMP
                    /*
                    //Iterate through all activator triggers
                    for (int i = 0; i < act.RegisteredTriggers.Count; i++)
                    {
                        #region Format
                        Color darkTint = new(0, 0, 0, 0.05f);
                        Color lightTint = new(1, 1, 1, 0.05f);
                        GUI.backgroundColor = i % 2 == 0 ? darkTint : lightTint;
                        GUIStyle style = new() { normal = { background = Texture2D.whiteTexture } };
                        #endregion

                        GUILayout.BeginVertical(style);
                        EditorGUILayout.Space(3);
                        GUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField((i + 1) + ". ", GUILayout.Width(15f));

                        ActivableBase selection = EditorGUILayout.ObjectField(act.RegisteredTriggers[i].Sender, typeof(ActivableBase), true) as ActivableBase;
                        act.RegisteredTriggers[i].Sender = selection;

                        //If there is an assigned object, check our methods and type
                        if (selection != null)
                        {
                            //Prevent assining actions to be activated from other actions, as that makes no fucking sense
                            if (act.RegisteredTriggers[i].Sender.Type == ActivableType.Action)
                            {
                                act.RegisteredTriggers[i] = null;
                                Debug.LogWarning("Activable Actions should not be activated by other Actions. Reference was removed." +
                                                 "\nTry changing your target큦 Type to " + ActivableType.Trigger + " or " + ActivableType.Both);
                                continue;
                            }

                            //Only include the methods with the activable action attribute, that have either no parameters or take one boolean input
                            MethodInfo[] methods = act.RegisteredTriggers[i].ReceiverMethods;

                            //Notify designers when object has no selectable methods
                            if (methods.Length == 0)
                            {
                                Debug.LogWarning("No suitable methods were found in " + act.RegisteredTriggers[i].Receiver +
                                                 "\nRemember that all selectable methods must use the " + nameof(ActivableAction) + " attribute" +
                                                 "\nand take 1 bool parameter");
                            }
                            else
                            {
                                #region Draw Method Name
                                string[] methodNames = new string[methods.Length];
                                for (int n = 0; n < methods.Length; n++)
                                {
                                    string paramNames = "";
                                    foreach (ParameterInfo parameter in methods[n].GetParameters())
                                    {
                                        paramNames += parameter.ParameterType.Name;
                                    }

                                    methodNames[n] = methods[n].Name + "()";
                                }
                                #endregion

                                //Assign method to call
                                act.RegisteredTriggers[i].SelectedIndex = EditorGUILayout.Popup(act.RegisteredTriggers[i].SelectedIndex, methodNames, GUILayout.MaxWidth(100f));
                                act.RegisteredTriggers[i].CallingMethod = methods[act.RegisteredTriggers[i].SelectedIndex];

                                act.RegisteredTriggers[i].Style = (ActivationStyle)EditorGUILayout.EnumPopup(act.RegisteredTriggers[i].Style, GUILayout.MaxWidth(100f));

                                act.RegisteredTriggers[i].Delay = EditorGUILayout.FloatField(act.RegisteredTriggers[i].Delay, GUILayout.MaxWidth(35f));
                            }

                            //EditorUtility.SetDirty(act.RegisteredTriggers[i].Sender);
                        }

                        if (GUILayout.Button("x", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                        {
                            if (act.RegisteredTriggers[i].Sender != null &&
                                act.RegisteredTriggers[i].Sender.RegisteredActions.Contains(act.RegisteredTriggers[i]))
                                act.RegisteredTriggers[i].Sender.RegisteredActions.Remove(act.RegisteredTriggers[i]);
                            act.RegisteredTriggers.RemoveAt(i);
                        }

                        GUILayout.EndHorizontal();
                        EditorGUILayout.Space(3);
                        GUILayout.EndVertical();
                    }
                    */
                    #endregion
                }
                else //DRAW DEFAULT FIELD
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                }
            }
        }

        EditorUtility.SetDirty(act);

        serializedObject.ApplyModifiedProperties();

        //base.OnInspectorGUI();
    }
}



#endif
