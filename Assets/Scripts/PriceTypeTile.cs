using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceTypeTile : Tile
{
    [SerializeField]
    private ushort _price;
    private TextMesh _priceMesh;
    public override void Awake()
    {
        base.Awake();

        _priceMesh = new GameObject("Price").AddComponent<TextMesh>();
        _priceMesh.anchor = TextAnchor.UpperCenter;
        _priceMesh.alignment = TextAlignment.Center;

        _priceMesh.text = "$" + Price;

        _priceMesh.color = Color.black;
        _priceMesh.fontSize = 12;
        _priceMesh.transform.parent = transform;
        _priceMesh.transform.localEulerAngles = new Vector3(90, 0, 0);
        _priceMesh.transform.localScale = new Vector3(0.1f, 0.0625f, 1);
        _priceMesh.transform.localPosition = new Vector3(0, 0, -0.30f);
    }

    public ushort Price
    {
        get { return _price; }
        set
        {
            _price = value;
            PriceMesh.text = "$"+_price;
        }
    }

    public TextMesh PriceMesh
    {
        get { return _priceMesh; }
    }
}
