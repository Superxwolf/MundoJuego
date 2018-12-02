using UnityEngine;
using System.Collections;

public class FixAspect : MonoBehaviour
{
    public bool fixPosition = true;
    public bool fixSize = false;

    void Start()
    {
        if (fixPosition)
        {
            Vector3 pos = transform.position;
            pos.x *= FixAspectManager.widthScaleMult;
            transform.position = pos;
        }
        if(fixSize)
        {
            Vector3 scale = transform.localScale;
            scale *= FixAspectManager.widthScaleMult;
            transform.localScale = scale;
        }
    }
}
