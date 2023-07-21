using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(GatewaysHandler), typeof(GridHandler), typeof(WildAreasHandler))]
public class SceneMap : MonoBehaviour
{
    [Button, ShowIf("sceneData")]
    private void GenerateNewMap()
    {
        gatewaysHandler = GetComponent<GatewaysHandler>();
    }
    
    public SceneData sceneData;
    
    //Components
    private GatewaysHandler gatewaysHandler;
    private GridHandler gridHandler;
    private WildAreasHandler wildAreasHandler;
}