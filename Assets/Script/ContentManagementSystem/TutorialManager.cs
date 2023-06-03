using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class PageDataList
{
    public List<PageData> Pages;
}
public class TutorialManager : MonoBehaviour
{
    public List<PageData> Pages = new List<PageData>();
}


