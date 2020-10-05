using System;
using System.Collections.Generic;
using AOI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NodeManager : MonoBehaviour
{
    private const int SIZE = 10;
    private const int MAX_COUNT = 200;

    [SerializeField] private GameObject mask;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform cellParent;

    private Dictionary<int, Cell> _cells;
    
    [Header("Debug")]
    [SerializeField] private InputField posInputField;
    [SerializeField] private InputField idInputField;
    
    private AOIManager _aoiManager;
    private int _baseId;
    
    // Start is called before the first frame update
    void Start()
    {
        _aoiManager = AOIUtil.CreateBlackBox("test");
        _cells = new Dictionary<int, Cell>();
        _baseId = 100;
    }

    private void AddNode(float x, float y)
    {
        var entity = _aoiManager.CreateLogicEntity(_baseId++);
        entity.SetPos(x, y);
        
        GameObject go = Instantiate(prefab, cellParent, true) as GameObject;
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * 20.0f, y * 20.0f);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.SetActive(true);

        var cell = go.GetComponent<Cell>();
        cell.Init(entity);
            
        _cells.Add(entity.GetId(), cell);
    }

    public void OnSearchButtonClicked()
    {
        float x = Random.Range(-SIZE, SIZE);
        float y = Random.Range(-SIZE, SIZE);
        float range = Random.Range(0.1f, 10.0f);
        IList<LogicEntity> list = DoSearch(new Vector2(x, y), range);

        // int id = Random.Range(100, 201);
        //
        // float range = Random.Range(0.1f, 10.0f);
        // IList<LogicEntity> list = DoSearch(this._blackBox.GetLogicEntityById(id), range);
        
    }

    private IList<LogicEntity> DoSearch(LogicEntity entity, float radius)
    {
        IList<LogicEntity> list = entity.GetLogicEntityInRange(radius);
        
        mask.GetComponent<RectTransform>().anchoredPosition = entity.GetPos() * 20.0f;
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(radius * 2 * 20, radius * 2 * 20);

        HashSet<int> set = new HashSet<int>();
        foreach (var t in list)
            set.Add(t.GetId());

        foreach (var pair in _cells)
        {
            if (set.Contains(pair.Key))
                pair.Value.TurnOn();
            else
                pair.Value.TurnOff();
            
            if (pair.Key == entity.GetId())
                pair.Value.TurnSelected();
        }

        return list;
    }

    private IList<LogicEntity> DoSearch(Vector2 pos, float radius)
    {
        IList<LogicEntity> list = _aoiManager.GetLogicEntitiesInRangeByCenter(pos, radius);

        mask.GetComponent<RectTransform>().anchoredPosition = pos * 20.0f;
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(radius * 2 * 20, radius * 2 * 20);

        HashSet<int> set = new HashSet<int>();
        foreach (var t in list)
            set.Add(t.GetId());

        foreach (var pair in _cells)
        {
            if (set.Contains(pair.Key))
                pair.Value.TurnOn();
            else
                pair.Value.TurnOff();
        }
        
        return list;
    }

    public void OnAddSomeButtonClicked()
    {
        List<Vector2> list = new List<Vector2>()
        {
            new Vector2(1, 5),
            new Vector2(2, 2),
            new Vector2(3, 1),
            new Vector2(5, 3),
            new Vector2(6, 6),
        };

        for (int i = 0; i < list.Count; ++i)
        {
            AddNode(list[i].x, list[i].y);
        }
        

        // for (int i = 0; i < MAX_COUNT; ++i)
        // {
        //     float x = Random.Range(-SIZE, SIZE);
        //     float y = Random.Range(-SIZE, SIZE);
        //     AddNode(x, y);
        // }
    }
    
    public void OnRemoveButtonClicked()
    {
        
    }

    public void OnAddButtonClicked()
    {
        string pos = posInputField.text;
        string[] arr = pos.Split(new[] {','});
        
        float x = Convert.ToSingle(arr[0].Trim());
        float y = Convert.ToSingle(arr[1].Trim());

        posInputField.text = "";

        if (x <= SIZE && x >= -SIZE && y <= SIZE && y >= -SIZE)
        {
            AddNode(x, y);
        }
    }

}
