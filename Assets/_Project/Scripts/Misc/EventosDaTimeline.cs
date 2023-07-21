using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Eventos da Timeline")]
public class EventosDaTimeline : ScriptableObject
{
    public void FinalizarTimeline()
    {
        if(Application.isPlaying)
        {
            GameManager.Instance.FinalizarTimeline();
        }
    }

    public void PausarTimeline()
    {
        if (Application.isPlaying)
        {
            GameManager.Instance.PausarTimeline();
        }
    }

    public void ResumirTimeline()
    {
        if (Application.isPlaying)
        {
            GameManager.Instance.ResumirTimeline();
        }
    }

    public void PararTimeline()
    {
        if (Application.isPlaying)
        {
            GameManager.Instance.PararTimeline();
        }
    }
}
