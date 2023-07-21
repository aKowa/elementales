using BergamotaDialogueSystem;
using BergamotaLibrary;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VasoPlanta : Interagivel
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private SpriteRenderer spriteDaPlanta;

    private MenuPlantarFrutaController menuPlantarFruta;

    private DialogueActivator dialogueActivator;
    private EntregarItem entregarItem;

    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoNaoPodeColherItem;
    [SerializeField] private DialogueObject dialogoPlantouItem;
    [SerializeField] private DialogueObject dialogoDesejaPlantarAlgo;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private string id;

    [Header("Opcoes")]
    [SerializeField] private Item plantaInicial;

    [Header("Data")]
    [SerializeField] private List<StructPlantavel> plantasData;

    [Space(10)]
    [SerializeField] private Sprite nada;

    private Planta itemPlantado;

    private bool podeColher;

    //Getters
    public string ID => id;
    public Planta ItemPlantado => itemPlantado;
    public DateTime DataPlantado => itemPlantado.DataPlantada;

    private void Awake()
    {
        menuPlantarFruta = FindObjectOfType<MenuPlantarFrutaController>();

        dialogueActivator = GetComponent<DialogueActivator>();
        entregarItem = GetComponent<EntregarItem>();
    }

    private void Start()
    {
        CarregarSave();

        VerificarSePodeColher();
        AtualizarSprite();
    }

    public override void Interagir(Player player)
    {
        if (VerificarSeTemAlgoPlantado())
        {
            if (podeColher == true)
            {
                //Dialogo Colher item
                ColherItem();
            }
            else
            {
                //Dialogo Nao pode ColherItem
                dialogueActivator.ShowDialogue(dialogoNaoPodeColherItem, DialogueUI.Instance);
            }
        }
        else
        {
            dialogueActivator.ShowDialogue(dialogoDesejaPlantarAlgo, DialogueUI.Instance);
        }
    }

    public void AbrirMenu()
    {
        menuPlantarFruta.IniciarMenu(this);
    }

    public void ColherItem()
    {
        EntregarFrutas();

        itemPlantado = null;

        AtualizarSprite();

        SalvarInformacoes();

        podeColher = false;
    }

    public void PlantarItem(Item frutaParaPlantar)
    {
        StructPlantavel planta = GetPlantaData(frutaParaPlantar);

        if (planta.Item == null)
        {
            throw new Exception("O item que tentou ser plantado nao esta na lista de plantas!");
        }

        itemPlantado = new Planta(planta);

        DialogueUI.Instance.SetPlaceholderDeTexto("%item", frutaParaPlantar.Nome);
        dialogueActivator.ShowDialogue(dialogoPlantouItem, DialogueUI.Instance);

        PlayerData.Instance.Inventario.RemoverItem(frutaParaPlantar, 1);

        VerificarSePodeColher();
        AtualizarSprite();

        SalvarInformacoes();
    }

    private void VerificarSePodeColher()
    {
        if(itemPlantado == null)
        {
            podeColher = false;
            return;
        }

        StructPlantavel planta = GetPlantaData(itemPlantado.GetItem);

        if(planta.Item == null)
        {
            throw new Exception("O item plantado nao esta na lista de plantas!");
        }

        DateTime dataFinal = itemPlantado.DataPlantada.AddHours(planta.HorasParaNascer);

        //Debug.Log($"Data plantada: {itemPlantado.DataPlantada.ToString("HH:mm:ss")}, Data final: {dataFinal.ToString("HH:mm:ss")}");

        podeColher = DateTime.Now >= dataFinal;
    }

    private bool VerificarSeTemAlgoPlantado()
    {
        if (itemPlantado == null)
        {
            return false;
        }

        return true;
    }

    public StructPlantavel GetPlantaData(Item item)
    {
        foreach(StructPlantavel planta in plantasData)
        {
            if(planta.Item.ID == item.ID)
            {
                return planta;
            }
        }

        return new StructPlantavel(null, 0, 0, null);
    }

    private void EntregarFrutas()
    {
        EntregarItem.ItemParaEntregar[] itensParaEntregar = new EntregarItem.ItemParaEntregar[1];
        StructPlantavel planta = GetPlantaData(itemPlantado.GetItem);

        if (planta.Item == null)
        {
            throw new Exception("O item plantado nao esta na lista de plantas!");
        }

        itensParaEntregar[0] = new EntregarItem.ItemParaEntregar(planta.Item, planta.QuantidadeDeFrutasQueVaoNascer);

        entregarItem.EntregarItens(itensParaEntregar);
    }

    private void AtualizarSprite()
    {
        if(itemPlantado == null)
        {
            spriteDaPlanta.sprite = nada;
            return;
        }

        StructPlantavel planta = GetPlantaData(itemPlantado.GetItem);

        if (planta.Item == null)
        {
            throw new Exception("O item plantado nao esta na lista de plantas!");
        }

        DateTime dataPlantada = itemPlantado.DataPlantada;
        DateTime dataFinal = dataPlantada.AddHours(planta.HorasParaNascer);

        TimeSpan tempoTotal = dataFinal - dataPlantada;
        TimeSpan tempoAtual = DateTime.Now - dataPlantada;

        if(tempoAtual >= tempoTotal)
        {
            spriteDaPlanta.sprite = planta.Sprite[planta.Sprite.Length - 1];
        }
        else
        {
            int numeroDeSprites = planta.Sprite.Length - 1;

            if(numeroDeSprites < 1)
            {
                spriteDaPlanta.sprite = planta.Sprite[0];
            }

            int indiceDoSprite = (int)((tempoAtual.TotalHours / tempoTotal.TotalHours) * numeroDeSprites);

            if(indiceDoSprite > numeroDeSprites)
            {
                indiceDoSprite = numeroDeSprites;
            }

            spriteDaPlanta.sprite = planta.Sprite[indiceDoSprite];
        }
    }

    private void CarregarSave()
    {
        Dictionary<string, VasoPlantaSave> vasosDePlanta = PlayerData.VasosDePlanta;

        if(vasosDePlanta.ContainsKey(id))
        {
            VasoPlantaSave vasoPlantaSave = vasosDePlanta[id];

            if(vasoPlantaSave.itemPlantadoID >= 0)
            {
                itemPlantado = new Planta(vasoPlantaSave);
            }
            else
            {
                itemPlantado = null;
            }
        }
        else if (plantaInicial != null)
        {
            StructPlantavel planta = GetPlantaData(plantaInicial);

            if (planta.Item == null)
            {
                throw new Exception("O item escolhido como planta inicial nao esta na lista de plantas!");
            }

            DateTime dataPlantada = DateTime.Now.Add(TimeSpan.FromHours((planta.HorasParaNascer + 1)) * -1);

            itemPlantado = new Planta(planta, dataPlantada);

            SalvarInformacoes();
        }
    }

    private void SalvarInformacoes()
    {
        Dictionary<string, VasoPlantaSave> vasosDePlanta = PlayerData.VasosDePlanta;

        if (vasosDePlanta.ContainsKey(id))
        {
            vasosDePlanta[id] = new VasoPlantaSave(this);
        }
        else
        {
            vasosDePlanta.Add(id, new VasoPlantaSave(this));
        }
    }
}

public class Planta
{
    //Variaveis
    private Item itemPlantado;
    private DateTime dataPlantada;

    //Getters
    public Item GetItem => itemPlantado;
    public DateTime DataPlantada => dataPlantada;


    public Planta(StructPlantavel item)
    {
        itemPlantado = item.Item;
        dataPlantada = DateTime.Now;
    }

    public Planta(StructPlantavel item, DateTime dataPlantada)
    {
        itemPlantado = item.Item;
        this.dataPlantada = dataPlantada;
    }

    public Planta(VasoPlantaSave vasoPlantaSave)
    {
        itemPlantado = GlobalSettings.Instance.Listas.ListaDeItens.GetData(vasoPlantaSave.itemPlantadoID);
        dataPlantada = SerializableDateTime.NewDateTime(vasoPlantaSave.dataPlantada);
    }
}

[System.Serializable]
public struct StructPlantavel
{
    [SerializeField] private Item item;
    [SerializeField] private int quantidadeDeFrutasQueVaoNascer;
    [SerializeField] private float horasParaNascer;

    [Space(10)]

    [SerializeField] private Sprite[] sprites;

    public Item Item => item;
    public int QuantidadeDeFrutasQueVaoNascer => quantidadeDeFrutasQueVaoNascer;
    public float HorasParaNascer => horasParaNascer;
    public Sprite[] Sprite => sprites;

    public StructPlantavel(Item item, int quantidadeDeFrutasQueVaoNascer, float horasParaNascer, Sprite sprite)
    {
        this.item = item;
        this.quantidadeDeFrutasQueVaoNascer = quantidadeDeFrutasQueVaoNascer;
        this.horasParaNascer = horasParaNascer;
        this.sprites = new Sprite[1] { sprite };
    }
}