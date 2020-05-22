using static EnumsForCards.cardPay;
using static EnumsForCards.cardMove;
using static EnumsForCards.cardCollect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChanceDeck
{
    private static List<Card> _deck;

    //each cardClass has different types within them. 
    //These types can be found within the class EnumsForCards
    //this was done so that it's easier to use the enums between different classes

    [RuntimeInitializeOnLoadMethod]
    private static void InitialiseDeck()
    {

        _deck = new List<Card>();


        //CardCollect Type Cards
        _deck.Add(new CardCollect("Bank pays you dividend of $50", 50, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("Your building and loan matures - Collect $150", 150, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("You have won a crossword competition - Collect $100", 100, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("Bank error in your favour - Collect $200", 200, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("From sale of stock you get $50", 50, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("Holiday Fund matures - Receive $100", 100, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("Income tax refund - Collect $20", 20, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("It is your birthday - Collect $10", 10, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("Life insurance matures - Collect $100", 100, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("Receive $25 consultancy fee", 25, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("You have won second prize in a beauty contest - Collect $10", 10, EnumsForCards.cardCollect.fromBank));
        _deck.Add(new CardCollect("Grand Opera Night - Collect $50 from every player for opening night seats", 50, EnumsForCards.cardCollect.fromPlayers));

        //CardPay Type Cards
        _deck.Add(new CardPay("Pay poor tax of $15", 15, EnumsForCards.cardPay.directRemove));
        _deck.Add(new CardPay("Doctor's fee _ Pay $50 ", 50, EnumsForCards.cardPay.directRemove));
        _deck.Add(new CardPay("Pay hospital fees of $100", 100, EnumsForCards.cardPay.directRemove));
        _deck.Add(new CardPay("You have been elected Chairman of the Board - Pay each player $50", 50, EnumsForCards.cardPay.payEachPlayer));
        _deck.Add(new CardPay("Make general repairs on all your properties - For each house pay $25 - For each hotel $100", 25, EnumsForCards.cardPay.forEachHotelAndHouseOwned));

        //CardMove Type Cards
        //Where tile is null must be replaced with tile in comments
        _deck.Add(new CardMove("Advance to Go (Collect $200)", GameObject.Find("tile00").GetComponent<Tile>(), EnumsForCards.cardMove.directMove)); //tile00
        _deck.Add(new CardMove("Advance to " + GameObject.Find("tile24").GetComponent<Tile>().GetComponent<Property>().Title + " - If you pass Go, collect $200", GameObject.Find("tile24").GetComponent<Tile>(), EnumsForCards.cardMove.directMove)); //tile24
        _deck.Add(new CardMove("Advance to St. " + GameObject.Find("tile11").GetComponent<Tile>().GetComponent<Property>().Title + " - If you pass Go, collect $200", GameObject.Find("tile11").GetComponent<Tile>(), EnumsForCards.cardMove.directMove)); //tile11
        _deck.Add(new CardMove("Take a trip to " + GameObject.Find("tile05").GetComponent<Tile>().GetComponent<Property>().Title + " - If you pass Go, collect $200", GameObject.Find("tile05").GetComponent<Tile>(), EnumsForCards.cardMove.directMove)); //tile05
        _deck.Add(new CardMove("Take a walk - Advance token to " + GameObject.Find("tile39").GetComponent<Tile>().GetComponent<Property>().Title, GameObject.Find("tile39").GetComponent<Tile>(), EnumsForCards.cardMove.directMove)); //tile39
        _deck.Add(new CardMove("Advance token to nearest Utility. If unowned, you man buy it from the Bank. If owned, throw dice and pay owner a total ten times the amount thrown", EnumsForCards.cardMove.closestUtility)); 
        _deck.Add(new CardMove("Advance token to the nearest Railroad and pay the owner twice the rental to which they they are otherwise entitled. If Railroad is unowned, you may buy it from the Bank", EnumsForCards.cardMove.closestTrainStation)); 
        _deck.Add(new CardMove("Go back 3 spaces", EnumsForCards.cardMove.moveBackThreeTiles));
        _deck.Add(new CardMove("Go to Jail - Go directly to Jail - Do not pass Go, do not collect $200", EnumsForCards.cardMove.goToJail));

        //CardGetOutOfJail Type Cards
        _deck.Add(new CardGetOutOfJail("Get out of jail free"));
        _deck.Add(new CardGetOutOfJail("Get out of jail free"));

        ShuffleDeck();
    }

    /// <summary>
    /// Shuffles the deck of cards
    /// </summary>
    public static void ShuffleDeck()
    {
        int count = _deck.Count;
        int last = count - 1;
        for (int i = 0; i < last; i++)
        {
            int r = UnityEngine.Random.Range(i, count);
            Card temp = _deck[i];
            _deck[i] = _deck[r];
            _deck[r] = temp;
        }
    }

    /// <summary>
    /// Draws card
    /// </summary>
    /// <returns>returns the card drawn</returns>
    public static Card DrawCard()
    {
        if (_deck != null)
        {
            Card temp2 = _deck[0];
            _deck.Remove(temp2);
            return temp2;
        }
        else return null;
    }

    /// <summary>
    /// Places card at bottom of deck
    /// </summary>
    /// <param name="temp3"></param>
    public static void PlaceUnderDeck(Card temp3)
    {
        _deck.Add(temp3);
    }




}
