using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonstroGridSlot : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private SlotDeObjeto slotDeObjeto;
    [SerializeField] private Image imagem;
    private MenuMonstrosController menuMonstrosController;

    [Header("Variaveis Padroes")]
    [SerializeField] private Sprite spriteAtivado;
    [SerializeField] private Sprite spriteDesativado;

    //Variaveis
    private int indice;

    //Getters
    public int Indice => indice;

    private void Awake()
    {
        slotDeObjeto.OnDropEvent.AddListener(TrocarPosicaoDoMonstro);
    }

    public void Iniciar(MenuMonstrosController menuMonstrosController, int indice)
    {
        this.menuMonstrosController = menuMonstrosController;
        this.indice = indice;

        if(indice < menuMonstrosController.Inventario.MonsterBag.Count)
        {
            SlotAtivado(true);
        }
        else
        {
            SlotAtivado(false);
        }
    }

    public void SlotAtivado(bool ativar)
    {
        slotDeObjeto.Ativado = ativar;

        if(ativar == true)
        {
            imagem.sprite = spriteAtivado;
        }
        else
        {
            imagem.sprite = spriteDesativado;
        }
    }

    private void TrocarPosicaoDoMonstro(PointerEventData eventData)
    {
        if(ObjetoArrastavel.ObjetoSendoArrastado != null)
        {
            MonstroSlot monstroSlot = ObjetoArrastavel.ObjetoSendoArrastado.GetComponent<MonstroSlot>();
            MonstroSlot monstroSlotTemp = menuMonstrosController.MonstroSlots[indice];

            int indiceOrigem = monstroSlot.Indice;
            int indiceDestino = indice;

            menuMonstrosController.MonstroSlots[indiceDestino] = monstroSlot;
            menuMonstrosController.MonstroSlots[indiceOrigem] = monstroSlotTemp;

            monstroSlot.Indice = indiceDestino;
            monstroSlotTemp.Indice = indiceOrigem;

            monstroSlotTemp.transform.SetAsLastSibling();
            monstroSlot.transform.SetAsLastSibling();

            monstroSlot.MoverAte(transform.position);
            monstroSlotTemp.MoverAte(menuMonstrosController.GridSlots[indiceOrigem].transform.position);

            menuMonstrosController.Inventario.MonsterBag[indiceDestino] = monstroSlot.Monstro;
            menuMonstrosController.Inventario.MonsterBag[indiceOrigem] = monstroSlotTemp.Monstro;
        }
    }
}
