using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    private void Start()
    {
        if(Screen.height / Screen.width > 16 / 9)
        {
            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        }
        else
        {
            aspectRatioFitter.aspectMode= AspectRatioFitter.AspectMode.WidthControlsHeight;
        }
        aspectRatioFitter.aspectRatio = 0.5f;
    }
}
