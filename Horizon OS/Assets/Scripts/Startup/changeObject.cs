using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject introScreen;
    public CanvasGroup instructionCanvasGroup;

    void Start()
    {
        // Set the alpha of the instruction canvas to 0 at the start
        instructionCanvasGroup.alpha = 0f;

        // Deactivate the instructionCanvas at the start
        instructionCanvasGroup.gameObject.SetActive(false);

        // Start playing the video
        videoPlayer.Play();

        // Call the function to check the video status
        StartCoroutine(CheckVideoStatus());
    }

    IEnumerator CheckVideoStatus()
    {
        // Wait until the video starts playing
        while (!videoPlayer.isPlaying)
        {
            yield return null;
        }

        // Video is playing, deactivate the instructionCanvas
        instructionCanvasGroup.gameObject.SetActive(false);

        // Wait until the video finishes playing
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        // Video has finished, fade in the instructionCanvas
        introScreen.SetActive(false);
        yield return new WaitForSeconds(2f); // Add a 2-second delay here
        instructionCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(FadeInCanvas());
    }

    IEnumerator FadeInCanvas()
    {
        // Gradually increase the alpha of the canvas
        while (instructionCanvasGroup.alpha < 1f)
        {
            instructionCanvasGroup.alpha += Time.deltaTime * 0.5f; // Adjust the speed of the fade here
            yield return null;
        }
    }
}
