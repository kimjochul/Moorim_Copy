using System.Collections.Generic;
using UnityEngine;

public class Order
{
    public int id;
    
    // 주문한 레시피와 개수
    public Dictionary<Recipe, int> recipes;
    
    // 주문한 게스트
    public GuestData orderer;

    public Order(Dictionary<Recipe, int> recipes, GuestData orderer)
    {
        this.orderer = orderer;
        this.recipes = recipes;
    }
}
