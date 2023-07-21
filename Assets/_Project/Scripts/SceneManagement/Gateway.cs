// Copyright (C) Lumen Section - All Rights Reserved
// Written by Nicolas Baillard <nicolas.baillard@gmail.com>

using System;
using UnityEngine;


namespace LumenSection.LevelLinker
{
    public class Gateway : UniqueId, IDoor
    {
        //Componentes
        private SceneSpawnManager sceneSpawnManager;

        //Variavel
        [SerializeField] private EntityModel.Direction direcao;
        [SerializeField] private Transform posicaoSpawnPlayer;
        [SerializeField] private bool ativarAoColidirComOPlayer = true;

        //Getters
        public string Name
        {
            get { return name; }
        }

        private void Awake()
        {
            //Componentes
            sceneSpawnManager = FindObjectOfType<SceneSpawnManager>();

            if(sceneSpawnManager == null)
            {
                Debug.LogWarning("Nao ha um SceneSpawnManager na cena!");
            }
        }

        public void Spawn(string lastGatewayID)
        {
            Player player = FindObjectOfType<Player>();

            sceneSpawnManager.DirecaoPlayerInicial = direcao;
            sceneSpawnManager.PosicaoPlayerInicial = posicaoSpawnPlayer.transform.position;

            sceneSpawnManager.SpawnPlayer(player, lastGatewayID);
        }

        public void FazerTransicao()
        {
            FazerTransicao(PlayerData.Instance.GetComponent<Player>());
        }

        public void FazerTransicao(Player player)
        {
            BergamotaLibrary.PauseManager.PermitirInput = false;

            player.PlayerMovement.ZeroVelocity();

            Transition.GetInstance().DoTransition("FadeIn", 0, 2, () =>
            {
                MapsManager.GetInstance().TakeDoor(this, () =>
                {
                    Transition.GetInstance().DoTransition("FadeOut", 0.5f, 2, () =>
                    {
                        FindObjectOfType<JanelaMapaAtual>(true).MostrarNomeDoMapa();
                        BergamotaLibrary.PauseManager.PermitirInput = true;
                    });
                });
            });
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(ativarAoColidirComOPlayer == false)
            {
                return;
            }

            if (!col.gameObject.CompareTag("Player"))
            {
                return;
            }

            FazerTransicao(col.GetComponent<Player>());
        }
    }
}