using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Properties that can have houses built on them.
public class Street : Property
{
    [SerializeField]
    private Color _colour;
    [SerializeField]
    private ushort _rent;
    [SerializeField]
    private ushort _housePrice;
    [SerializeField]
    private ushort _firstHouseValue;
    [SerializeField]
    private ushort _secondHouseValue;
    [SerializeField]
    private ushort _thirdHouseValue;
    [SerializeField]
    private ushort _fourthHouseValue;
    [SerializeField]
    private ushort _hotelValue;

    private byte _houses;
    private ushort[] _houseValues;

    List<GameObject> houseObjects = new List<GameObject>();
    GameObject banner;

    public override void Awake()
    {
        base.Awake();
        _houseValues = new ushort[] { _firstHouseValue, _secondHouseValue, _thirdHouseValue, _fourthHouseValue, _hotelValue };

        banner = GameObject.CreatePrimitive(PrimitiveType.Plane);
        banner.transform.parent = transform;
        banner.transform.localScale = new Vector3(transform.localScale.x / 100, 1, transform.localScale.z / 800);
        banner.transform.localEulerAngles = Vector3.zero;
        banner.transform.localPosition = new Vector3(0,0.55f,0.5f-5*banner.transform.localScale.z);

        banner.GetComponent<Renderer>().material.color = Colour;

        _titleMesh.transform.localPosition = new Vector3(0, 0, banner.transform.localPosition.z-5*banner.transform.localScale.z-0.05f);
    }

    public Color Colour
    {
        get { return _colour; }
    }

    //displays amount received by different number of houses
    public override string Description()
    {
        string houseValues = "\n";
        for (byte i = 1; i <= 4; i++)
            houseValues += "With " + i + " House" + (i != 1 ? "s" : "") + ": " + "$" + _houseValues[i - 1] + "\n";
        houseValues += "With HOTEL: $" + _houseValues[4] + "\n";
        return "Price: $" + Price + "\nRent: $" + _rent + houseValues + "One House Costs: $" + _housePrice + "\nMortgage Value: $" + _morgVal;
    }

    public override ushort PaymentPrice()
    {
        return _houses == 0 ? _rent : _houseValues[_houses - 1];
    }

    public void BuildHouse()
    {
        if (/*CanBuildHouses() && */_houses < 5)
        {
            _houses++;
            //create 3D models
            if (_houses <= 4)
            {
                GameObject prefab = Resources.Load("house2") as GameObject;
                GameObject house = Instantiate(prefab, banner.transform);
                house.transform.localScale = new Vector3(75, 150, 525);
                house.transform.localPosition = new Vector3(-2.8f + houseObjects.Count * 2.25f, 3.75f, 0.4f);
                houseObjects.Add(house);
            }
            else
            {
                GameObject prefab = Resources.Load("house") as GameObject;
                GameObject house = Instantiate(prefab, banner.transform);
                house.transform.localScale = new Vector3(100f, 200f, 750f);
                house.transform.localPosition = new Vector3(0.9f, 5f, 0f);

                foreach (GameObject obj in houseObjects)
                    Destroy(obj);

                houseObjects.Clear();
                houseObjects.Add(house);
            }
        }
    }

    public void RemoveHouse()
    {
        throw new NotImplementedException();
    }

    //player must own all properties of the same colour to build houses
    public bool CanBuildHouses()
    {
        throw new NotImplementedException();
    }

    public override void Mortgage()
    {
        base.Mortgage();
        RemoveHousesOfThisColour();
    }

    public byte Houses
    {
        get { return _houses; }
    }

    public bool HasHotel
    {
        get { return _houses == 5; }
    }

    public override void DisplayOptions()
    {
        throw new NotImplementedException();
    }

    public override void ReturnToBank()
    {
        base.ReturnToBank();
        Street[] streets = GameObject.FindObjectsOfType<Street>();
        foreach (Street street in streets)
            street.RemoveHousesOfThisColour();
    }

    private void RemoveHousesOfThisColour()
    {
        Street[] streets = GameObject.FindObjectsOfType<Street>();
        foreach (Street street in streets)
            if (street.Colour == Colour)
                for (byte i = 0; i < street.Houses; i++)
                    street.RemoveHouse();
    }
}
