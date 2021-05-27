[System.Serializable]
public class Weapon : Item {
    public float damage;
    public float attackTime;
    public float swingRange;
    public WeaponType weaponType;
    public bool oneHanded;
    
    public Weapon () {
        category = "Weapon";
    }
}

public enum WeaponType {
    Sword, Spear, Axe
}