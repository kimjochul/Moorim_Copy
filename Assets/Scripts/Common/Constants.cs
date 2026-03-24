public class Constants
{
    public enum IngredientRarity
    {
        Normal,
        Epic,
    }
    public enum RecipeClass
    {
        Normal,
        Event,
    }
    public enum IngredientType
    {
        Vegetable,
        Meat,
        Spices,
    }
    public enum FoodType
    {
        Meal,
        Drink,
        Dessert,
        SideDish
    }
    
    public enum CookingMethod
    {
        None, Bake, Fry, Boil, Mix, Braise
    }
    public enum PlayerType
    {
        Myungwol, Sowol
    }
    public const float SPOIL_TIME = 180f;
    public const int SERVINGSLOT_MAXCOUNT = 8;
    public const int FRIDGE_SIZE = 4;
}