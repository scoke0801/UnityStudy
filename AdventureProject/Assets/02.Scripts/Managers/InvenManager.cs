using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenManager : Singleton<InvenManager>
{
    #region Variables
    // Key: InvenIndex, Value: Inventory
    private Dictionary<int, Inventory> _invens;
    #endregion

    #region Public Methods
    public override void Init()
    {
        _invens = new Dictionary<int, Inventory>();

        DoMakeInven_Test();
    }

    public void AddInvnetory(Inventory inventory)
    {
        _invens.TryAdd(inventory.Index, inventory);
    }

    public Inventory GetInventory(int index)
    {
        if(index < 0 || index >= _invens.Count) { return null; }

        return _invens[index];
    }
    #endregion

    #region Private Methods
    private void DoMakeInven_Test()
    {
    }
    #endregion
}
