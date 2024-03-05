using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// public class camera_controller : MonoBehaviour
// {
//     private bool camtersedia = false;
//     private WebCamTexture backCam;
//     private Texture defaultBackground;

//     public RawImage background;
//     public AspectRatioFitter fit;

//     // Start is called before the first frame update
//     void Start()
//     {
//         defaultBackground = background.texture;
//         WebCamDevice[] devices = WebCamTexture.devices;

//         if (devices.Length == 0)
//         {
//             Debug.Log("Tidak ada kamera yang tersedia");
//             camtersedia = false;
//             return;
//         }

//         for (int i = 0; i < devices.Length; i++)
//         {
//             if (!devices[i].isFrontFacing)
//             {
//                 backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
//             }
//         }

//         if (backCam == null)
//         {
//             Debug.Log("Tidak ada kamera belakang yang tersedia");
//             return;
//         }

//         backCam.Play();
//         background.texture = backCam;

//         camtersedia = true;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (!camtersedia)
//             return;

//         float ratio = (float)backCam.width / (float)backCam.height;
//         fit.aspectRatio = ratio;

//         float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
//         background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

//         int orient = -backCam.videoRotationAngle;
//         background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
//     }
// }

public class CameraController : MonoBehaviour
{
    private bool camTersedia = false;
    private WebCamTexture backCam;
    private WebCamTexture frontCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    void Start()
    {
        defaultBackground = background.texture;
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

    void Update()
    {
        if (!camTersedia)
            return;

        if (backCam != null)
        {
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
