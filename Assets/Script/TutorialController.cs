using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    public Transform dialogBox;  // The 3D dialog box
    public TextMeshPro title;
    public TextMeshPro tab;
    public TextMeshPro textContent;
    public Renderer image;  // The Renderer component of a Plane/Sphere/etc. to show the image
    public VideoPlayer videoPlayer;
    public GameObject qrCode; // The QR code object

    private PageDataList tutorialData; // Updated from TutorialData to PageDataList
    private int currentPageIndex = 0;

    public Button3D nextPageButton;
    public Button3D previousPageButton;
    public Button3D quitButton;

    private IEnumerator Start()
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "casamento e cachorro.json");
        UnityWebRequest request = UnityWebRequest.Get(jsonFilePath);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string jsonString = request.downloadHandler.text;
            Debug.Log("JSON String: " + jsonString); // Add this line to log the JSON string.
            tutorialData = JsonUtility.FromJson<PageDataList>(jsonString); // Updated from TutorialData to PageDataList
            LoadPage(currentPageIndex);
        }

        nextPageButton.OnClick += NextPage;
        previousPageButton.OnClick += PreviousPage;
        quitButton.OnClick += QuitTutorial;
    }

    private IEnumerator LoadImage(string imagePath, Renderer outputImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            outputImage.material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    private void LoadPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < tutorialData.Pages.Count)
        {
            PageData page = tutorialData.Pages[pageIndex]; // Updated from tutorialData.pages to tutorialData.Pages
            title.text = page.title;
            tab.text = page.tab;
            textContent.text = page.textContent;

            string imagePath = Path.Combine(Application.streamingAssetsPath, page.imagePath);
            StartCoroutine(LoadImage(imagePath, image));

            // Load video from video path
            if (!string.IsNullOrEmpty(page.videoPath))
            {
                string videoPath = Path.Combine(Application.streamingAssetsPath, page.videoPath);
                videoPlayer.url = videoPath;
                videoPlayer.Prepare();
            }
            // Apply the positional offset
            Vector3 basePosition = qrCode.transform.position; // get QR code position here
            dialogBox.transform.position = basePosition + new Vector3(page.positionalOffset.xOffset, page.positionalOffset.yOffset, page.positionalOffset.zOffset);
        }
        else
        {
            Debug.LogError("Invalid page index: " + pageIndex);
        }
    }

    public void NextPage(Button3D button)
    {
        if (currentPageIndex < tutorialData.Pages.Count - 1) // Updated from tutorialData.pages.Length to tutorialData.Pages.Count
        {
            currentPageIndex++;
            LoadPage(currentPageIndex);
        }
    }

    public void PreviousPage(Button3D button)
    {     
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            LoadPage(currentPageIndex);
        }        
    }
    public void QuitTutorial(Button3D button)
    {   
        Application.Quit();
            Debug.Log("Quit Application");
    }
}