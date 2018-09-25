using System.Configuration;

namespace BPC.Config
{
    public class DatabaseSettings : ConfigurationSection
    {
        [ConfigurationProperty("ConnectionStringParts")]
        public DbFeatures DbFeatures
        {
            get
            {
                return (DbFeatures)this["ConnectionStringParts"];
            }
            set
            {
                value = (DbFeatures)this["ConnectionStringParts"];
            }
        }
    }

}







