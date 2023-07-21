using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LumenSection.LevelLinker;
using Sirenix.OdinInspector;
using UnityEngine;

public class GatewaysHandler : MonoBehaviour
{
    [Button]
    public void NewGateway()
    {
        GameObject obj = new GameObject();
        obj.AddComponent<Gateway>();
        obj.name = "New Gateway";
    }
}
