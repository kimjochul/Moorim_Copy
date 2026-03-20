using UnityEngine;

public interface IFoodReceptacle
{
    Transform GetFoodDropTransform();
    void PlaceFood();
    void TakePlacedFood();
}
