using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DestructibleObject : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Values"), Space(3)]
    [Range(1,5), Tooltip("Explosions the object can resist before breaking")] public int ExplosionCount = 1;
    public GameObject InstantiatedParticles;
    public float ParticlesLifeTime;
    public bool UseAnimator;
    public AnimatedDestructionParameters AnimatorParameters;

    [Space(5), Header("Audio"), Space(3)]
    public AudioCue DamageSfx;
    public AudioCue DestructionSfx;

    #endregion

    #region Variables

    private int _explosions = 0;

    #endregion

    #region Methods
    public void AddDamage()
    {
        _explosions++;

        if (_explosions >= ExplosionCount)
        {
            Break();
        }
        else
        {
            AudioManager.TryPlayCueAtPoint(DamageSfx, transform.position);

            //we have an animator we want to use and a parameter name for advancing destruction stages
            if (UseAnimator && AnimatorParameters.DestructionAnimator != null && !string.IsNullOrWhiteSpace(AnimatorParameters.StagesParameterName))
            {
                AnimatorParameters.DestructionAnimator.SetInteger(AnimatorParameters.StagesParameterName, _explosions);
            }
        }
    }

    public void Break()
    {
        AudioManager.TryPlayCueAtPoint(DestructionSfx, transform.position);

        if (InstantiatedParticles != null) Destroy(Instantiate(InstantiatedParticles, transform.position, Quaternion.identity), ParticlesLifeTime);

        if (UseAnimator && AnimatorParameters.DestructionAnimator != null && !string.IsNullOrWhiteSpace(AnimatorParameters.DestructionTriggerName))
        {
            AnimatorParameters.DestructionAnimator.SetTrigger(AnimatorParameters.DestructionTriggerName);
        }

        if (GetComponentInChildren<PlayerMovement>() != null)
        {
            GetComponentInChildren<PlayerMovement>().transform.parent = null;
        }
        Destroy(gameObject);
    }

    #endregion
}

[System.Serializable]
public class AnimatedDestructionParameters
{
    [Space(3)]
    [Tooltip("Animator that handles destruction animations")] public Animator DestructionAnimator;
    [Tooltip("If the object can resist multiple explosions, animator parameter to adjust")] public string StagesParameterName;
    [Tooltip("Trigger destruction animation")] public string DestructionTriggerName;

}


#if UNITY_EDITOR

[CustomEditor(typeof(DestructibleObject))]
public class DestructibleObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DestructibleObject obj = (DestructibleObject)target;
        if (obj == null) return;

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(obj.ExplosionCount)));

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(obj.InstantiatedParticles)));
        if (obj.InstantiatedParticles != null)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(obj.ParticlesLifeTime)));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(obj.UseAnimator)));
        if(obj.UseAnimator)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(obj.AnimatorParameters)));
        }

        if (obj.ExplosionCount > 1)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(obj.DamageSfx)));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(obj.DestructionSfx)));

        serializedObject.ApplyModifiedProperties();
    }
}

#endif

