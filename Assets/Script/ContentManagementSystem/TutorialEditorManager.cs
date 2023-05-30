using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialEditorManager : MonoBehaviour
{
    // Input Fields from Tutorial Manager Panel
    public TMP_InputField titleInputField;
    public TMP_InputField tabInputField;
    public TMP_InputField textContentInputField;

    // Text Objects from Preview Tutorial Panel
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI tabText;
    public TextMeshProUGUI textContentText;

    void Start()
    {
        titleInputField.onValueChanged.AddListener(OnTitleChanged);
        tabInputField.onValueChanged.AddListener(OnTabChanged);
        textContentInputField.onValueChanged.AddListener(OnTextContentChanged);
    }

    public void OnTitleChanged(string newValue)
    {
        titleText.text = newValue;
    }

    public void OnTabChanged(string newValue)
    {
        tabText.text = newValue;
    }

    public void OnTextContentChanged(string newValue)
    {
        textContentText.text = newValue;
    }

}
