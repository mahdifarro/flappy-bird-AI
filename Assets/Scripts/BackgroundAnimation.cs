using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    public float animationSpeed=0.1f;
    private Renderer backgroundTenderer;

    // Start is called before the first frame update
    void Start()
    {
        backgroundTenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        backgroundTenderer.material.mainTextureOffset= new Vector2(Time.time*animationSpeed, 0);
    }
}
