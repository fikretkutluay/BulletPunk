// GameEnums.cs dosyasýnýn tamamý bu olsun:
public enum ControlType
{
    MoveUp,    // W
    MoveDown,  // S
    MoveLeft,  // A
    MoveRight, // D
    Skill1,    // Q
    Skill2,    // E
    Skill3,    // R
    Dash       // Space
}

[System.Serializable]
public class UpgradeOption
{
    public string upgradeName;
    public string description;
    public UpgradeType type;
}

public enum UpgradeType { Q_Damage, E_Damage, R_Damage, Health_Max }