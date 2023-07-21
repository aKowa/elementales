using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipoLogo : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image imagem;
    [SerializeField] private TMP_Text texto;

    public void SetTipo(MonsterType tipo)
    {
        if(tipo == null || tipo.name == "Empty")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);

            texto.text = GetNome(tipo);
            imagem.color = GetColor(tipo);
        }
    }

    private string GetNome(MonsterType tipo)
    {
        return tipo.Nome;
    }

    private Color GetColor(MonsterType tipo)
    {
        return tipo.TypeColor;
    }
}
