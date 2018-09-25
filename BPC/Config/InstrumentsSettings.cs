using System.Configuration;

namespace BPC.Config
{
    public class InstrumentsSettings : ConfigurationSection
    {
        [ConfigurationProperty("ConnectionStringParts")]
        public InstrumentsPath InstrumentsPath
        {
            get
            {
                return (InstrumentsPath)this["ConnectionStringParts"];
            }
            set
            {
                value = (InstrumentsPath)this["ConnectionStringParts"];
            }
        }
    }
}
