using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoxMonstroGridSlot : MonoBehaviour
{
    //Componentes
    [SerializeField] private SlotDeObjeto slotDeObjeto;
    private MonsterBoxController monsterBoxController;

    //Variaveis
    private int indiceBox;
    private int indice;
    private MonsterBoxController.TipoSlot tipo;

    //Getters
    public int IndiceBox => indiceBox;
    public int Indice => indice;

    private void Awake()
    {
        slotDeObjeto.OnDropEvent.AddListener(TrocarPosicaoDoMonstro);
    }

    public void Iniciar(MonsterBoxController monsterBoxController, int indiceBox, int indice, MonsterBoxController.TipoSlot tipo)
    {
        this.monsterBoxController = monsterBoxController;
        this.indiceBox = indiceBox;
        this.indice = indice;
        this.tipo = tipo;
    }

    private void TrocarPosicaoDoMonstro(PointerEventData eventData)
    {
        if (ObjetoArrastavel.ObjetoSendoArrastado != null)
        {
            BoxMonstroSlot monstroSlot = ObjetoArrastavel.ObjetoSendoArrastado.GetComponent<BoxMonstroSlot>();
            BoxMonstroSlot monstroSlotTemp = monsterBoxController.GetMonstroSlot(tipo, indice);

            if(monstroSlot.Tipo == MonsterBoxController.TipoSlot.Party)
            {
                if(monsterBoxController.TemOutroMonstroSaudavelNaParty(monstroSlot.Monstro) == false)
                {
                    if (monstroSlotTemp == null)
                    {
                        if(tipo != MonsterBoxController.TipoSlot.Party)
                        {
                            ObjetoArrastavel.ObjetoSendoArrastado.TrocouDePosicao = false;

                            monsterBoxController.DialogoHealthyMonster();

                            return;
                        }
                    }
                    else if (monstroSlotTemp.Monstro.IsFainted == true && monstroSlotTemp.Tipo != MonsterBoxController.TipoSlot.Party)
                    {
                        ObjetoArrastavel.ObjetoSendoArrastado.TrocouDePosicao = false;

                        monsterBoxController.DialogoHealthyMonster();

                        return;
                    }
                }
            }
            else if(monstroSlot.Tipo == MonsterBoxController.TipoSlot.Box && monstroSlot.Monstro.IsFainted == true)
            {
                if(monstroSlotTemp != null)
                {
                    if(monsterBoxController.TemOutroMonstroSaudavelNaParty(monstroSlotTemp.Monstro) == false)
                    {
                        ObjetoArrastavel.ObjetoSendoArrastado.TrocouDePosicao = false;

                        monsterBoxController.DialogoHealthyMonster();

                        return;
                    }
                }
            }

            int indiceDestino = monstroSlot.Indice;
            MonsterBoxController.TipoSlot tipoDestino = monstroSlot.Tipo;

            monstroSlot.MoverMonstroParaLista(indice, tipo);

            if(monstroSlotTemp != null)
            {
                monstroSlotTemp.MoverMonstroParaLista(indiceDestino, tipoDestino);

                monstroSlotTemp.transform.SetAsLastSibling();
            }
            else
            {
                switch (tipoDestino)
                {
                    case MonsterBoxController.TipoSlot.Box:
                        monsterBoxController.Inventario.MonsterBank[monsterBoxController.IndiceBox].Monsters[indiceDestino] = null;
                        break;

                    case MonsterBoxController.TipoSlot.Party:
                        monsterBoxController.Inventario.MonsterBag.RemoveAt(indiceDestino);
                        break;
                }
            }

            monstroSlot.transform.SetAsLastSibling();

            monsterBoxController.AjustarMonsterBag();
        }
    }
}
