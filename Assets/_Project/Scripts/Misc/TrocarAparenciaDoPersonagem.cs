using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class TrocarAparenciaDoPersonagem : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // The name of the sprite sheet to use
    [SerializeField] private Texture2D spriteSheetTexture;

    // The name of the currently loaded sprite sheet
    private string LoadedSpriteSheetName;

    // The dictionary containing all the sliced up sprites in the sprite sheet
    private Dictionary<string, Sprite> spriteSheet;

    //Getters
    public Texture2D SpriteSheetTexture
    {
        get => spriteSheetTexture;
        set => spriteSheetTexture = value;
    }

    // -----------------------------------------------------------------------------------------
    // awake method to initialisation
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        this.LoadSpriteSheet();
    }

    // -----------------------------------------------------------------------------------------
    // Runs after the animation has done its work
    private void LateUpdate()
    {
        // Check if the sprite sheet name has changed (possibly manually in the inspector)
        if (this.LoadedSpriteSheetName != this.spriteSheetTexture.name)
        {
            // Load the new sprite sheet
            this.LoadSpriteSheet();
        }

        // Swap out the sprite to be rendered by its name
        // Important: The name of the sprite must be the same!
        this.spriteRenderer.sprite = this.spriteSheet[this.spriteRenderer.sprite.name];
    }

    // -----------------------------------------------------------------------------------------
    // Loads the sprites from a sprite sheet
    private void LoadSpriteSheet()
    {
        // Load the sprites from a sprite sheet file (png). 
        // Note: The file specified must exist in a folder named Resources
        string spritesheetfolder = "Characters";
        string spritesheetfilepath = Path.Combine(spritesheetfolder, spriteSheetTexture.name);
        var sprites = Resources.LoadAll<Sprite>(spritesheetfilepath);

        if (sprites.Count() == 0)
        {
            spritesheetfilepath = Path.Combine(spritesheetfolder, "Personagem1");
            sprites = Resources.LoadAll<Sprite>(spritesheetfilepath);
        }

        this.spriteSheet = sprites.ToDictionary(x => x.name, x => x);

        // Remember the name of the sprite sheet in case it is changed later
        this.LoadedSpriteSheetName = this.spriteSheetTexture.name;
    }
}
