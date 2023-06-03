using System.Collections;
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
    public GameObject videoPlayerObject;
    private string imagePath;
    private string videoPath;

    // Raw Image and Video Player Objects from Preview Tutorial Panel
    public RawImage previewImage;
    public VideoPlayer previewVideoPlayer;

    public TutorialManager tutorialManager; //object to access the list of PageData
    private int currentPageIndex = 0; //variable to track the current page

    public GameObject qrCode;
    public GameObject dialogBox;

    void Start()
    {
        titleInputField.onValueChanged.AddListener(OnTitleChanged);
        tabInputField.onValueChanged.AddListener(OnTabChanged);
        textContentInputField.onValueChanged.AddListener(OnTextContentChanged);
    }
    //buttons method
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
    public void OnDeletePageButtonPressed()
    {
        if (tutorialManager.Pages.Count > 0)
        {
            tutorialManager.Pages.RemoveAt(currentPageIndex);

            // If the current page index is greater than the last index in the Pages list, decrease it by one
            if (currentPageIndex >= tutorialManager.Pages.Count)
            {
                currentPageIndex--;
            }

            // If there are any pages left, load the current page
            if (tutorialManager.Pages.Count > 0)
            {
                LoadPageData();
            }
            else // If no pages left, clear the fields
            {
                ClearFields();
            }
        }
    }
    private void ClearFields()
    {
        titleInputField.text = "";
        tabInputField.text = "";
        textContentInputField.text = "";
        previewImage.texture = null;
        previewVideoPlayer.url = "";
        // Make sure to stop the video player if it was playing a video
        if (previewVideoPlayer.isPlaying)
        {
            previewVideoPlayer.Stop();
        }
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
            imagePath = FileBrowser.Result[0];  // Store the selected image path

            // Load the image into a texture
            byte[] imageBytes = File.ReadAllBytes(imagePath);
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
            videoPath = FileBrowser.Result[0];  // Store the selected video path

            // Load the video from the file path
            previewVideoPlayer.url = videoPath;
            previewVideoPlayer.Prepare();
            previewVideoPlayer.Play();
        }
    }
    //load tutorial dialog box
    private IEnumerator ShowLoadTutorialDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, "Load Tutorial", "Load");

        // FileBrowser.Success returns true when a file/folder is picked
        // If so, output the file/folder path
        if (FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0];

            // Load the JSON string from the file
            string jsonData = File.ReadAllText(filePath);
            PageDataList pageDataList = JsonUtility.FromJson<PageDataList>(jsonData);
            tutorialManager.Pages = pageDataList.Pages;

            // Load the first page
            currentPageIndex = 0;
            LoadPageData();
        }
    }
    //save tutorial dialog box
    private IEnumerator ShowSaveTutorialDialogCoroutine()
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, null, "Save Tutorial", "Save");

        // FileBrowser.Success returns true when a file/folder is picked
        // If so, output the file/folder path
        if (FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0];

            PageDataList pageDataList = new PageDataList();
            pageDataList.Pages = tutorialManager.Pages;

            string jsonData = JsonUtility.ToJson(pageDataList);
            File.WriteAllText(filePath, jsonData);
        }
    }
    public void OnSavePageButtonPressed()
    {
        PageData currentPageData = new PageData
        {
            title = titleInputField.text,
            tab = tabInputField.text,
            textContent = textContentInputField.text,
            imagePath = imagePath,    // Set these fields when an image or video is selected
            videoPath = videoPath     // Set these fields when an image or video is selected
        };

        tutorialManager.Pages.Add(currentPageData);
        Debug.Log(currentPageData); Debug.Log("the page is being saved" + currentPageData);
    }
    //method to access the next page of the tutorial's list. It will check if it is the last page.
    public void OnNextPageButtonPressed()
    {
        if (currentPageIndex < tutorialManager.Pages.Count - 1)
        {
            currentPageIndex++;
            LoadPageData();
            if (previewVideoPlayer.isPlaying)
            {
                previewVideoPlayer.Stop();
            }
        }
    }
    public void OnPreviousPageButtonPressed()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            LoadPageData();
            if (previewVideoPlayer.isPlaying)
            {
                previewVideoPlayer.Stop();
            }
        }
    }
    private void LoadImageFromPath(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            // Load the image into a texture
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);

            // Apply the texture to the preview image
            previewImage.texture = tex;
        }
    }
    private void LoadVideoFromPath(string videoPath)
    {
        if (File.Exists(videoPath))
        {       
            // Load the video from the file path
            previewVideoPlayer.url = videoPath;
            previewVideoPlayer.Prepare();
            previewVideoPlayer.Play();            
        }
    }
    private void LoadPageData()
    {
        // Get the current page data
        PageData currentPageData = tutorialManager.Pages[currentPageIndex];

        // Update the UI elements
        titleInputField.text = currentPageData.title;
        tabInputField.text = currentPageData.tab;
        textContentInputField.text = currentPageData.textContent;

        // Load image and video from their paths
        LoadImageFromPath(currentPageData.imagePath);
        if (string.IsNullOrEmpty(currentPageData.videoPath) || !File.Exists(currentPageData.videoPath))
        {
            // If the current page does not have a video, reset the VideoPlayer and disable the RawImage
            previewVideoPlayer.clip = null;
            videoPlayerObject.SetActive(false);
        }
        else
        {
            videoPlayerObject.SetActive(true);
            LoadVideoFromPath(currentPageData.videoPath);
        }

        // Apply the positional offset
        Vector3 basePosition = qrCode.transform.position;
        dialogBox.transform.position = basePosition + new Vector3(currentPageData.positionalOffset.xOffset, currentPageData.positionalOffset.yOffset, currentPageData.positionalOffset.zOffset);
    }

    //load tutorial button
    public void OnLoadTutorialButtonPressed()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: FileBrowser.ShowLoadDialog( OnSuccess, OnCancel )
        StartCoroutine(ShowLoadTutorialDialogCoroutine());
    }

    public void OnSaveTutorialButtonPressed()
    {
        // Show a save file dialog and wait for a response from user
        // Load file/folder: FileBrowser.ShowSaveDialog( OnSuccess, OnCancel )
        StartCoroutine(ShowSaveTutorialDialogCoroutine());
    }

    public void LoadTutorial(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        PageDataList pageDataList = JsonUtility.FromJson<PageDataList>(jsonData);

        tutorialManager.Pages = pageDataList.Pages;

        // Load the first page
        currentPageIndex = 0;
        LoadPageData();
    }

}
