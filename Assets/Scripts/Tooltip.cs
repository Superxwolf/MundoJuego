﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public Text text;
	
	public void SetText(string newText)
    {
        text.text = newText;
    }
}
