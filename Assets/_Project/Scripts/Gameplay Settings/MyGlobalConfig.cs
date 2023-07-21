using Sirenix.Utilities;

[GlobalConfig("Assets/Resources/MyConfigFiles/")]
public class MyGlobalConfig : GlobalConfig<MyGlobalConfig>
{
    public int MyGlobalVariable;
}