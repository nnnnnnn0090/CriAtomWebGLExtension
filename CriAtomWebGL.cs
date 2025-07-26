class CriAtomWebGL
{
    public static bool IsAcfLoaded { get; private set; } = false;
    public static bool IsAcbLoaded { get; private set; } = false;
    private static CriAtomCoroutineRunner _runner;
    
    private static Dictionary<string, CriAtomExAcb> loadedAcbs = new Dictionary<string, CriAtomExAcb>();
    
    public static CriAtomExAcb GetLoadedAcb(string acbName)
    {
        if (loadedAcbs.ContainsKey(acbName))
        {
            return loadedAcbs[acbName];
        }
        Debug.LogWarning($"ACB '{acbName}' は見つかりませんでした");
        return null;
    }
    
    public static void RegisterAcf(string acfPath)
    {
        IsAcfLoaded = false;
        
        if (_runner == null)
        {
            GameObject go = new GameObject("CriAtomCoroutineRunner");
            GameObject.DontDestroyOnLoad(go);
            _runner = go.AddComponent<CriAtomCoroutineRunner>();
        }
        
        _runner.StartCoroutine(RegisterAcfInternal(acfPath));
    }
    
    public static void RegisterAcb(string acbPath)
    {
        IsAcbLoaded = false;
        
        if (_runner == null)
        {
            GameObject go = new GameObject("CriAtomCoroutineRunner");
            GameObject.DontDestroyOnLoad(go);
            _runner = go.AddComponent<CriAtomCoroutineRunner>();
        }
        
        _runner.StartCoroutine(RegisterAcbInternal(acbPath));
    }
    
    private static IEnumerator RegisterAcbInternal(string acbPath)
    {
        UnityWebRequest request = UnityWebRequest.Get(acbPath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("ACBロード失敗: " + request.error);
            yield break;
        }

        byte[] acbBytes = request.downloadHandler.data;
        
        string fileName = System.IO.Path.GetFileNameWithoutExtension(acbPath);
        
        CriAtomExAcb acb = CriAtomExAcb.LoadAcbData(acbBytes, null, fileName);
        
        if (acb == null)
        {
            Debug.LogError("ACB登録失敗（バイナリ）: " + acbPath);
        }
        else
        {
            Debug.Log("ACB登録成功（WebGLバイナリ）: " + acbPath);
            loadedAcbs[fileName] = acb;
            IsAcbLoaded = true;
        }
    }
    
    private static IEnumerator RegisterAcfInternal(string acfPath)
    {
        UnityWebRequest request = UnityWebRequest.Get(acfPath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("ACFロード失敗: " + request.error);
            yield break;
        }

        byte[] acfBytes = request.downloadHandler.data;

        var handle = GCHandle.Alloc(acfBytes, GCHandleType.Pinned);
        IntPtr ptr = handle.AddrOfPinnedObject();
        bool result = CriAtomEx.RegisterAcf(ptr, acfBytes.Length);
        handle.Free();

        if (!result)
        {
            Debug.LogError("ACF登録失敗（バイナリ）");
        }
        else
        {
            Debug.Log("ACF登録成功（WebGLバイナリ）");
            IsAcfLoaded = true;
        }
    }
    
    private class CriAtomCoroutineRunner : MonoBehaviour { }
}
