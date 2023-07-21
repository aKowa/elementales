using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusLogo : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image imagem;
    [SerializeField] private TMP_Text texto;

    public void SetStatus(StatusEffectBase status)
    {
        if(status == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);

            texto.text = GetNome(status);
            imagem.color = GetColor(status);
        }
    }

    private string GetNome(StatusEffectBase status)
    {
        return status.Nome;
    }

    private Color GetColor(StatusEffectBase status)
    {
        return status.StatusColor;
    }
}
