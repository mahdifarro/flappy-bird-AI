using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectTweener : MonoBehaviour
{
    private void Awake()
    {
        DOTween.Init();
    }

    public static void MoveObject(Transform gameObjectTransform, Vector3 start, Vector3 finish, float time)
    {
        gameObjectTransform.gameObject.SetActive(true);
        gameObjectTransform.localPosition = start;
        gameObjectTransform.DOMove(finish, time).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                gameObjectTransform.position = start;
                gameObjectTransform.gameObject.SetActive(false);
            });
    }

    public static void StopTweens()
    {
        DOTween.Clear(true);
    }
}