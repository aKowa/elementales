using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceAreaCenter : MonoBehaviour
{
    [SerializeField] private List<Collider> wallColliders = new List<Collider>();
    private List<Collider> collidersWhoCameThrough = new List<Collider>();

    public List<Collider> CollidersWhoCameThrough
    {
        get => collidersWhoCameThrough;
        set => collidersWhoCameThrough = value;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            PrintIgnoreCollision();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dice"))
        {
            //Debug.Log($"{other.gameObject.name} saiu.");

            if (CollidersWhoCameThrough.Contains(other) == false)
            {
                wallColliders.ForEach(w => Physics.IgnoreCollision(other, w, false));
                CollidersWhoCameThrough.Add(other);
            }
        }
    }

    private void PrintIgnoreCollision()
    {
        foreach (var c in CollidersWhoCameThrough)
            wallColliders.ForEach(wc =>
            {
                if (Physics.GetIgnoreCollision(c, wc))
                    Debug.Log($"{c} with {wc} = true");
            });
    }

    public void ResetWallColliders()
    {
        List<Collider> collidersTemp = collidersWhoCameThrough.ToList();
        collidersTemp.ForEach(c =>
        {
            if (c == null)
                CollidersWhoCameThrough.Remove(c);
        });
        
        CollidersWhoCameThrough.ForEach(c => wallColliders.ForEach(w => Physics.IgnoreCollision(c, w, true)));
        CollidersWhoCameThrough.Clear();
    }
}