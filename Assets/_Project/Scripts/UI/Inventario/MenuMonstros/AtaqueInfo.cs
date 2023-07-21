using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AtaqueInfo : MonoBehaviour
{
    //Componentes
    [SerializeField] private TMP_Text textoCategoria;
    [SerializeField] private TMP_Text textoPoder;
    [SerializeField] private TMP_Text textoPrecisao;
    [SerializeField] private TMP_Text textoDescricao;

    public void AtualizarInformacoes(ComandoDeAtaque ataque)
    {
        textoCategoria.text = GetCategoria(ataque.AttackData.Categoria);
        textoPoder.text = ataque.AttackData.Poder.ToString();
        textoPrecisao.text = ataque.AttackData.ChanceAcerto.ToString();
        textoDescricao.text = ataque.Descricao;
    }

    public void ResetarInformacoes()
    {
        textoCategoria.text = string.Empty;
        textoPoder.text = string.Empty;
        textoPrecisao.text = string.Empty;
        textoDescricao.text = string.Empty;
    }

    private string GetCategoria(AttackData.CategoriaEnum categoria)
    {
        switch (categoria)
        {
            case AttackData.CategoriaEnum.Fisico:
                return "Physical";

            case AttackData.CategoriaEnum.Especial:
                return "Special";

            case AttackData.CategoriaEnum.Status:
                return "Status";

            default:
                return "NotDefined";
        }
    }
}
