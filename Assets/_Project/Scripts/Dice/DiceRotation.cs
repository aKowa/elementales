using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class DiceRotation : SerializedMonoBehaviour
{
    //Componentes
    [Header("Components")]
    [SerializeField] private Camera diceRotationCamera;
    [SerializeField] private RenderTexture renderTexture;

    [Header("Options")]
    [OdinSerialize] private Dictionary<DiceType, GameObject> diceDictionary;
    [SerializeField] private Transform rotationParent;
    private Dictionary<string, Dictionary<DiceType, DiceMaterialAndSprite>> DiceArtDictionary;

    //Getters
    public RenderTexture RenderTexture => renderTexture;
    public Dictionary<DiceType, GameObject> DiceDictionary => diceDictionary;

    private void Awake()
    {
        DiceArtDictionary = GlobalSettings.Instance.DiceSettings.DiceArtDictionary;

        renderTexture = Instantiate(renderTexture);

        diceRotationCamera.targetTexture = renderTexture;
    }

    [Button]
    public void ChooseDiceToShow(DiceType diceTypeParam)
    {
        foreach (DiceType diceType in diceDictionary.Keys)
        {
            diceDictionary[diceType].SetActive(diceType == diceTypeParam);
        }
    }

    [Button]
    public void ChooseDiceTexture(MeshRenderer diceMesh, string materialName)
    {
        foreach (DiceType diceType in diceDictionary.Keys)
        {
            if (diceDictionary[diceType].activeSelf)
            {
                Material material = DiceArtDictionary[materialName][diceType].material;
                diceMesh.material = material;
            }
        }
    }

    private void Update()
    {
        rotationParent.RotateAround(rotationParent.position, rotationParent.up, Time.unscaledDeltaTime * 45f);
        rotationParent.RotateAround(rotationParent.position, rotationParent.forward, Time.unscaledDeltaTime * 45f);
    }
}