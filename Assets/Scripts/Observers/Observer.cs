using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    // Pipes that are in the observer environment
    public List<GameObject> pipesList;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            GameObject parent = collision.transform.parent.gameObject;
            if (!IsParentFound(parent))
            {
                pipesList.Add(parent);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Obstacle") && pipesList.Count!=0)
        {
            GameObject parent = collision.transform.parent.gameObject;
            if (IsParentFound(parent))
            {
                pipesList.Remove(parent);
            }
        }
    }

    private bool IsParentFound(GameObject parent)
    {
        if (pipesList.Contains(parent))
        {
            return true;
        }
        else { return false; }
    }
}
