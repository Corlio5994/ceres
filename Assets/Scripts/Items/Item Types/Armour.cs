[System.Serializable]
public class Armour : Item {
    public ArmourSlot armourSlot;
    public float defence;
    
    public Armour() {
        category = "Armour";
    }
}

public enum ArmourSlot {
    Shield,
    Head,
    Chest, 
    Legs,
    Feet
}