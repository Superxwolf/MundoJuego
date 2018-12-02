using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceFixAspect : MonoBehaviour
{
    public bool scaleXPos, scaleYPos, scaleXScale, scaleYScale;
    public bool fixPosition = true;
    public bool fixSize = false;
    public float minScale = 0f, maxScale = 2f;

	// Use this for initialization
	void Start ()
    {
        var trueScale = Mathf.Max(FixAspectManager.widthScaleMult, minScale);
        trueScale = Mathf.Min(FixAspectManager.widthScaleMult, maxScale);

        if (fixPosition)
        {
            Vector3 pos = transform.position;
            if(scaleXPos) pos.x *= trueScale;
            if (scaleYPos) pos.y *= trueScale;
            transform.position = pos;
        }
        if (fixSize)
        {
            Vector3 scale = transform.localScale;
            if (scaleXScale) scale.x *= trueScale;
            if (scaleYScale) scale.y *= trueScale;
            transform.localScale = scale;
        }
    }
}
