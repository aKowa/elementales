using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiaDices : MonstroInfo
{
    //Componentes
    [Header("Componentes")]

    [SerializeField] private GameObject diceRotationBase;
    [SerializeField] private GameObject diceTextureBase;

    [SerializeField] private Transform diceTexturesHolder;
    [SerializeField] private Transform combatLessonsAtivosHolder;

    [SerializeField] private AtaqueInfo combatLessonInfo;

    [Header("Menus")]
    [SerializeField] private MenuDasSkinsController menuDasSkins;
    [SerializeField] private MenuDosCombatLessonsController menuDosCombatLessons;

    //Variaveis
    private List<RawImage> diceTextures = new List<RawImage>();
    private List<DiceRotation> dices = new List<DiceRotation>();
    private List<CombatLessonSlot> combatLessonSlots = new List<CombatLessonSlot>();

    private Monster monstroAtual;

    private void Awake()
    {
        combatLessonInfo.gameObject.SetActive(false);

        menuDasSkins.EventoSkinSelecionada.AddListener(SkinSelecionada);
        menuDosCombatLessons.EventoCombatLessonsAtualizados.AddListener(AtualizarCombatLessons);

        foreach (CombatLessonSlot combatLessonSlot in combatLessonsAtivosHolder.GetComponentsInChildren<CombatLessonSlot>(true))
        {
            combatLessonSlot.ResetarInformacoes();
            combatLessonSlot.EventoSlotSelecionado.AddListener(AbrirMenuDosCombatLessons);
            combatLessonSlot.BotaoInfoSelecionado.AddListener(AbrirCombatLessonInfo);

            combatLessonSlots.Add(combatLessonSlot);
        }
    }

    public override void AtualizarInformacoes(Monster monstro)
    {
        monstroAtual = monstro;

        AtualizarDados(monstroAtual);
        AtualizarCombatLessons();
    }

    public override void ResetarInformacoes()
    {
        ResetarDiceIcons();
    }

    private void AtualizarDados(Monster monstro)
    {
        ResetarDiceIcons();

        for (int i = 0; i < monstro.Dices.Count; i++)
        {
            RawImage diceTexture = Instantiate(diceTextureBase, diceTexturesHolder).GetComponent<RawImage>();
            diceTexture.gameObject.SetActive(true);

            DiceRotation dice = Instantiate(diceRotationBase).GetComponent<DiceRotation>();
            dice.gameObject.SetActive(true);

            dice.transform.position = new Vector3(i * 10, 0, 0);

            dice.ChooseDiceToShow(monstro.Dices[i]);
            dice.ChooseDiceTexture(dice.DiceDictionary[monstro.Dices[i]].GetComponentInChildren<MeshRenderer>(), monstro.DiceMaterial);

            diceTexture.texture = dice.RenderTexture;

            diceTextures.Add(diceTexture);
            dices.Add(dice);
        }
    }

    private void ResetarDiceIcons()
    {
        foreach (RawImage diceTexture in diceTextures)
        {
            Destroy(diceTexture.gameObject);
        }

        foreach (DiceRotation dice in dices)
        {
            Destroy(dice.gameObject);
        }

        diceTextures.Clear();
        dices.Clear();
    }

    private void AtualizarCombatLessons()
    {
        for(int i = 0; i < combatLessonSlots.Count; i++)
        {
            if(i >= monstroAtual.CombatLessonsAtivos.Count)
            {
                combatLessonSlots[i].ResetarInformacoes();
            }
            else
            {
                combatLessonSlots[i].Iniciar(monstroAtual.CombatLessonsAtivos[i]);
            }
        }
    }

    public void AbrirMenuDasSkins()
    {
        menuDasSkins.IniciarMenu(monstroAtual.DiceMaterial, PlayerData.SkinsDeDados);
    }

    private void SkinSelecionada(string chaveDaSkin)
    {
        if(chaveDaSkin == monstroAtual.DiceMaterial)
        {
            return;
        }

        monstroAtual.DiceMaterial = chaveDaSkin;

        AtualizarDados(monstroAtual);

        menuDasSkins.AtualizarSelecaoDosDados(monstroAtual.DiceMaterial);
    }

    private void AbrirCombatLessonInfo(CombatLesson combatLesson)
    {
        combatLessonInfo.gameObject.SetActive(true);
        combatLessonInfo.AtualizarInformacoes(combatLesson);
    }

    public void FecharCombatLessonInfo()
    {
        combatLessonInfo.gameObject.SetActive(false);
    }

    private void AbrirMenuDosCombatLessons(CombatLesson combatLesson)
    {
        menuDosCombatLessons.IniciarMenu(monstroAtual);
    }

    public void AbrirMenuDosCombatLessons()
    {
        AbrirMenuDosCombatLessons(null);
    }
}
