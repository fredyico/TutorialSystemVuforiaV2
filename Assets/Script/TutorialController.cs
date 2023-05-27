using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class Page
{
    public string title;
    public string tab;
    public string textContent;
    public string imagePath;
    public string videoPath;
}

[System.Serializable]
public class TutorialData
{
    public Page[] pages;
}

public class TutorialController : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text tab;
    public TMP_Text textContent;
    public RawImage image;
    public VideoPlayer videoPlayer;
    private TutorialData tutorialData;
    private int currentPageIndex = 0;

    /*
    private void Start()
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "tutorialData.json");
        string jsonString = File.ReadAllText(jsonFilePath);
        tutorialData = JsonUtility.FromJson<TutorialData>(jsonString);
        LoadPage(currentPageIndex);
    }
    */

    private IEnumerator Start()
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "tutorialData.json");
        UnityWebRequest request = UnityWebRequest.Get(jsonFilePath);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string jsonString = request.downloadHandler.text;
            tutorialData = JsonUtility.FromJson<TutorialData>(jsonString);
            LoadPage(currentPageIndex);
        }
    }
    private IEnumerator LoadImage(string imagePath, RawImage outputImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            outputImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }


    private void LoadPage(int pageIndex)
    {
        Page page = tutorialData.pages[pageIndex];
        title.text = page.title;
        tab.text = page.tab;
        textContent.text = page.textContent;

        string imagePath = Path.Combine(Application.streamingAssetsPath, page.imagePath);
        StartCoroutine(LoadImage(imagePath, image));
    }

    public void NextPage()
    {
        if (currentPageIndex < tutorialData.pages.Length - 1)
        {
            currentPageIndex++;
            LoadPage(currentPageIndex);
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            LoadPage(currentPageIndex);
        }
    }

    // TODO: Add function for quit button
    public void QuitTutorial()
    {
        // Whatever you want to do when quit is clicked. To close the application:
        Application.Quit();
    }
}

