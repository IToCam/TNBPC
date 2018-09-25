using System.Configuration;

namespace BPC.Config
{
    public class MontelApiSettings : ConfigurationSection
{
    [ConfigurationProperty("ConnectionStringParts")]
    public ApiFeatures ApiFeatures
    {
        get
        {
            return (ApiFeatures)this["ConnectionStringParts"];
        }
        set
        {
            value = (ApiFeatures)this["ConnectionStringParts"];
        }
    }
}

}







