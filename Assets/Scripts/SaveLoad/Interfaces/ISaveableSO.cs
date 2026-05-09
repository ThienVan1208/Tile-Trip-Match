public interface ISaveableSO
{
    string SaveID { get; }
    void PopulateSaveData(GameData data);
    void LoadFromSaveData(GameData data);
}