using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
	//Componentes
	private PlayableDirector director;

	//Variaveis
	[SerializeField] private DialogueObject dialogo;

	private bool clipRodou = false;
	private bool pausaAgendada = false;

	public override void OnPlayableCreate(Playable playable)
	{
		director = (playable.GetGraph().GetResolver() as PlayableDirector);
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if (clipRodou == false && info.weight > 0f)
		{
			if (Application.isPlaying)
			{
				if (DialogueUI.Instance.IsOpen == false)
				{
					DialogueUI.Instance.ShowDialogue(dialogo);
				}
				else
				{
					Debug.LogWarning($"A Timeline tentou abrir o dialogo {dialogo.name} enquanto a caixa de dialogo estava aberta!");
				}

				pausaAgendada = true;
			}

			clipRodou = true;
		}
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if (pausaAgendada == true)
		{
			pausaAgendada = false;

			if(Application.isPlaying)
            {
				GameManager.Instance.PausarTimeline(director);
				GameManager.Instance.EsperarDialogoParaResumirTimeline(director);
			}
		}

		clipRodou = false;
	}
}
