using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public GameObject foodPrefab;

    public void AddFoodToHotbar()
    {
        // Instantiate a new food item in the hotbar
        Instantiate(foodPrefab, transform.position, Quaternion.identity);
    }
}
