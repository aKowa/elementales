using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDeOpcoesController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderEfeitosSonoros;
    [SerializeField] private RectTransform menuConfirmacaoVoltarAoMenuPrincipal;
    [SerializeField] private RectTransform imagemTutorialBatalhaSelecionado;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private SceneReference cenaMenuPrincipal;

    [Space(10)]
    [SerializeField] private ListaDeFlags listaDeFlagsTutorialBatalha;
    [SerializeField] private string flagTutorialBatalha;

    protected override void OnAwake()
    {
        menuConfirmacaoVoltarAoMenuPrincipal.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        sliderMusica.onValueChanged.AddListener(SetMusicVolume);
        sliderEfeitosSonoros.onValueChanged.AddListener(SetSoundVolume);

        AtualizarImagemTutorialBatalha();
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        SaveManager.SalvarConfiguracoes();
    }

    public void IniciarMenu()
    {
        OpenView();

        sliderMusica.value = MusicManager.instance.Volume / 100;
        sliderEfeitosSonoros.value = SoundManager.instance.Volume / 100;

        AtualizarImagemTutorialBatalha();
    }

    public void SetMusicVolume(float value)
    {
        MusicManager.instance.SetVolume(value * 100);
    }

    public void SetSoundVolume(float value)
    {
        SoundManager.instance.SetVolume(value * 100);
    }

    public void ConfirmacaoVoltarAoMenuPrincipal()
    {
        menuConfirmacaoVoltarAoMenuPrincipal.gameObject.SetActive(true);
    }

    public void VoltarAoMenuPrincipal()
    {
        FecharMenusSuspensos();

        canvasGroup.blocksRaycasts = false;

        FazerTransicaoProMenuInicial();
    }

    public void VisitarAWiki()
    {
        Application.OpenURL("https://bit.ly/Elementales-Wiki");
    }

    public void FecharMenusSuspensos()
    {
        menuConfirmacaoVoltarAoMenuPrincipal.gameObject.SetActive(false);
    }

    private void FazerTransicaoProMenuInicial()
    {
        PauseManager.PermitirInputGeral = false;

        Transition.GetInstance().DoTransition("FadeIn", 0, () =>
        {
            MapsManager.GetInstance().LoadSceneByName(cenaMenuPrincipal.ScenePath);
        });
    }

    public void AtivarOuDesativarTutorialBatalha()
    {
        Flags.SetFlag(listaDeFlagsTutorialBatalha.name, flagTutorialBatalha, !Flags.GetFlag(listaDeFlagsTutorialBatalha.name, flagTutorialBatalha));

        AtualizarImagemTutorialBatalha();
    }

    public void AtualizarImagemTutorialBatalha()
    {
        imagemTutorialBatalhaSelecionado.gameObject.SetActive(Flags.GetFlag(listaDeFlagsTutorialBatalha.name, flagTutorialBatalha));
    }
}
