using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Faz um RectTranform cobrir a area segura de uma tela de um dispositivo mobile

public class SafeAreaSetter : MonoBehaviour
{
    //Componentes
    private Canvas canvas;
    private RectTransform panelSafeArea;

    //Variaveis
    private Rect currentSafeArea = new Rect();
    private ScreenOrientation currentOrientation = ScreenOrientation.AutoRotation;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        panelSafeArea = GetComponent<RectTransform>();

        currentOrientation = Screen.orientation;
        currentSafeArea = Screen.safeArea;

        ApplySafeArea();
    }

    private void Update()
    {
        if ((currentOrientation != Screen.orientation) || (currentSafeArea != Screen.safeArea))
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        if(panelSafeArea == null)
        {
            Debug.LogError("O objeto nao tem um RectTranform para ser usado!");
            return;
        }

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;

        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        panelSafeArea.anchorMin = anchorMin;
        panelSafeArea.anchorMax = anchorMax;

        currentOrientation = Screen.orientation;
        currentSafeArea = Screen.safeArea;
    }
}