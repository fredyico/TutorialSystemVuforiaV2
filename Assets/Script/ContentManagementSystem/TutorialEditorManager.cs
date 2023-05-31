using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using SimpleFileBrowser; // File Browser namespace
using UnityEngine.Video;

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

    // Raw Image and Video Player Objects from Preview Tutorial Panel
    public RawImage previewImage;
    public VideoPlayer previewVideoPlayer;

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
    public void OnImageButtonPressed()
    {
        StartCoroutine(ShowLoadImageDialogCoroutine());
    }
    public void OnVideoButtonPressed()
    {
        StartCoroutine(ShowLoadVideoDialogCoroutine());
    }

    private IEnumerator ShowLoadImageDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: FileBrowser.ShowLoadDialog( OnSuccess, OnCancel )
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files);

        // FileBrowser.Success returns true when a file/folder is picked
        // If so, output the file/folder path
        if (FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0];

            // Load the image into a texture
            byte[] imageBytes = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);

            // Apply the texture to the preview image
            previewImage.texture = tex;
        }
    }
    private IEnumerator ShowLoadVideoDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files);

        if (FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0];

            // Load the video from the file path
            previewVideoPlayer.url = filePath;
            previewVideoPlayer.Prepare();
            previewVideoPlayer.Play();
        }
    }
}
