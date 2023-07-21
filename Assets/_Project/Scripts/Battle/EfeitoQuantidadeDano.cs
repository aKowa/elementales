using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EfeitoQuantidadeDano : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private TMP_Text texto;

    [Header("Variaveis Padroes")]
    [SerializeField] private Color corNormal;
    [SerializeField] private Color corCritico;

    public void Iniciar(int quantidadeDano, bool ataqueCritico)
    {
        texto.faceColor = corNormal;

        if (quantidadeDano >= 0)
        {
            texto.text = quantidadeDano.ToString();
        }
        else
        {
            texto.text = "Miss";
            return;
        }

        if (ataqueCritico == true)
        {
            texto.text += "!!!";
            texto.faceColor = corCritico;
        }
    }

    public void SeDestruir()
    {
        Destroy(gameObject);
    }
}
