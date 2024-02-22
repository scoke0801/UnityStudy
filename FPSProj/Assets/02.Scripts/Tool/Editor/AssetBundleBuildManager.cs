 
using UnityEditor;
using System.IO;

public class AssetBundleBuildManager 
{

    [MenuItem("Tools/AssetBundle Build")]
    public static void AssetBundle()
    {
        string path = "./Bundle";

        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        EditorUtility.DisplayDialog("���� ���� ����", "���� �Ϸ�", "Ȯ��");

    }
}
