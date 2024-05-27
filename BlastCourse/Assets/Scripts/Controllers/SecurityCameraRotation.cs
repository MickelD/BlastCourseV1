using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SecurityCameraRotation : MonoBehaviour
{
    [Space(5), Header("Transforms"), Space(2)]
    [SerializeField] Transform xRotTransform;
    [SerializeField] Transform yRotTransform;

    [Space(5), Header("Rotations"), Space(2)]
    [SerializeField] Vector2 xRotRange;
    [SerializeField] Vector2 yRotRange;

    [Space(5), Header("Intervals"), Space(2)]
    [SerializeField] Vector2 rotTime;
    [SerializeField] Vector2 waitTime;

    private void OnEnable()
    {
        StartCoroutine(RandomAim());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator RandomAim()
    {
        while (isActiveAndEnabled)
        {
            float rotSpeed = Random.Range(rotTime.x, rotTime.y);

            xRotTransform.DOLocalRotate(new Vector3(0f, Random.Range(xRotRange.x, xRotRange.y), 0f), rotSpeed);
            yield return yRotTransform.DOLocalRotate(new Vector3(Random.Range(yRotRange.x, yRotRange.y), 0f, 0f), rotSpeed).WaitForCompletion();

            yield return new WaitForSeconds(Random.Range(waitTime.x, waitTime.y));
        }
    }
}


