using UnityEngine;
using UnityEngine.UI;
using Python.Runtime;
using System.IO;

public class WebCam : MonoBehaviour
{
    WebCamTexture webcam;
    public RawImage img;
    PyObject pythonScript;

    void Start()
    {
        // Set the path to the Python DLL after initializing Python
        // Runtime.PythonDLL = @"C:\Users\revor\AppData\Local\Programs\Python\Python311\python311.dll";
        Runtime.PythonDLL = @"/usr/lib/x86_64-linux-gnu/libpython3.10.so";
        // Initialize Python must be done first
        PythonEngine.Initialize();

        // Initialize WebCamTexture
        webcam = new WebCamTexture();
        img.texture = webcam;

        // Load the Python script
        using (Py.GIL())
        {
            pythonScript = Py.Import("CameraTracker");
        }

        // Start playing WebCamTexture
        webcam.Play();
    }

    void Update()
    {
        // Capture frame from camera
        Color32[] pixels = new Color32[webcam.width * webcam.height];
        webcam.GetPixels32(pixels);
        int width = webcam.width;
        int height = webcam.height;

        // Convert Color32 array to byte array representing RGB image data
        byte[] frameBytes = new byte[width * height * 3];
        for (int i = 0; i < pixels.Length; i++)
        {
            frameBytes[i * 3] = pixels[i].r;
            frameBytes[i * 3 + 1] = pixels[i].g;
            frameBytes[i * 3 + 2] = pixels[i].b;
        }

        // Send frame bytes to Python
        using (Py.GIL())
        {
            // Invoke the track_hand method in the Python script
            var result = pythonScript.InvokeMethod("track_hand", frameBytes.ToPython(), width.ToPython(), height.ToPython());
            // Convert the result to a string
            string resultString = result.ToString();
            // Split the string by the delimiter
            Debug.Log(resultString);
        }
    }
}
