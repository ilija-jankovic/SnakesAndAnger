using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumsForCards
{
    public enum cardCollect { fromBank, fromPlayers };
    public enum cardPay { directRemove, payEachPlayer, forEachHotelAndHouseOwned };
    public enum cardMove { directMove, closestUtility, closestTrainStation, moveBackThreeTiles, goToJail };


}

