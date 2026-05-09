using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLevelSO", menuName = "SaveLoad/PlayerLevel")]
public class PlayerLevelSO : ScriptableObject, ISaveableSO
{
    public string SaveID => "PlayerLevel";
    public int MaxUnlockedLevel = 0;

    public void PopulateSaveData(GameData data)
    {
        data.MaxUnlockedLevel = MaxUnlockedLevel;
    }

    public void LoadFromSaveData(GameData data)
    {
        MaxUnlockedLevel = data.MaxUnlockedLevel;
    }

    public void UpdateLevel(int newLevel)
    {
        if(MaxUnlockedLevel >= newLevel) return;

        MaxUnlockedLevel = newLevel;
    }
}