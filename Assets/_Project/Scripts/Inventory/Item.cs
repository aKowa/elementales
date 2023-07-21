using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventario/Item")]
public class Item : ScriptableObject
{
    //Enums
    public enum TipoItem { Consumivel, MonsterBall, Habilidade, ItemChave }
    public enum EfeitoItem { CuraHP, RecuperaPP, ReviveMonstro, CuraStatus, AumentaStatusTemporariamente, AumentaStatusPermanentemente, CapturaMonstro, EnsinaHabilidade, Nenhum }

    //Variaveis
    [Header("Informacoes do Inventario")]
    [SerializeField] private TipoItem tipo = TipoItem.ItemChave;
    [SerializeField] private EfeitoItem efeito = EfeitoItem.Nenhum;

    [SerializeField] private string nome;
    
    [SerializeField, PreviewField(75), HideLabel] 
    private Sprite imagem;
    [SerializeField][TextArea(1, 5)] private string descricao;
    [SerializeField] private int preco;

    [Header("Efeitos do Item")]
    [SerializeField] private AcaoNoInventario efeitoForaDaBatalha;
    [SerializeField] private AcaoNoInventario efeitoNaBatalha;
    [SerializeField] private ComandoDeItem comandoNaBatalha;

    //Getters
    public int ID => GlobalSettings.Instance.Listas.ListaDeItens.GetId(this);
    public TipoItem Tipo => tipo;
    public EfeitoItem Efeito => efeito;
    public string Nome => nome;
    public Sprite Imagem => imagem;
    public string Descricao => descricao;
    public int Preco => preco;
    public AcaoNoInventario EfeitoForaDaBatalha => efeitoForaDaBatalha;
    public AcaoNoInventario EfeitoNaBatalha => efeitoNaBatalha;
    public ComandoDeItem ComandoNaBatalha => comandoNaBatalha;
}