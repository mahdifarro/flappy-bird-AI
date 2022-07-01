using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideDetector : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _GameManager.Instance.HitObject(collision.collider.tag);
    }
}
