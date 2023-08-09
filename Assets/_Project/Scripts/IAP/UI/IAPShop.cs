using System;
using System.Collections.Generic;
using System.Linq;
using BergamotaDialogueSystem;
using BergamotaLibrary;
using UnityEngine;
using UnityEngine.Purchasing;
using Random = UnityEngine.Random;

public class IAPShop : MonoBehaviour
{
    private const string coinPack1 = "coinpack1",
        coinPack2 = "coinpack2",
        coinPack3 = "coinpack3",
        chest1 = "chest1",
        chest2 = "chest2",
        chest3 = "chest3",
        dealPack1 = "dealpack1",
        dealPack2 = "dealpack2",
        key1 = "key1",
        key2 = "key2",
        magnet = "magnet",
        xmasChest = "xmaschest",
        dealPackXMas = "dealpackxmas";

    //Rewards
    [SerializeField] private Item key;
    [SerializeField] private InventarioComPeso classicChest;
    [SerializeField] private InventarioComPeso rareChest;
    [SerializeField] private InventarioComPeso xMasChest;
    
    //Dialogs
    [SerializeField] private DialogueObject dialogoCompraSucesso, dialogoCompraFalhou;
    [SerializeField] private DialogueActivator dialogueActivator;
    [SerializeField] private EntregarItem entregarItem;

    //Sons
    [Header("Sons")]
    [SerializeField] private AudioClip somCompraItem;

    public void AssignButtonBehaviour(List<ItemSlotLojaIAP> itemSlotLojaIaps)
    {
        Debug.Log("OnFinishedUpdatingList_AssignButtonBehaviour");
        foreach (var item in itemSlotLojaIaps)
        {
            IAPButton iapButton = item.IAPButton;
            iapButton.onPurchaseComplete.RemoveAllListeners();
            iapButton.onPurchaseFailed.RemoveAllListeners();
            iapButton.onPurchaseComplete.AddListener(OnPurchaseComplete);
            iapButton.onPurchaseFailed.AddListener(OnPurchaseFailed);
        }
    }

    public void OnPurchaseComplete(Product product)
    {
        Debug.Log($"Purchase completed: {product}");
        List<List<EntregarItem.ItemParaEntregar>> listOfRewardsPerChest = new List<List<EntregarItem.ItemParaEntregar>>();
        List<string> chestNames = new List<string>();
        switch (product.definition.id)
        {
            case coinPack1:
                PlayerData.Instance.Inventario.Dinheiro += 1500;
                break;
            
            case coinPack2:
                PlayerData.Instance.Inventario.Dinheiro += 3750;
                break;
            
            case coinPack3:
                PlayerData.Instance.Inventario.Dinheiro += 6100;
                break;
            
            case chest1:
                listOfRewardsPerChest.Add(RewardFromChest(classicChest, 6));
                chestNames.Add(classicChest.displayName);
                break;
            
            case chest2:
                for (int i = 0; i < 3; i++)
                {
                    List<EntregarItem.ItemParaEntregar> newReward = RewardFromChest(classicChest, 6);
                    chestNames.Add(classicChest.displayName);
                    listOfRewardsPerChest.Add(newReward);
                }
                break;
            
            case chest3:
                for (int i = 0; i < 7; i++)
                {
                    List<EntregarItem.ItemParaEntregar> newReward = RewardFromChest(classicChest, 6);
                    chestNames.Add(classicChest.displayName);
                    listOfRewardsPerChest.Add(newReward);
                }
                break;
            
            case dealPack1:
                PlayerData.Instance.Inventario.Dinheiro += 2500;
                listOfRewardsPerChest.Add(RewardFromChest(classicChest, 6));
                chestNames.Add(classicChest.displayName);
                break;
            
            case dealPack2:
                PlayerData.Instance.Inventario.Dinheiro += 5200;
                listOfRewardsPerChest.Add(RewardFromChest(classicChest, 6));
                chestNames.Add(classicChest.displayName);
                PlayerData.Instance.Inventario.AddItem(key, 1);
                break;
            
            case key1:
                PlayerData.Instance.Inventario.AddItem(key, 1);
                break;
            
            case key2:
                PlayerData.Instance.Inventario.AddItem(key, 3);
                break;
            
            case xmasChest:
                listOfRewardsPerChest.Add(RewardFromChest(xMasChest, 5));
                chestNames.Add(xMasChest.displayName);
                break;
            
            case dealPackXMas:
                PlayerData.Instance.Inventario.Dinheiro += 3500;
                for (int i = 0; i < 2; i++)
                {
                    List<EntregarItem.ItemParaEntregar> newReward = RewardFromChest(xMasChest, 5);
                    listOfRewardsPerChest.Add(newReward);
                    chestNames.Add(xMasChest.displayName);
                }
                break;
            
            case magnet:
                PlayerData.Instance.Inventario.AddMagnetBuff();
                break;
        }
        DialogueUI.Instance.SetPlaceholderDeTexto("%IAP", product.metadata.localizedTitle);
        dialogueActivator.ShowDialogue(dialogoCompraSucesso, DialogueUI.Instance);

        if (listOfRewardsPerChest.Any())
        {
            for (int i = 0; i < listOfRewardsPerChest.Count; i++)
            {
                DialogueUI.Instance.SetPlaceholderDeTexto("%Chest", chestNames[i]);
                List<EntregarItem.ItemParaEntregar> itemsParaEntregar = listOfRewardsPerChest[i];
                entregarItem.EntregarItens(itemsParaEntregar.ToArray());
            }
        }
        SoundManager.instance.TocarSomIgnorandoPause(somCompraItem);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log($"Purcahse of {product.definition.id} failed due to {reason}");
        dialogueActivator.ShowDialogue(dialogoCompraFalhou, DialogueUI.Instance);
    }

    public List<EntregarItem.ItemParaEntregar> RewardFromChest(InventarioComPeso chest, int amount)
    {
        List<EntregarItem.ItemParaEntregar> itemsRewarded = new List<EntregarItem.ItemParaEntregar>();
        InventarioComPeso tempChest = Instantiate(chest);
        bool rewardedRare = false;
        for (int i = 0; i < amount; i++)
        {
            Item item;
            double weight;
            (item, weight) = tempChest.ItensDaLoja.GetRandomWithWeight();
            tempChest.ItensDaLoja.RemoveItem(item);
            if (weight <= 2)
                rewardedRare = true;
            if (i == amount - 1 && rewardedRare == false)
            {
                RewardFromChest(rareChest, 1);
                break;
            }
            int quantidade = weight <= 2 ? 1 : Random.Range(2, 5);
            // PlayerData.Instance.Inventario.AddItem(item, quantidade);
            itemsRewarded.Add(new EntregarItem.ItemParaEntregar(item, quantidade));
            Debug.Log($"Chest rewarded {item} x{quantidade}");
        }

        if (itemsRewarded.Any())
        {
            return itemsRewarded;
        }

        return null;
    }
}