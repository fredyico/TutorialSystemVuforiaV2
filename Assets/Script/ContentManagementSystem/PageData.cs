using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PositionalOffset
{
    public float xOffset;
    public float yOffset;
    public float zOffset;
}

[Serializable]
public class PageData
{
    public string title;
    public string tab;
    public string textContent;
    public string imagePath;
    public string videoPath;
    public PositionalOffset positionalOffset; // Add this line
}


