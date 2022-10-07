using UnityEngine;
using UnityEngine.UI;
using SSystem;

[ExecuteInEditMode]
public class TestSave : MonoBehaviour
{
    public TestModel TestModel;

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
        SaveSystem.Save(TestModel);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        var model = SaveSystem.Load<TestModel>();

        TestModel = model;

        _idField.text = TestModel.Int.ToString();
        _nameField.text = TestModel.String;
        _speedField.text = TestModel.Float.ToString();
    }

    public void SetID(string id)
    {
        TestModel.Int = int.Parse(id);
    }

    public void SetName(string name)
    {
        TestModel.String = name;
    }

    public void SetSpeed(string speed)
    {
        TestModel.Float = float.Parse(speed);
    }
}

[System.Serializable]
public class TestModel
{
    public int Int;
    public string String;
    public float Float;
    public double Double;
    public SubTestModel Test = new SubTestModel();
    public bool Bool;
    public byte Byte;
    public sbyte SByte;
    public short Short;
    public ushort UShort;
    public uint UInt;
    public long Long;
    public char Char;
    public object Object;
    public decimal Decimal;
}

[System.Serializable]
public class SubTestModel
{
    public int Int;
    public string String;
    public float Float;
    public double Double;
}

public enum EnumAs
{
    one,
    two,
    three
}