public interface ISaveable
{
    DataDefination GetDataID();
    void RegisterSaveData() => DataManager.instance.RegisterSaveData(this);
    void UnRegisterSaveData() => DataManager.instance.UnRegisterSaveData(this);
    void GetSaveData(Data data);
    void LoadData(Data data);
}