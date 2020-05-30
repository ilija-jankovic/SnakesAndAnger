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

    private ushort[] _houseValues;

    List<GameObject> houseObjects = new List<GameObject>();
    GameObject banner;

    public override void Awake()
    {
        base.Awake();
        _houseValues = new ushort[] { _firstHouseValue, _secondHouseValue, _thirdHouseValue, _fourthHouseValue, _hotelValue };

        //display property colour
        banner = GameObject.CreatePrimitive(PrimitiveType.Plane);
        banner.name = "Banner";
        banner.transform.parent = transform;
        banner.transform.localScale = new Vector3(transform.localScale.x / 100, 1, transform.localScale.z / 800);
        banner.transform.localEulerAngles = Vector3.zero;
        banner.transform.localPosition = new Vector3(0,0.55f,0.5f-5*banner.transform.localScale.z);

        banner.GetComponent<Renderer>().material.color = Colour;

        SpriteRenderer border = new GameObject("BannerBorder").AddComponent<SpriteRenderer>();
        border.transform.SetParent(transform);
        border.transform.localEulerAngles = new Vector3(90, 0, 0);
        border.transform.localScale = new Vector3(0.2f, 0.075f, 0);
        border.transform.localPosition = new Vector3(0f, 0.58f, 0f);

        border.sprite = Resources.Load<Sprite>("border");

        TitleMesh.transform.localPosition = new Vector3(0, 0, banner.transform.localPosition.z-5*banner.transform.localScale.z-0.05f);
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
        return Houses == 0 ? _rent : _houseValues[Houses - 1];
    }

    public void BuildHouse()
    {
        if (CanBuildHouse())
        {
            //create 3D models
            if (Houses < 4)
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

                //disable houses
                foreach (GameObject obj in houseObjects)
                    obj.SetActive(false);

                houseObjects.Add(house);
            }
            Owner.RemoveFunds(_housePrice);

            GameManager.UpdateBuyButtonInteractibility();
            MenuManager.UpdateInventoryData(Owner);
        }
    }

    public bool CanSellHouse()
    {
        if (Houses == 0)
            return false;

        //must remove houses in reverse order to being built
        List<Street> streetsOfThisColour = GetStreetsOfThisColour();
        byte thisIndex = (byte)streetsOfThisColour.IndexOf(this);
        if (thisIndex == streetsOfThisColour.Count - 1)
            return streetsOfThisColour[thisIndex].Houses == streetsOfThisColour[0].Houses;
        else
            return streetsOfThisColour[thisIndex].Houses - streetsOfThisColour[thisIndex+1].Houses == 1;
    }

    public void SellHouse()
    {
        GameObject houseToRemove = houseObjects[houseObjects.Count - 1];
        houseObjects.Remove(houseToRemove);
        Destroy(houseToRemove);

        //set houses to be visible incase only hotel was visible
        foreach (GameObject obj in houseObjects)
            obj.SetActive(true);

        Owner.AddFunds(SellHousePrice);

        GameManager.UpdatePayButtonInteractibility();
        GameManager.UpdateBuyButtonInteractibility();
        MenuManager.UpdateInventoryData(Owner);
    }

    //player must own all properties of the same colour to build houses
    public bool CanBuildHouse()
    {
        if (Houses == 5 || Owner == null || Owner.GetBalance() < _housePrice)
            return false;

        List<Street> streetsOfThisColour = GetStreetsOfThisColour();
        foreach (Street street in streetsOfThisColour)
        {
            if (street.Mortgaged || street.Owner != Owner)
                return false;
        }

        //must build houses sequentially on each property - it is a Monopoly rule
        byte thisIndex = (byte)streetsOfThisColour.IndexOf(this);
        if (thisIndex == 0)
            return streetsOfThisColour[thisIndex].Houses == streetsOfThisColour[streetsOfThisColour.Count - 1].Houses;
        else
            //checks if previous tile has 1 more house
            return streetsOfThisColour[thisIndex - 1].Houses - streetsOfThisColour[thisIndex].Houses == 1;
    }

    public override bool CanMortagage()
    {
        List<Street> streetsOfThisColour = GetStreetsOfThisColour();
        foreach (Street street in streetsOfThisColour)
            if (street.Houses > 0)
                return false;
        return base.CanMortagage();
    }

    public byte Houses
    {
        get { return (byte)houseObjects.Count; }
    }

    public bool HasHotel
    {
        get { return Houses == 5; }
    }

    public override void ReturnToBank()
    {
        List<Street> streetsOfThisColour = GetStreetsOfThisColour();
        foreach (Street street in streetsOfThisColour)
            for (byte i = 0; i < street.Houses; i++)
                street.SellHouse();
        base.ReturnToBank();
    }

    private List<Street> GetStreetsOfThisColour()
    {
        Street[] streets = GameObject.FindObjectsOfType<Street>();
        List<Street> sameColour = new List<Street>();
        foreach (Street street in streets)
            if (street.Colour == Colour)
                sameColour.Add(street);

        //sort streets from lowest to heighest in position
        sameColour.Sort(delegate (Street s1, Street s2) {
            return s1.name.CompareTo(s2.name);
        });

        return sameColour;
    }

    public ushort HousePrice
    {
        get { return _housePrice; }
    }

    public ushort SellHousePrice
    {
        get { return (ushort)(HousePrice / 2); }
    }
}
