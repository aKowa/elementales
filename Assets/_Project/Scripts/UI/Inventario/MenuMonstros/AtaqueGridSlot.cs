using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AtaqueGridSlot : MonoBehaviour
{
    //Componentes
    [SerializeField] private SlotDeObjeto slotDeObjeto;
    [SerializeField] private Image imagem;
    private GuiaMoves guiaMoves;

    [Header("Variaveis Padroes")]
    [SerializeField] private Sprite spriteAtivado;
    [SerializeField] private Sprite spriteDesativado;

    //Variaveis
    private int indice;

    //Getters
    public int Indice => indice;

    private void Awake()
    {
        slotDeObjeto.OnDropEvent.AddListener(TrocarPosicaoDoAtaque);
    }

    public void Iniciar(GuiaMoves guiaMoves, int indice)
    {
        this.guiaMoves = guiaMoves;
        this.indice = indice;

        if (indice < guiaMoves.MonstroAtual.Attacks.Count)
        {
            SlotAtivado(true);
        }
        else
        {
            SlotAtivado(false);
        }
    }

    private void SlotAtivado(bool ativar)
    {
        slotDeObjeto.Ativado = ativar;

        if (ativar == true)
        {
            imagem.sprite = spriteAtivado;
        }
        else
        {
            imagem.sprite = spriteDesativado;
        }
    }

    private void TrocarPosicaoDoAtaque(PointerEventData eventData)
    {
        if (ObjetoArrastavel.ObjetoSendoArrastado != null)
        {
            AtaqueSlot ataqueSlot = ObjetoArrastavel.ObjetoSendoArrastado.GetComponent<AtaqueSlot>();
            AtaqueSlot ataqueSlotTemp = guiaMoves.AtaqueSlots[indice];

            int indiceOrigem = ataqueSlot.Indice;
            int indiceDestino = indice;

            guiaMoves.AtaqueSlots[indiceDestino] = ataqueSlot;
            guiaMoves.AtaqueSlots[indiceOrigem] = ataqueSlotTemp;

            ataqueSlot.Indice = indiceDestino;
            ataqueSlotTemp.Indice = indiceOrigem;

            ataqueSlotTemp.transform.SetAsLastSibling();
            ataqueSlot.transform.SetAsLastSibling();

            ataqueSlot.MoverAte(transform.position);
            ataqueSlotTemp.MoverAte(guiaMoves.GridSlots[indiceOrigem].transform.position);

            guiaMoves.MonstroAtual.Attacks[indiceDestino] = ataqueSlot.AttackHolder;
            guiaMoves.MonstroAtual.Attacks[indiceOrigem] = ataqueSlotTemp.AttackHolder;
        }
    }
}
