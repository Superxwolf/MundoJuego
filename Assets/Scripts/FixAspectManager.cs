using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAspectManager
{
    // Scale all object by this.
    public static float widthScaleMult = 1f;
    public static float heightScaleMult = 1f;

    // Screen size by Unity's units
    public static Vector2 screenSize = new Vector2(10f, 5.863f);

    [RuntimeInitializeOnLoadMethod]
    static void StaticInit()
    {
        //Hard coded target aspect as target
        float targetAspect = 1024f/ 600f;
        float windowAspect = Screen.width / (float)Screen.height;
        widthScaleMult = windowAspect / targetAspect;

        float screenHeight = Camera.main.orthographicSize * 2;
        screenSize = new Vector2(screenHeight / Screen.height * Screen.width, screenHeight);
    }
}
