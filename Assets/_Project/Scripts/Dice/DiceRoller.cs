using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceRoller : MonoBehaviour
{
    [SerializeField] private int throwDistance;
    [SerializeField] private int yDistance;
    [SerializeField] private int force;
    [SerializeField] private bool debug;
    [SerializeField] private Transform diceInstancesParent;
    [SerializeField] private RectTransform diceIconPrefab;
    [SerializeField] private List<Collider> diceWalls;
    [SerializeField] private DiceAreaCenter diceArea;
    [SerializeField] private Collider deadzone;
    [SerializeField] private Transform diceRespawnLocation;

    private Dictionary<DiceType, DiceTypePrefabAndSprite> dicePrefabDictionary;

    private DeterministicDiceRoller deterministicDiceRoller;
    private Dictionary<string,Dictionary<DiceType,DiceMaterialAndSprite>> diceArtDictionary;

    public void Initialize()
    {
        deterministicDiceRoller = GetComponent<DeterministicDiceRoller>();
        dicePrefabDictionary = GlobalSettings.Instance.DiceSettings.DicePrefabDictionary;
        diceArtDictionary = GlobalSettings.Instance.DiceSettings.DiceArtDictionary;
    }

    public List<GameObject> GenerateDices(List<DiceType> monsterDices, string diceMaterial, Vector3 dicePos)
    {
        if (String.IsNullOrEmpty(diceMaterial))
            diceMaterial = "Default";
        
        List<GameObject> diceInstances = new List<GameObject>();
        for (var i = 0; i < monsterDices.Count; i++)
        {
            DiceType dice = monsterDices[i];
            Vector3 positionWithOffset = new Vector3(dicePos.x - (2f * i), dicePos.y, dicePos.z);
            GameObject diceClone = Instantiate(dicePrefabDictionary[dice].prefab, positionWithOffset, Quaternion.identity,
                diceInstancesParent);
            diceClone.GetComponentInChildren<MeshRenderer>().material = diceArtDictionary[diceMaterial][dice].material;
            diceInstances.Add(diceClone);
            diceInstances.ForEach(dI => dI.GetComponent<Rigidbody>().isKinematic = true);
            diceWalls.ForEach(w => Physics.IgnoreCollision(diceClone.GetComponent<Collider>(), w, true));
        }

        return diceInstances;
    }

    public void InitializeDiceRoller() => deterministicDiceRoller.Initialize();
    public void ClearDiceResults() => deterministicDiceRoller.wantedRoll.Clear();
    public void ResetDicePositions() => deterministicDiceRoller.RestoreAllPositions();
    public void AddToDicesList(List<GameObject> dices) => deterministicDiceRoller.AddToDicesList(dices);
    public void AddToAvoidList(List<GameObject> dices) => deterministicDiceRoller.AddToAvoidList(dices);
    public void AddToDestructionList(List<GameObject> dices) => deterministicDiceRoller.AddToDestructionList(dices);
    
    public void DestroyDices(List<GameObject> dices)
    {
        
        if (dices.IsNullOrEmpty() == false)
        {
            AddToDestructionList(dices);
            dices.ForEach(Destroy);
        }
    }

    public void CleanDicesListAndPositions()
    {
        deterministicDiceRoller.dicesList.Clear();
        deterministicDiceRoller.StoreDicesInList();
        deterministicDiceRoller.ClearAndStorePositions();
    }

    public void SendDiceResultsToWantedRolls(List<int> results) => deterministicDiceRoller.wantedRoll.AddRange(results);

    public void RollDice()
    {
        //Debug.Log("Rodei os dados");

        diceWalls.ForEach(w => diceArea.ResetWallColliders());
        deterministicDiceRoller.forceDirection = diceArea.transform.position;
        deterministicDiceRoller.throwDistance = throwDistance;
        deterministicDiceRoller.forceMultiplier = force;
        deterministicDiceRoller.rollDice = true;
    }

    public void AssignStopDiceAction(Action onDiceStopped) => deterministicDiceRoller.AssignStopDiceAction(onDiceStopped);

    public void AddDiceIcon(DiceType diceType, int diceResult, string diceMaterial, RectTransform iconsHolder)
    {
        if (String.IsNullOrEmpty(diceMaterial))
            diceMaterial = "Default";
        
        RectTransform diceIconClone = Instantiate(diceIconPrefab, iconsHolder);
        diceIconClone.GetComponentInChildren<TextMeshProUGUI>().text = diceResult.ToString();
        Image image = diceIconClone.GetComponent<Image>();
        image.sprite = dicePrefabDictionary[diceType].sprite;
        image.color = diceArtDictionary[diceMaterial][diceType].color;
    }

    public void CleanVariables() => deterministicDiceRoller?.ClearVariablesAndStates();

    public void CleanVariablesAndDestroyLeftoverDice() => deterministicDiceRoller?.ClearVariablesAndStatesAndDestroyDice();

    public void SendDiceToRespawnPosition(GameObject gameObject)
    {
        gameObject.transform.position = diceRespawnLocation.position;
    }
}

public enum DiceType
{
    D4 = 4,
    D6 = 6,
    D8 = 8,
    D10 = 10,
    D12 = 12,
    D20 = 20
}