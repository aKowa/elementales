using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(InventarioNPC))]
public class InventarioNPCEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InventarioNPC inventarioNPC = (InventarioNPC)target;

        if (GUILayout.Button("Gerar Ataques"))
        {
            List<ComandoDeAtaque> ataques = new List<ComandoDeAtaque>();

            for (int i = 0; i < inventarioNPC.MonsterBag.Count; i++)
            {
                inventarioNPC.MonsterBag[i].Attacks.Clear();

                foreach (var item in inventarioNPC.MonsterBag[i].MonsterData.GetMonsterUpgradesPerLevel)
                {
                    if (item != null)
                    {
                        if (item.Level <= inventarioNPC.MonsterBag[i].AtributosAtuais.Nivel)
                        {
                            if (item.Ataque != null)
                            {
                                ataques.Add(item.Ataque);
                            }
                        }
                    }
                }

                int ataquesCount = 0;
                int ataquesMax = GetNumOfAttacks(inventarioNPC.MonsterBag[i].AtributosAtuais.Nivel, ataques.Count);

                while (ataquesCount < ataquesMax)
                {
                    var comandoDeAtaque = ataques[Random.Range(0, ataques.Count)];
                    bool contem = false;
                    foreach (var ataque in inventarioNPC.MonsterBag[i].Attacks)
                    {
                        if (ataque.Attack == comandoDeAtaque)
                        {
                            contem = true;
                            break;
                        }
                    }
                    if (!contem)
                    {
                        inventarioNPC.MonsterBag[i].Attacks.Add(new AttackHolder(comandoDeAtaque));
                        ataques.Remove(comandoDeAtaque);
                        ataquesCount++;
                    }
                }
            }

            EditorUtility.SetDirty(inventarioNPC);
        }

        if (GUILayout.Button("Gerar Dados"))
        {
            for (int i = 0; i < inventarioNPC.MonsterBag.Count; i++)
            {
                List<UpgradesPerLevel.DiceImprovement> diceUpgrades = new List<UpgradesPerLevel.DiceImprovement>();

                foreach (var item in inventarioNPC.MonsterBag[i].MonsterData.GetMonsterUpgradesPerLevel)
                {
                    if (item != null)
                    {
                        if (item.Level <= inventarioNPC.MonsterBag[i].AtributosAtuais.Nivel)
                        {
                            if (item.DiceUpgrade != UpgradesPerLevel.DiceImprovement.None && item.Level != 1)
                            {
                                diceUpgrades.Add(item.DiceUpgrade);
                            }
                        }
                    }
                }
                
                inventarioNPC.MonsterBag[i].Dices.Clear();
                inventarioNPC.MonsterBag[i].Dices.Add(DiceType.D6);
                foreach (UpgradesPerLevel.DiceImprovement upgrade in diceUpgrades)
                {
                    switch (upgrade)
                    {
                        case UpgradesPerLevel.DiceImprovement.New:
                            inventarioNPC.MonsterBag[i].Dices.Add(DiceType.D4);
                            break;
                        case UpgradesPerLevel.DiceImprovement.Better:
                            //TODO: This is not correct. Occasionally, the lowest dice will not be the lastIndex.
                            int lastIndex = inventarioNPC.MonsterBag[i].Dices.Count-1;
                            inventarioNPC.MonsterBag[i].Dices[lastIndex] = inventarioNPC.MonsterBag[i].Dices[lastIndex].Next();
                            break;
                    }
                }
            }
            
            EditorUtility.SetDirty(inventarioNPC);
        }

        if (GUILayout.Button("Gerar Combat Lessons"))
        {
            for (int i = 0; i < inventarioNPC.MonsterBag.Count; i++)
            {
                inventarioNPC.MonsterBag[i].CombatLessons.Clear();
                int maxLessons = 2;
                int count = 0;
                foreach (var item in inventarioNPC.MonsterBag[i].MonsterData.GetMonsterUpgradesPerLevel)
                {
                    if (count >= maxLessons)
                        break;
                    
                    if (item != null)
                    {
                        if (item.Level <= inventarioNPC.MonsterBag[i].AtributosAtuais.Nivel)
                        {
                            if (item.Lesson != null)
                            {
                                inventarioNPC.MonsterBag[i].CombatLessonsAtivos.Add(item.Lesson);
                                count++;
                            }
                        }
                    }
                }
            }
            
            EditorUtility.SetDirty(inventarioNPC);
        }

        if (GUILayout.Button("Gerar Novos Atributos"))
        {
            for (int i = 0; i < inventarioNPC.MonsterBag.Count; i++)
            {
                inventarioNPC.MonsterBag[i].AtributosAtuais.GerarNovosAtributos(inventarioNPC.MonsterBag[i].MonsterData);
                inventarioNPC.MonsterBag[i].AtributosAtuais.SomarExp(inventarioNPC.MonsterBag[i].AtributosAtuais.ExpParaONivelAtual());
                inventarioNPC.MonsterBag[i].AtributosAtuais.Exp = 0;
            }
            
            EditorUtility.SetDirty(inventarioNPC);
        }
    }
    
    private int GetNumOfAttacks(int nivel, int attacksCount)
    {
        int numMinOfAttacks = (int)Mathf.Clamp(nivel * 0.2f * Random.Range(1, 4), 1, 4);
        int numMaxOfAttacks = Mathf.Clamp(attacksCount, 1, 4);
        return Random.Range(numMinOfAttacks, numMaxOfAttacks + 1);
    }
}
