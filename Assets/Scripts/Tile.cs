using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all Monopoly tiles.
public abstract class Tile : MonoBehaviour
{
    private TileLink _tileLink;
    private Player _player;
    [SerializeField]
    private string _title;
    private TextMesh _titleMesh;
    [SerializeField]
    private Sprite _icon;
    private SpriteRenderer _iconRenderer;
    private SpriteRenderer _border;
    //options to display when the player lands on this tile
    public virtual void Awake()
    {
        //create gameobject that shows the tile's icon
        _iconRenderer = new GameObject("TileIcon").AddComponent<SpriteRenderer>();
        _iconRenderer.transform.SetParent(transform);
        _iconRenderer.transform.localEulerAngles = new Vector3(90,0,0);
        _iconRenderer.transform.localScale = new Vector3(0.05f, 0.05f, 0f);
        _iconRenderer.transform.localPosition = new Vector3(0f,0.55f,-0.05f);

        _iconRenderer.sprite = _icon;

        _border = new GameObject("Border").AddComponent<SpriteRenderer>();
        _border.transform.SetParent(transform);
        _border.transform.localEulerAngles = new Vector3(90, 0, 0);
        _border.transform.localScale = new Vector3(0.2f, 0.125f, 0);
        _border.transform.localPosition = new Vector3(0f, 0.58f, 0f);

        _border.sprite = Resources.Load<Sprite>("border");

        //title
        _titleMesh = new GameObject("Title").AddComponent<TextMesh>();
        _titleMesh.anchor = TextAnchor.UpperCenter;
        _titleMesh.alignment = TextAlignment.Center;

        _titleMesh.text = _title.Replace('$', '\n');
        _title = _title.Replace('$', ' ');

        _titleMesh.color = Color.black;
        _titleMesh.fontSize = 12;
        _titleMesh.transform.parent = transform;
        _titleMesh.transform.localEulerAngles = new Vector3(90, 0, 0);
        _titleMesh.transform.localScale = new Vector3(0.1f, 0.0625f, 1);
        _titleMesh.transform.localPosition = new Vector3(0, 0, 0.4f);
    }

    public void Start()
    {
        //remove border if corner tile
        if (Array.IndexOf(GameManager.Tiles, this) % 10 == 0)
            Border.gameObject.SetActive(false);
    }

    //adds snake/ladder
    public void AddTileLink(TileLink link)
    {
        _tileLink = link;
    }

    public void MoveAlongTileLink()
    {
        if (TileLink != null && TileLink.Head == this)
        {
            Player.Move(TileLink.Tail);
            TileLink.Destroy();
        }
    }

    //returns the player that is on this current tile. If multiple players, chooses the current player
    public Player Player
    {
        get { return GameManager.CurrentPlayer.Position == this ? GameManager.CurrentPlayer : null; }
    }

    //checks if there is a player on this tile
    public bool hasPlayer()
    {
        return _player == null;
    }

    public TileLink TileLink
    {
        get { return _tileLink; }
    }

    public Sprite Icon
    {
        get { return _icon; }
        set
        {
            _icon = value;
            IconRenderer.sprite = _icon;
        }
    }

    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            TitleMesh.text = _title;
        }
    }

    public TextMesh TitleMesh
    {
        get { return _titleMesh; }
    }

    public SpriteRenderer IconRenderer
    {
        get { return _iconRenderer; }
    }

    public SpriteRenderer Border
    {
        get { return _border; }
    }
}
