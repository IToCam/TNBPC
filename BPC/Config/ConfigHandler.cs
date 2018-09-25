using System.Configuration;

namespace BPC.Config
{
    public class ConfigHandler
    {
        public string GetDataSource()
        {
            DatabaseSettings databaseSettings = ConfigurationManager.GetSection("DatabaseSettings") as DatabaseSettings;
            return databaseSettings.DbFeatures.DataSource;
        }

        public string GetDataInitialCatalog()
        {
            DatabaseSettings databaseSettings = ConfigurationManager.GetSection("DatabaseSettings") as DatabaseSettings;
            return databaseSettings.DbFeatures.InitialCatalog;
        }

        public string GetDataUserId()
        {
            DatabaseSettings databaseSettings = ConfigurationManager.GetSection("DatabaseSettings") as DatabaseSettings;
            return databaseSettings.DbFeatures.UserID;
        }

        public string GetDataPassword()
        {
            DatabaseSettings databaseSettings = ConfigurationManager.GetSection("DatabaseSettings") as DatabaseSettings;
            return databaseSettings.DbFeatures.Password;
        }

        public string GetMontelClientId()
        {
            MontelApiSettings montelApiSettings = ConfigurationManager.GetSection("MontelApiSettings") as MontelApiSettings;
            return montelApiSettings.ApiFeatures.Clientid;
        }

        public string GetMontelUsername()
        {

            MontelApiSettings montelApiSettings = ConfigurationManager.GetSection("MontelApiSettings") as MontelApiSettings;
            return montelApiSettings.ApiFeatures.Username;
        }

        public string GetMontelPassword()
        {

            MontelApiSettings montelApiSettings = ConfigurationManager.GetSection("MontelApiSettings") as MontelApiSettings;
            return montelApiSettings.ApiFeatures.Password;
        }

        public string GetInstrumentsPath()
        {
            InstrumentsSettings instrumentsSettings = ConfigurationManager.GetSection("InstrumentsSettings") as InstrumentsSettings;
            return instrumentsSettings.InstrumentsPath.Path ;
        }
    }


public class InstrumentsPath: ConfigurationElement
    {
        [ConfigurationProperty("Path", IsRequired = true)]
        public string Path
        {
            get
            {
                return (string)this["Path"];
            }
            set
            {
                value = (string)this["Path"];
            }
        }

    }

public class DbFeatures : ConfigurationElement
    {
        [ConfigurationProperty("DataSource", IsRequired = true)]
        public string DataSource
        {
            get
            {
                return (string)this["DataSource"];
            }
            set
            {
                value = (string)this["DataSource"];
            }
        }

        [ConfigurationProperty("InitialCatalog", IsRequired = true)]
        public string InitialCatalog
        {
            get
            {
                return (string)this["InitialCatalog"];
            }
            set
            {
                value = (string)this["InitialCatalog"];
            }
        }

        [ConfigurationProperty("UserID", IsRequired = true)]
        public string UserID
        {
            get
            {
                return (string)this["UserID"];
            }
            set
            {
                value = (string)this["UserID"];
            }
        }
        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }
            set
            {
                value = (string)this["Password"];
            }
        }
    }

public class ApiFeatures : ConfigurationElement
{
    [ConfigurationProperty("Clientid", IsRequired = true)]
    public string Clientid
    {
        get
        {
            return (string)this["Clientid"];
        }
        set
        {
            value = (string)this["Clientid"];
        }
    }

    [ConfigurationProperty("Username", IsRequired = true)]
    public string Username
    {
        get
        {
            return (string)this["Username"];
        }
        set
        {
            value = (string)this["Username"];
        }
    }

    [ConfigurationProperty("Password", IsRequired = true)]
    public string Password
    {
        get
        {
            return (string)this["Password"];
        }
        set
        {
            value = (string)this["Password"];
        }
    }
    
}



}







