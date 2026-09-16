using UnityEngine;

public class KeyItem : InteractableObject
{
    // --- INTERACCIÓN ---
    public override void Interact()
    {
        Debug.Log("Objeto recogido: " + gameObject.name);

        GameManager.Instance.ItemCollected();

        Destroy(gameObject);
    }
    
    // --- TEXTO DE INTERACCIÓN ---
    public override string GetInteractText()
    {
        return gameObject.name + " [E] to pick up";
    }
}
