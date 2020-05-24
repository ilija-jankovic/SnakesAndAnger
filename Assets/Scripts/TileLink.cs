using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class TileLink : Card
{
    private Tile[] tiles = new Tile[2];
    protected byte _maxLength;

    //selected head from UI stored here
    private static Button chosenHead = null;

    public TileLink() : base(null)
    {
        _maxLength = (byte)UnityEngine.Random.Range(2, 10);
    }

    public override void Use()
    {
        MenuManager.SwitchToMenu(MenuManager.TileLinkOptions);
        MenuManager.SwitchToCamera(MenuManager.OverviewCamera);

        //gets width of a row of tiles to offset buttons
        float offset = 0;
        for (byte i = 0; i < 10; i++)
            offset += GameManager.Tiles[i].transform.localScale.x;
        offset /= 2;

        float scl = 4;
        float buttonSize = 20;

        for (byte i = 0; i < GameManager.Tiles.Length; i++)
        {
            //destroy prev button
            GameObject.Destroy(GameObject.Find("tilelinkbutton" + i));

            GameObject button = new GameObject("tilelinkbutton" + i);
            button.transform.parent = MenuManager.TileLinkOptions.transform;
            button.AddComponent<RectTransform>();
            button.AddComponent<Button>().onClick.AddListener(delegate { TileLinkButtonListener(button.GetComponent<Button>()); });
            button.AddComponent<Image>();

            Tile tile = GameManager.Tiles[i];
            button.transform.localPosition = new Vector2(scl * (tile.transform.localPosition.x + buttonSize/2 + offset), scl*(tile.transform.localPosition.z - buttonSize/2 - offset));

            button.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonSize, buttonSize);

            if(tile.TileLink != null)
            {
                button.GetComponent<Button>().interactable = false;
                button.GetComponent<Image>().color = chosenHead.colors.disabledColor;
            }
        }
    }

    private void TileLinkButtonListener(Button button)
    {
        if(chosenHead == null)
        {
            chosenHead = button;

            Button[] buttons = GameObject.FindObjectsOfType<Button>();
            foreach (Button other in buttons)
                if (other.name.StartsWith("tilelinkbutton")){
                    if (Mathf.Abs(GetTileIndexFromButton(chosenHead) - GetTileIndexFromButton(other)) > _maxLength || other == chosenHead) {
                        other.interactable = false;
                        other.GetComponent<Image>().color = chosenHead.colors.disabledColor;
                    }
                }
            return;
        }

        //set head of new tilelink
        tiles[0] = GameManager.Tiles[GetTileIndexFromButton(chosenHead)];
        //set tail of new tilelink
        tiles[1] = GameManager.Tiles[GetTileIndexFromButton(button)];

        //add tile link to both head and tail for easier referencing
        tiles[0].AddTileLink(this);
        tiles[1].AddTileLink(this);

        base.Use();
        MenuManager.SwitchToMenuWithInventory(MenuManager.TurnOptions);
        MenuManager.SwitchToCamera(MenuManager.MainCamera);
    }

    private int GetTileIndexFromButton(Button button)
    {
        return int.Parse(button.name.Substring("tilelinkbutton".Length, button.name.Length - "tilelinkbutton".Length));
    }

    public Tile Head
    {
        get { return tiles[0]; }
    }

    public Tile Tail
    {
        get { return tiles[1]; }
    }

    public byte MaxLength
    {
        get { return _maxLength; }
    }
}
