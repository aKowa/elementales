using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Modificador
{
    public enum Atributo { ataque=0, defesa=1, spAtaque=2, spDefesa=3, velocidade=4 };

    private Atributo atributo;
    [SerializeField] private List<ModificadorTurno> listaBuffsDebuffs = new List<ModificadorTurno>();

    public Atributo GetAtributo => atributo;

    public List<ModificadorTurno> ListaBuffsDebuffs => listaBuffsDebuffs;
    private int idModificador;
    public int IdModificador => idModificador;
    public Modificador(Atributo atributo)
    {
        this.atributo = atributo;
        listaBuffsDebuffs = new List<ModificadorTurno>();
        idModificador = 0;
    }
    public void LimparVariaveis()
    {
        idModificador = 0;
        listaBuffsDebuffs.Clear();
    }
    public int ValorAtual
    {
        get
        {
            int x = 0;
            for (int i = 0; i < listaBuffsDebuffs.Count; i++)
            {
                x += listaBuffsDebuffs[i].ValorModificador;
            }
            x = Mathf.Clamp(x, -6, 6);
            return x;
        }
    }
    public int AvancarTurno()
    {
        List<ModificadorTurno> listaIndicesModificadoresParaRemover = new List<ModificadorTurno>();
        for (int i = 0; i < listaBuffsDebuffs.Count; i++)
        {
            if (listaBuffsDebuffs[i].PassaComTurnos)
            {
                if (listaBuffsDebuffs[i].VerificarTempoModificadorAtivo())
                {
                    listaIndicesModificadoresParaRemover.Add(listaBuffsDebuffs[i]);
                }
            }
        }
        for (int i = 0; i < listaIndicesModificadoresParaRemover.Count; i++)
        {
            listaBuffsDebuffs.Remove(listaIndicesModificadoresParaRemover[i]);
        }
        return ValorAtual;
    }
    public int Adicionar(ModificadorTurno modificador)
    {
        listaBuffsDebuffs.Add(modificador);
        idModificador++;
        return idModificador - 1;
    }
    public void RemoverStatus(int id)
    {
        for (int i = 0; i < listaBuffsDebuffs.Count; i++)
        {
            if(listaBuffsDebuffs[i].Origem == id)
            {
                listaBuffsDebuffs.RemoveAt(i);
                break;
            }
        }
    }

    public int Calc()
    {
        int n1 = 0;
        switch (ValorAtual)
        {
            case -6:
                n1 = +25;
                break;
            case -5:
                n1 = +28;
                break;
            case -4:
                n1 = +33;
                break;
            case -3:
                n1 = +40;
                break;
            case -2:
                n1 = +50;
                break;
            case -1:
                n1 = +66;
                break;
            case 0:
                n1 = 100;
                break;
            case 1:
                n1 = 150;
                break;
            case 2:
                n1 = 200;
                break;
            case 3:
                n1 = 250;
                break;
            case 4:
                n1 = 300;
                break;
            case 5:
                n1 = 350;
                break;
            case 6:
                n1 = 400;
                break;
        }
        return n1;
    }
}
[System.Serializable]
public class ModificadorTurno
{
    [SerializeField] private int quantidadeTurnos;
    [SerializeField] private bool passaComTurnos;
    [SerializeField] private int valorModificador;
    [SerializeField] private bool SeRemover = false;
    private int origem;
    public int Origem
    {
        get
        {
            return origem;
        }
        set => origem = value;
    }
    public bool PassaComTurnos => passaComTurnos;
    public int ValorModificador => valorModificador;
    public int QuantidadeTurnos => quantidadeTurnos;

    public ModificadorTurno(int quantidadeTurnos, bool passaComTurnos, int valorModificador, int origem)
    {
        this.quantidadeTurnos = quantidadeTurnos+1;
        this.passaComTurnos = passaComTurnos;
        this.valorModificador = valorModificador;
        this.origem = origem;
    }
    public bool VerificarTempoModificadorAtivo()
    {
        quantidadeTurnos--;
        if (passaComTurnos)
        {
            if (quantidadeTurnos <= 0)
                SeRemover = true;
        }
        return SeRemover;
    }
}


