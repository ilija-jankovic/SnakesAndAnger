using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : TileLink
{
    /// <summary>
    /// Constructor for the Snake Class
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    public Snake()
    {

    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }

    public override string GetDescription()
    {
        return "Snake can be placed up to " + _maxLength + " tiles.";
    }

    public override Texture2D Icon
    {
        get
        {
            Sprite sprite = Resources.Load("snake") as Sprite;
            var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                    (int)sprite.textureRect.y,
                                                    (int)sprite.textureRect.width,
                                                    (int)sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            return croppedTexture;
        }
    }
}
