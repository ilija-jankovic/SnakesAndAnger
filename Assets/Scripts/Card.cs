﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    //desription of card
    private string _description;
    //Player that picked up the card
    //protected Player _owner;

    /// <summary>
    /// constructor for Card
    /// </summary>
    /// <param name="does">Card description</param>
    public Card(string does)
    {
        _description = does;
    }


    public Player Owner
    {
        get
        {
            foreach (Player player in GameManager.Players)
                if (player.UsableCards.Contains(this))
                    return player;
            return null;
        }
    }


    //Methods

    /// <summary>
    /// Uses the card, each card has a different use function
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// gets the description of the card
    /// </summary>
    /// <returns></returns>
    public virtual string GetDescription()
    {
        return _description;
    }

    /// <summary>
    /// gets image that pops up beside text
    /// </summary>
    public virtual Texture2D Icon
    {
        get { return null; }
    }

    protected Texture2D GetTextureFromSprite(string path)
    {
        var sprite = Resources.Load<Sprite>(path);
        var croppedTexture = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height);
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                (int)sprite.textureRect.y,
                                                (int)sprite.textureRect.width,
                                                (int)sprite.textureRect.height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        return croppedTexture;
    }
}
