using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleLoadManager : MonoBehaviour
{
    IEnumerator Start()
    {
        AssetBundle asset = AssetBundle.LoadFromFile("Bundle/player");

        if(asset == null)
        {
            yield break;
        }

        GameObject player = asset.LoadAsset<GameObject>("SK_CosmoBunny_with_bones_in_dress Variant");
        GameObject player_ = Instantiate(player);

        player_.transform.position = Vector3.zero;
    }

}
