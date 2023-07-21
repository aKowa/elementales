using UnityEngine;
using UnityEditor;

namespace BergamotaDialogueSystem
{
    //Este e um editor para fazer um botao de recarregar na interface do Unity para os scripts de DialogueResponseEvents
    [CustomEditor(typeof(DialogueResponseEvents))]
    public class DialogueResponseEventsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DialogueResponseEvents responseEvents = (DialogueResponseEvents)target;

            if (GUILayout.Button("Recarregar"))
            {
                responseEvents.OnValidate();
            }
        }
    }
}