using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Inventario/Habilidade")]

public class EnsinarHabilidade : AcaoNoInventario
{
    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private ComandoDeAtaque ataque;

    [Header("Dialogos")]
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoNaoPodeAprender;
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoAprendeuHabilidade;
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoJaSabeHabilidade;
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoQuerEsquecerHabilidade;

    //Getters
    public ComandoDeAtaque Ataque => ataque;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        return true;
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        MonsterLearnAttack.AprenderAtaque enumRespostaAprenderAtaque = menuBagController.TelaAprenderAtaque.VerificarAprenderAtaquePorItem(monstro, ataque);

        menuBagController.DialogueUI.SetPlaceholderDeTexto("%monstro", monstro.NickName);
        menuBagController.DialogueUI.SetPlaceholderDeTexto("%comando", ataque.Nome);

        switch (enumRespostaAprenderAtaque)
        {
            case MonsterLearnAttack.AprenderAtaque.PodeAprenderAtaque:
                menuBagController.AbrirDialogo(dialogoQuerEsquecerHabilidade);
                break;
            case MonsterLearnAttack.AprenderAtaque.NaoPodeAprenderAtaque:
                menuBagController.AbrirDialogo(dialogoNaoPodeAprender);
                break;
            case MonsterLearnAttack.AprenderAtaque.AcabouAprenderAtaque:
                menuBagController.AbrirDialogo(dialogoAprendeuHabilidade);
                break;
            case MonsterLearnAttack.AprenderAtaque.JaSabeAtaque:
                menuBagController.AbrirDialogo(dialogoJaSabeHabilidade);
                break;
        }
    }
}
