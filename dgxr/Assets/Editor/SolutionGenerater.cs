using UnityEditor;

public static class SolutionGenerater
{
    public static void Generate()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }
}