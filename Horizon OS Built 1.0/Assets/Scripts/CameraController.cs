using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Python.Runtime;

using System;
using System.Runtime.InteropServices;

// using Microsoft.CSharp.RuntimeBinder;

public class CameraController : MonoBehaviour
{
    private bool camTersedia = false;
    private WebCamTexture backCam;
    private WebCamTexture frontCam;
    private dynamic handTracking; // Menyimpan instance dari kelas HandTracking

    public RawImage background;
    public AspectRatioFitter fit;

    public dynamic pythonScript;
    public dynamic setCamera;

    void Start()
    {
        string pythonDllPath = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            pythonDllPath = @"C:\path\to\python38.dll"; // Sesuaikan dengan jalur Python DLL di Windows
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            pythonDllPath = @"/usr/lib/x86_64-linux-gnu/libpython3.10.so"; // Sesuaikan dengan jalur Python DLL di Linux
        }
        else
        {
            throw new PlatformNotSupportedException("Sistem operasi tidak didukung.");
        }

        Runtime.PythonDLL = pythonDllPath;
        PythonEngine.Initialize();

        // Agar Python.Runtime dapat mengenali scripts yang ada di Assets Unity
        // Ref: https://stackoverflow.com/questions/4712665/python-for-net-importerror-no-module-named-warnings
        PythonEngine.PythonPath =  PythonEngine.PythonPath + ";" + "/home/zuruz/Documents/Projects"; 

        Debug.Log("HALOOOOOO");
        Debug.Log(PythonEngine.PythonPath);

        using (Py.GIL())
        {
            PyObject pyBackCam = backCam.ToPython();
            PyObject pyWidth = Screen.width.ToPython();
            PyObject pyHeight = Screen.height.ToPython();

            pythonScript = Py.Import("CameraTracker");
            setCamera = pythonScript.InvokeMethod("set_camera", new PyObject[] {pyBackCam, pyWidth, pyHeight});

            // dynamic HandTrackingClass = pythonScript.GetAttr("HandTracking");

            // handTracking = HandTrackingClass(backCam, Screen.width, Screen.height);
            // handTracking.set_camera();

            // Buat instance dari kelas HandTracking
            // handTracking = pythonScript.GetAttr("HandTracking").Invoke(pyBackCam, pyWidth, pyHeight);
            // Debug.Log(handTracking);

            // Panggil metode set_camera dari instance kelas HandTracking
            // handTracking.set_camera();

            WebCamDevice[] devices = WebCamTexture.devices;

            if (devices.Length == 0)
            {
                Debug.Log("Tidak ada kamera yang tersedia");
                camTersedia = false;
                return;
            }

            for (int i = 0; i < devices.Length; i++)
            {
                Debug.Log("Device Name: " + devices[i].name + ", Is Front Facing: " + devices[i].isFrontFacing);
                if (!devices[i].isFrontFacing)
                {
                    backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                }
                else
                {
                    frontCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                }
            }

            if (backCam != null)
            {
                backCam.Play();
                background.texture = backCam;
                camTersedia = true;
            }
            else if (frontCam != null)
            {
                frontCam.Play();
                background.texture = frontCam;
                camTersedia = true;
            }
            else
            {
                Debug.Log("Tidak ada kamera yang tersedia");
                camTersedia = false;
            }
        }
    }

    void Update()
    {
        using (Py.GIL())
        {
            if (!camTersedia)
                return;

            if (backCam != null)
            {
                // Debug.Log(handTracking.trackHand());
                Debug.Log(pythonScript.invokeMethod("track_hand"));

                float ratio = (float)backCam.width / (float)backCam.height;
                fit.aspectRatio = ratio;

                float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
                background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

                int orient = -backCam.videoRotationAngle;
                background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
            }
            else if (frontCam != null)
            {
                float ratio = (float)frontCam.width / (float)frontCam.height;
                fit.aspectRatio = ratio;

                float scaleY = frontCam.videoVerticallyMirrored ? -1f : 1f;
                background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

                int orient = -frontCam.videoRotationAngle;
                background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
            }
        }
    }
}
