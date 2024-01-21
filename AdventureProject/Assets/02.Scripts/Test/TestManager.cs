using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField] int _invenCount = 2;

    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private List<ItemInfo> _infos;

    [SerializeField] GameObject _invenPrefab;

    [SerializeField] Canvas _canvas;

    #region Unity Methods
    public void Awake()
    {
        InvenManager.Instance.Init();

        TestInvenMake();
        TestInvenUIMake();
    }

    public void Update()
    {
        HandleKeyInput();
    }
    #endregion

    #region Private Methods
    private void TestInvenMake()
    {
        for (int i = 0; i < _invenCount; ++i)
        {
            Inventory inven = new Inventory();
            inven.Index = i;

            for (int j = 0; j < Define.Inven.INVEN_MAX; ++j)
            {
                if (Random.Range(0, 100) > 50)
                {
                    Sprite sprite = GetRandomSprite();
                    int amount = Random.Range(1, 9999);

                    Item item = new Item();
                    item.Amount = amount;
                    item.Info = GetRandomItemInfo();
                    item.SlotIndex = j;

                    inven.AddItem(item);
                }
            }
            InvenManager.Instance.AddInvnetory(inven);
        }
    }

    private void TestInvenUIMake()
    {
        _canvas = GameObject.FindObjectOfType<Canvas>();
        if (!_canvas)
        {
            _canvas = new Canvas();
        }

        for(int i = 0; i < _invenCount; ++i)
        {
            GameObject go = Instantiate(_invenPrefab, _canvas.transform);

            //UI_Inventory ui_Inven = go.GetOrAddComponent<UI_Inventory>();
            //ui_Inven.InvenIndex = i;
            
            go.name = $"{_invenPrefab.name}_{i + 1}";
        }
    }
    private ItemInfo GetRandomItemInfo()
    {
        int min = 0;
        int max = _infos.Count;
        int index = Random.Range(min, max);

        Debug.Log(index);
        return _infos[index];
    }

    private Sprite GetRandomSprite()
    {
        int min = 0;
        int max = _sprites.Count;

        return _sprites[Random.Range(min, max)];
    }

    private void HandleKeyInput()
    {
        if (Input.GetKeyDown("i"))
        {
        }

        if (Input.GetKeyDown("e"))
        { 
            
        }
    }
    #endregion
}
