 
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

        EditorUtility.DisplayDialog("에셋 번들 빌드", "빌드 완료", "확인");

    }
}
