using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Monster/Status/Secundario")]

public class StatusEffectSecundario : ScriptableObject
{
    //Enums

    public enum TipoStatus { Locked, MonstroForaDeBatalhaTemporario }
    public enum TipoDeEfeitoVisualEnum { Nenhum, TintEffect, TintEffectSlow, TintSolidEffect, TintSolidEffectSlow }

    //Variaveis

    [Header("Variaveis")]
    [SerializeField] TipoStatus tipoStatus;

    [Header("Dialogo")]
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoDoEfeito;

    [Header("Efeito Visual")]
    [SerializeField] private TipoDeEfeitoVisualEnum tipoDeEfeitoVisual;

    [ShowIf("@tipoDeEfeitoVisual != TipoDeEfeitoVisualEnum.Nenhum")]
    [SerializeField] private Color corDoEfeito;

    [ShowIf("@tipoDeEfeitoVisual != TipoDeEfeitoVisualEnum.Nenhum")]
    [SerializeField] private float velocidadeDoEfeito;

    //Getters

    public TipoStatus GetTipoStatus => tipoStatus;
    public BergamotaDialogueSystem.DialogueObject DialogoDoEfeito => dialogoDoEfeito;
    public Color CorDoEfeito => corDoEfeito;
    public float VelocidadeDoEfeito => velocidadeDoEfeito;
    public TipoDeEfeitoVisualEnum TipoDeEfeitoVisual => tipoDeEfeitoVisual;

    public bool ForaDeCombate()
    {
        if (tipoStatus == TipoStatus.MonstroForaDeBatalhaTemporario)
            return true;

        return false;
    }
}
