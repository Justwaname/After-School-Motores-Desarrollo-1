using UnityEngine;

public class KeyItem : InteractableObject
{
    public override void Interact()
    {
        Debug.Log("Objeto recogido: " + gameObject.name);

        GameManager.Instance.ItemCollected();

        Destroy(gameObject);
    }

    public override string GetInteractText()
    {
        return gameObject.name + " [E] to pick up";
    }
}
