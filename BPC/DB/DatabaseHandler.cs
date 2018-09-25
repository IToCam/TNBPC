using System;
using System.Data.SqlClient;
using BPC.Config;

namespace BPC.DB
{ 
    public class DatabaseHandler
    {

        //inserts a trade in DB TNDB_MarketData table Trade
        public void InsertTrade(    
                                     int InstrumentID, byte? VendorID
                                    ,DateTime TradeTime, decimal Price, DateTime DataBaseSaveTime
                                )
        {

            ConfigHandler configHandler = new ConfigHandler();

            SqlConnection conn = new SqlConnection(
                new SqlConnectionStringBuilder()
                {
                    DataSource = configHandler.GetDataSource(),
                    InitialCatalog = configHandler.GetDataInitialCatalog(),
                    UserID = configHandler.GetDataUserId(),
                    Password = configHandler.GetDataPassword()
                }.ConnectionString
            );


            // create command object with SQL query and link to connection object
            SqlCommand Cmd = new SqlCommand("if not exists(select tradetime from trade where instrumentid = @InstrumentID and vendorid = @VendorID and dbo.fnDateOnly(TradeTime) = dbo.fnDateOnly(@TradeTime)) " +
                                                "INSERT INTO dbo.Trade " +
                                             "(InstrumentID, VendorID, TradeTime, Price, Volume,  DataBaseSaveTime)"
                                             +
                                                "VALUES( @InstrumentID, @VendorID, @TradeTime, @Price, @Volume, @DataBaseSaveTime)"

                                                , conn);

            // create your parameters
            Cmd.Parameters.Add("@InstrumentID", System.Data.SqlDbType.Int);
            Cmd.Parameters.Add("@VendorID", System.Data.SqlDbType.TinyInt);
            Cmd.Parameters.Add("@TradeTime", System.Data.SqlDbType.DateTime);
            Cmd.Parameters.Add("@Price", System.Data.SqlDbType.Decimal);
            Cmd.Parameters.Add("@Volume", System.Data.SqlDbType.Decimal);
            Cmd.Parameters.Add("@DataBaseSaveTime", System.Data.SqlDbType.DateTime);

            decimal volume = 1;

            Cmd.Parameters["@InstrumentID"].Value = InstrumentID;
            Cmd.Parameters["@VendorID"].Value = VendorID;
            Cmd.Parameters["@TradeTime"].Value = TradeTime;
            Cmd.Parameters["@Price"].Value = Price;
            Cmd.Parameters["@Volume"].Value = volume;
            Cmd.Parameters["@DataBaseSaveTime"].Value = DataBaseSaveTime;
            
            // open sql connection
            conn.Open();

            // execute the query and return number of rows affected, should be one
            int RowsAffected = Cmd.ExecuteNonQuery();

            // close connection when done
            conn.Close();
        }
    }


    
}
