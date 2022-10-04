using UnityEngine;
using UnityEngine.UI;
using SSystem;

[ExecuteInEditMode]
public class TestSave : MonoBehaviour
{
    public int Id;
    public string Name;
    public float Speed;

    [SerializeField] private InputField _idField;
    [SerializeField] private InputField _nameField;
    [SerializeField] private InputField _speedField;
    [SerializeField] private Text _path;

    private void Awake()
    {
        _path.text = Application.persistentDataPath;
        _idField.onValueChanged.AddListener(SetID);
        _nameField.onValueChanged.AddListener(SetName);
        _speedField.onValueChanged.AddListener(SetSpeed);
    }

    [ContextMenu("Save")]
    public void Save()
    {
        var model = new TestModel()
        {
            ID = Id,
            Name = Name,
            Speed = Speed
        };

        SaveSystem.Save(model);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        var model = SaveSystem.Load<TestModel>();

        Id = model.ID;
        Name = model.Name;
        Speed = model.Speed;

        _idField.text = Id.ToString();
        _nameField.text = Name;
        _speedField.text = Speed.ToString();
    }

    public void SetID(string id)
    {
        Id = int.Parse(id);
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetSpeed(string speed)
    {
        Speed = float.Parse(speed);
    }
}

[System.Serializable]
public class TestModel
{
    public int ID;
    public string Name;
    public float Speed;
}