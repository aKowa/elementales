// Copyright (C) Lumen Section - All Rights Reserved
// Written by Nicolas Baillard <nicolas.baillard@gmail.com>
using UnityEngine;



namespace LumenSection.LevelLinker
{
public class SpawnPoint : MonoBehaviour
{
  public GameObject CharacterPrefab;
  
  
  
  private void Start()
  {
    // Instantiate character if it's not there already
    var go = GameObject.FindWithTag("Player");
    if (go != null)
      return;

    go = Instantiate(CharacterPrefab, transform.position, Quaternion.identity);
    var character = go.GetComponent<Character>();
    character.SetDirection(Vector2.down);
  }
}
}
