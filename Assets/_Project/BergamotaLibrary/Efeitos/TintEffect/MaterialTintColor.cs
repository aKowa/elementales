using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    public class MaterialTintColor : MonoBehaviour
    {
        private Material material;
        private Color materialTintColor;

        private void Awake()
        {
            materialTintColor = new Color(1, 0, 0, 0);
            SetMaterial(GetComponent<SpriteRenderer>().material);
        }

        /// <summary>
        /// Seta o material.
        /// </summary>
        /// <param name="material">Material</param>
        public void SetMaterial(Material material)
        {
            this.material = material;
        }

        /// <summary>
        /// Seta a cor que vai afetar o material.
        /// </summary>
        /// <param name="color">Cor</param>
        public void SetTintColor(Color color)
        {
            materialTintColor = color;
            material.SetColor("_Tint", materialTintColor);
        }
    }
}
