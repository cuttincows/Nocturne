using UnityEngine;

[CreateAssetMenu(fileName = "FishDefinition", menuName = "Nocturne/Fish Definition")]
public class FishDefinition : ScriptableObject {
    public string displayName;

    [Header("Sprites")]
    public Sprite rawSprite;
    public Sprite cookedSprite;

    [Header("Cooking")]

    public float cookTime = 5f;

    [Header("Values")]
 
    public int fuelValue = 50;
  
    public int foodValue = 50;
}