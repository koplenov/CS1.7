
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AlphaImage : MonoBehaviour
{
    public Image image;

    private void Start()
    {
        image.alphaHitTestMinimumThreshold = 0.001f;
    }


    }

    
