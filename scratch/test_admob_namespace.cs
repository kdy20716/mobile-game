#if UNITY_EDITOR
try
{
    var type = System.Type.GetType("GoogleMobileAds.Api.MobileAds, GoogleMobileAds");
    if (type != null) return "GoogleMobileAds Found: " + type.FullName;
    
    // Check loaded assemblies
    foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
    {
        if (asm.GetName().Name.Contains("GoogleMobileAds"))
        {
            return "Found Assembly: " + asm.GetName().Name;
        }
    }
    return "GoogleMobileAds assembly not loaded yet";
}
catch (System.Exception ex)
{
    return "Error: " + ex.Message;
}
#endif
return "Not Editor";
