private IEnumerator Start()
{
    string acfPath = Application.streamingAssetsPath + "/Example.acf";
    string bgmAcbPath = Application.streamingAssetsPath + "/BGM.acb";
    string seAcbPath = Application.streamingAssetsPath + "/SE.acb";

    if (Application.platform == RuntimePlatform.WebGLPlayer)
    {
        CriAtomWebGL.RegisterAcf(acfPath);
        yield return new WaitUntil(() => CriAtomWebGL.IsAcfLoaded);

        CriAtomWebGL.RegisterAcb(bgmAcbPath);
        yield return new WaitUntil(() => CriAtomWebGL.IsAcbLoaded);
        
        CriAtomWebGL.RegisterAcb(seAcbPath);
        yield return new WaitUntil(() => CriAtomWebGL.IsAcbLoaded);
        
        bgmAcb = CriAtomWebGL.GetLoadedAcb("BGM");
        seAcb = CriAtomWebGL.GetLoadedAcb("SE");
    }
}
