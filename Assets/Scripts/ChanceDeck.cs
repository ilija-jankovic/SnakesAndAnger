using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceDeck
{
    private List<Card> _deck;

    public ChanceDeck()
    {
        _deck = new List<Card>();
        //Initialise cards, storing in deck
        InitialiseDeck();
        //Shuffle Deck
        ShuffleDeck();
        //shuffledList = myList.OrderBy( x => Random.value ).ToList( );
        //listOfThings = listOfThings.OrderBy(i => Guid.NewGuid()).ToList();
    }

    private void InitialiseDeck()
    {
        //CardCollect Type Cards
        _deck.Add(new CardCollect("Bank pays you dividend of $50", 50, 1));
        _deck.Add(new CardCollect("Your building and loan matures - Collect $150", 150, 1));
        _deck.Add(new CardCollect("You have won a crossword competition - Collect $100", 100, 1));
        _deck.Add(new CardCollect("Bank error in your favour - Collect $200", 200, 1));
        _deck.Add(new CardCollect("From sale of stock you get $50", 50, 1));
        _deck.Add(new CardCollect("Holiday Fund matures - Receive $100", 10, 1));
        _deck.Add(new CardCollect("Income tax refund - Collect $20", 20, 1));
        _deck.Add(new CardCollect("It is your birthday - Collect $10", 10, 1));
        _deck.Add(new CardCollect("Life insurance matures - Collect $100", 10, 1));
        _deck.Add(new CardCollect("Receive $25 consultancy fee", 25, 1));
        _deck.Add(new CardCollect("You have won second prize in a beauty contest - Collect $10", 10, 1));
        _deck.Add(new CardCollect("Grand Opera Night - Collect $50 from every player for opening night seats", 25, 2));

        //CardPay Type Cards
        _deck.Add(new CardPay("Pay poor tax of $15", 15, 1));
        _deck.Add(new CardPay("Doctor's fee _ Pay $50 ", 50, 1));
        _deck.Add(new CardPay("Pay hospital fees of $100", 100, 1));
        _deck.Add(new CardPay("You have been elected Chairman of the Board - Pay each player $50", 50, 2));
        _deck.Add(new CardPay("Make general repairs on all your properties - For each house pay $25 - For each hotel $100", 25, 3));

        //CardMove Type Cards
        //Where tile is null must be replaced with tile in comments
        _deck.Add(new CardMove("Advance to Go (Collect $200)", null, 1)); //tile00
        _deck.Add(new CardMove("Advance to Illinois Ave - If you pass Go, collect $200", null, 1)); //tile24
        _deck.Add(new CardMove("Advance to St. Charles Place - If you pass Go, collect $200", null, 1)); //tile11
        _deck.Add(new CardMove("Take a trip to Reading Railroad - If you pass Go, collect $200", null, 1)); //tile05
        _deck.Add(new CardMove("Take a walk on the Boardwalk - Advance token to Boardwalk", null, 1)); //tile29
        _deck.Add(new CardMove("Advance token to nearest Utility. If unowned, you man buy it from the Bank. If owned, throw dice and pay owner a total ten times the amount thrown", 2)); 
        _deck.Add(new CardMove("Advance token to the nearest Railroad and pay the owner twice the rental to which they they are otherwise entitled. If Railroad is unowned, you may buy it from the Bank", 3)); 
        _deck.Add(new CardMove("Go back 3 spaces", 4));
        _deck.Add(new CardMove("Go to Jail - Go directly to Jail - Do not pass Go, do not collect $200", 5));

        //CardGetOutOfJail Type Cards
        _deck.Add(new CardGetOutOfJail("Get out of jail free"));
        _deck.Add(new CardGetOutOfJail("Get out of jail free"));
    }

    public void ShuffleDeck()
    {

    }

    public Card DrawCard()
    {
        return _deck[1];
    }


}
