using UnityEngine;
using System.IO;

public class ScreenshotCapture : MonoBehaviour
{
    public Rect buttonRect = new Rect(10, 10, 150, 50);
    public string folderName = "Screenshots";

    private void Start()
    {
        string folderPath = Path.Combine(Application.dataPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(buttonRect, "Take Screenshot"))
        {
            TakeScreenshot();
        }
    }

    [ContextMenu("Take Screenshot")]
    public void TakeScreenshot()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = "screenshot_" + timestamp + ".png";
        string folderPath = Path.Combine(Application.dataPath, folderName);
        string filePath = Path.Combine(folderPath, fileName);

        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot saved to: " + filePath);
    }
}