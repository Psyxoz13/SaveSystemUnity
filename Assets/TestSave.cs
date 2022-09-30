using UnityEngine;

[ExecuteInEditMode]
public class TestSave : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private float _speed;

    [ContextMenu("Save")]
    public void Save()
    {
        var model = new TestModel()
        {
            ID = _id,
            Name = _name,
            Speed = _speed
        };

        SaveSystem.Save(model);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        var model = SaveSystem.Load<TestModel>();

        _id = model.ID;
        _name = model.Name;
        _speed = model.Speed;
    }

    private void Update()
    {

    }
}

[System.Serializable]
public class TestModel
{
    public int ID;
    public string Name;
    public float Speed;
}