using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Methods that implement this attribute and take one bool parameter are exposed to Activable Actions and Triggers
/// </summary>

[AttributeUsage(AttributeTargets.Method)]
public class ActivableAction : Attribute
{
    public ActivableAction() { }
}

/// <summary>
/// If used on a field of an Activable script, this field only appears on the specified type
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class OnlyShowFor : Attribute
{
    public Type ActivableClass;

    public OnlyShowFor(Type activableClass)
    {
        ActivableClass = activableClass;
    }
}

/// <summary>
/// If used on a field of an Activable script, it prevents it from being serialized when the Activable's type matches the one specified
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class BelongToActivableType : Attribute
{
    public ActivableType activableType;

    public BelongToActivableType(ActivableType type)
    {
        activableType = type;
    }
}

/// <summary>
/// Fields of an Activable script that implement this ignore the base serialization of the Custom ActivableEditor, so that they can have a customized inspector of their own
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class OverrideBaseActivableEditor : Attribute
{
    public OverrideBaseActivableEditor() { }
}

/// <summary>
/// Fields of an Activable script that implement this are not drawn if the passed Boolean is false
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ExcludeFromActivableEditor : Attribute
{
    public string BooleanName;

    public ExcludeFromActivableEditor(string booleanName) { BooleanName = booleanName; }
}

/// <summary>
/// Draws the field/property ONLY if the compared property compared by the comparison type with the value of comparedValue returns true.
/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfAttribute : PropertyAttribute
{
    #region Fields

    public string comparedPropertyName { get; private set; }
    public object comparedValue { get; private set; }
    public DisablingType disablingType { get; private set; }

    /// <summary>
    /// Types of comperisons.
    /// </summary>
    public enum DisablingType
    {
        ReadOnly = 2,
        DontDraw = 3
    }

    #endregion

    /// <summary>
    /// Only draws the field only if a condition is met. Supports enum and bools.
    /// </summary>
    /// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
    /// <param name="comparedValue">The value the property is being compared to.</param>
    /// <param name="disablingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
    public DrawIfAttribute(string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.DontDraw)
    {
        this.comparedPropertyName = comparedPropertyName;
        this.comparedValue = comparedValue;
        this.disablingType = disablingType;
    }
}

#if UNITY_EDITOR 

public class EditorGUIExtension
{
    public static bool Switch(bool value, params GUILayoutOption[] options)
    {
        return Switch(new GUIContent(), value, options);
    }
    public static bool Switch(string label, bool value, params GUILayoutOption[] options)
    {
        return Switch(new GUIContent(label), value, options);
    }
    public static bool Switch(GUIContent label, bool value, params GUILayoutOption[] options)
    {
        float width = EditorGUIUtility.currentViewWidth;
        float height = EditorGUIUtility.singleLineHeight;
        bool hasLabel = !string.IsNullOrEmpty(label.text);
        foreach (GUILayoutOption option in options)
        {
            string typeString = typeof(GUILayoutOption).GetField("type", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(option).ToString();
            string valueString = typeof(GUILayoutOption).GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(option).ToString();
            float valueFloat = float.Parse(valueString);
            switch (typeString)
            {
                case "fixedWidth":
                    width = valueFloat;
                    break;
                case "fixedHeight":
                    height = valueFloat;
                    break;
            }
        }

        Rect position = EditorGUILayout.GetControlRect(hasLabel, height);
        position.width = width;

        Rect labelFieldRect;
        Rect ButtonBackRect;
        Rect ButtonFrontRect;

        if (hasLabel)
        {
            float labelWidth = EditorGUIUtility.labelWidth < position.width ? EditorGUIUtility.labelWidth : position.width;
            float buttonBackWidth = labelWidth + 50 < position.width ? 50 : position.width - labelWidth;
            float buttonFrontWidth = labelWidth + (buttonBackWidth / 2) < position.width ? (buttonBackWidth / 2) : 0;
            labelFieldRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
            ButtonBackRect = new Rect(position.x + labelWidth + 2, position.y, buttonBackWidth, EditorGUIUtility.singleLineHeight);
            ButtonFrontRect = new Rect((value ? ButtonBackRect.xMax - buttonFrontWidth - 1 : ButtonBackRect.x + 1), ButtonBackRect.y + 1, buttonFrontWidth, ButtonBackRect.height - 2);

            GUI.Label(labelFieldRect, label);
        }
        else
        {
            float buttonBackWidth = 50 < position.width ? 50 : position.width;
            float buttonFrontWidth = buttonBackWidth / 2 < position.width ? buttonBackWidth / 2 - 1 : 0;
            ButtonBackRect = new Rect(position.x, position.y, buttonBackWidth, EditorGUIUtility.singleLineHeight);
            ButtonFrontRect = new Rect((value ? ButtonBackRect.xMax - buttonFrontWidth - 1 : ButtonBackRect.x + 1), ButtonBackRect.y + 1, buttonFrontWidth, ButtonBackRect.height - 2);
        }

        if (GUI.Button(ButtonBackRect, GUIContent.none, "TextField") || GUI.Button(ButtonFrontRect, GUIContent.none))
        {
            value = !value;
        }

        return value;
    }
}

#endif