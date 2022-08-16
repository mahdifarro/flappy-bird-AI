using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideDetector : MonoBehaviour
{
    private AiBirdManager aiBirdManager;

    private void Awake()
    {
        aiBirdManager = GetComponent<AiBirdManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        aiBirdManager.HitObject(collision.collider.tag);
    }
}
