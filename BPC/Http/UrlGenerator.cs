using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web;

namespace BPC.Http
{
    public interface IUrlGenerator
    {

        string GetQuoteRequestUrl(List<string> symbolKeys, List<string> fields);
        string SubscribeQuoteRequestUrl(Guid subscriptionId, List<string> symbolKeys, List<string> fields);
        string ChangeQuoteSubscriptionUrl(Guid subscriptionId, List<string> symbolKeys, List<string> fields);
        string StopQuoteSubscriptionUrl(Guid subscriptionId);

        string GetOhlcRequestUrl(string symbolKey, List<string> fields, DateTime fromDate, DateTime? toDate, string sortType, string missingValueModel, bool continuous);
        string SubscribeOhlcRequestUrl(Guid subscriptionGuid, string symbolKey, List<string> fields);
        string ChangeOhlcRequestUrl(Guid subscriptionGuid, string symbolKey, List<string> fields);
        string StopOhlcSubscriptionUrl(Guid subscriptionId);

        string GetSpotPriceRequestUrl(string spotKey, List<string> fields, DateTime fromDate, DateTime? toDate, string currency, string sortType);
        string SubscribeSpotPricesUrl(Guid subscriptionGuid, string spotKey, List<string> fields, string denomination);
        string ChangeSpotPricesSubscriptionUrl(Guid subscriptionGuid, string spotKey, List<string> fields, string currency);
        string GetSpotVolumeRequestUrl(string spotKey, List<string> fields, DateTime fromDate, DateTime? toDate, string sortType);
        string SubscribeSpotVolumesUrl(Guid subscriptionGuid, string spotKey, List<string> fields);
        string ChangeSpotVolumeSubscriptionUrl(Guid subscriptionGuid, string spotKey, List<string> fields);
        string StopSpotSubscriptionUrl(Guid subscriptionId);

        string GetTradeRequestUrl(string symbolKey, DateTime fromTime, DateTime? toTime, string sortType);
        string GetTradeByCountRequestUrl(string symbolKey, int numberOfTrades, string sortTypeGet);
        string SubscribeTradeRequestUrl(Guid subscriptionGuid, string symbolKey);
        string ChangeTradeRequestUrl(Guid subscriptionGuid, string symbolKey);
        string StopTradeSubscriptionUrl(Guid subscriptionId);

        string GetOrderRequestUrl(string symbolKey);

        string GetFundamentalRequestUrl(string fundamentalKey,  DateTime fromDate, DateTime? toDate, string sortType);
        string SubscribeFundamental(Guid subscriptionId, string fundamentalKey);
        string ChangeFundamentalSubscriptionUrl(Guid subscriptionId, string fundamentalKey);
        string StopFundamentalSubscriptionUrl(Guid subscriptionId);

        string GetValidateTokenUrl();
        string GetActiveDerivativesMetadataUrl();
        string GetExpiredDerivativesMetadataUrl();
        string GetSpecifiedDerivativesMetadataUrl(List<string> symbolKeys);
        string GetSpotMetadataUrl();

        string GetFundamentalMetadataUrl();
    }

    public class UrlGenerator : IUrlGenerator
    {
        private readonly IProxyHandler _proxyHandler;
        private readonly string _httpCompress;


        public string GetQuoteRequestUrl(List<string> symbolKeys, List<string> fields)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/quote/get";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (string symbol in symbolKeys)
            {
                query.Add("symbolKeys", symbol);
            }
            
            foreach (string field in fields)
            {
                query.Add("fields", field);
            }
            
            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string SubscribeQuoteRequestUrl(Guid subscriptionGuid, List<string> symbolKeys, List<string> fields)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/quote/subscribe";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();

            foreach (string symbol in symbolKeys)
            {
                query.Add("symbolKeys", symbol);
            }
            
            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string ChangeQuoteSubscriptionUrl(Guid subscriptionGuid, List<string> symbolKeys, List<string> fields)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/quote/changesubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();

            foreach (string symbol in symbolKeys)
            {
                query.Add("symbolKeys", symbol);
            }

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            builder.Query = query.ToString();
            return builder.ToString();
        }
        
        public string StopQuoteSubscriptionUrl(Guid subscriptionGuid)
        {

            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/quote/stopsubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            builder.Query = query.ToString();

            return builder.ToString(); 
        }
        


        public string GetOhlcRequestUrl(string symbolKey, List<string> fields, DateTime fromDate, DateTime? toDate, string sortType, string missingValueModel, bool continuous)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/ohlc/get";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["symbolKey"] = symbolKey;

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            query["fromDate"] = fromDate.ToString("yyyy-MM-dd");
            query["toDate"] = toDate?.ToString("yyyy-MM-dd");
            query["sortType"] = sortType;
            query["insertElementsWhenDataMissing"] = missingValueModel;
            query["continuous"] = continuous.ToString();

            builder.Query = query.ToString();

            return builder.ToString();
        }
        
        public string SubscribeOhlcRequestUrl(Guid subscriptionGuid, string symbolKey, List<string> fields)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/ohlc/subscribe";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            query["symbolKey"] = symbolKey;

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string ChangeOhlcRequestUrl(Guid subscriptionGuid, string symbolKey, List<string> fields)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/ohlc/changesubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            query["symbolKey"] = symbolKey;

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            builder.Query = query.ToString();

            return builder.ToString();
        }
        
        public string StopOhlcSubscriptionUrl(Guid subscriptionGuid)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/ohlc/stopsubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            builder.Query = query.ToString();

            return builder.ToString();
        }




        public string GetSpotPriceRequestUrl(string spotKey, List<string> fields, DateTime fromDate, DateTime? toDate, string currency, string sortType)
        {
            
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/spot/getprices";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["spotKey"] = spotKey;

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            query["fromDate"] = fromDate.ToString("yyyy-MM-dd");
            query["toDate"] = toDate?.ToString("yyyy-MM-dd");
            query["currency"] = currency;

            query["sortType"] = sortType;
            
            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string SubscribeSpotPricesUrl(Guid subscriptionGuid, string spotKey, List<string> fields, string currency)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/spot/subscribeprices";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            query["spotKey"] = spotKey;

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            query["currency"] = currency;

            builder.Query = query.ToString();

            return builder.ToString();

        }

        public string ChangeSpotPricesSubscriptionUrl(Guid subscriptionGuid, string spotKey, List<string> fields, string currency)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/spot/changepricesubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            query["spotKey"] = spotKey;

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            query["currency"] = currency;

            builder.Query = query.ToString();

            return builder.ToString();

        }
        
        public string GetSpotVolumeRequestUrl(string spotKey, List<string> fields, DateTime fromDate, DateTime? toDate, string sortType)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/spot/getvolumes";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["spotKey"] = spotKey;

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            query["fromDate"] = fromDate.ToString("yyyy-MM-dd");
            query["toDate"] = toDate?.ToString("yyyy-MM-dd");

            query["sortType"] = sortType;
            
            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string SubscribeSpotVolumesUrl(Guid subscriptionGuid, string spotKey, List<string> fields)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/spot/subscribevolumes";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            query["spotKey"] = spotKey;

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }


            builder.Query = query.ToString();

            return builder.ToString();

        }

       public string ChangeSpotVolumeSubscriptionUrl(Guid subscriptionGuid, string spotKey, List<string> fields)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/spot/changevolumesubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            query["spotKey"] = spotKey;

            foreach (string field in fields)
            {
                query.Add("fields", field);
            }

            builder.Query = query.ToString();

            return builder.ToString();

        }

        public string StopSpotSubscriptionUrl(Guid subscriptionGuid)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/spot/stopsubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            builder.Query = query.ToString();

            return builder.ToString();

        }

        public string GetTradeRequestUrl(string symbolKey, DateTime fromTime, DateTime? toTime, string sortType)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/trade/get";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["symbolKey"] = symbolKey;

            query["fromTime"] = fromTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            query["toTime"] = toTime?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            query["sortType"] = sortType;
            
            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string GetTradeByCountRequestUrl(string symbolKey, int numberOfTrades, string sortType)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/trade/getbycount";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["symbolKey"] = symbolKey;
            query["numberOfTrades"] = numberOfTrades.ToString();
            query["sortType"] = sortType;

            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string SubscribeTradeRequestUrl(Guid subscriptionGuid, string symbolKey)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/trade/subscribe";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            query["symbolKey"] = symbolKey;
            
            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string ChangeTradeRequestUrl(Guid subscriptionGuid, string symbolKey)
        {
            
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/trade/changesubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            query["symbolKey"] = symbolKey;
            
           
            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string StopTradeSubscriptionUrl(Guid subscriptionGuid)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/trade/stopsubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionGuid.ToString();
            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string GetOrderRequestUrl(string symbolKey)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/order/get";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["symbolKey"] = symbolKey;

            builder.Query = query.ToString();

            return builder.ToString();
        }


        public string GetFundamentalRequestUrl(string fundamentalKey, DateTime fromDate, DateTime? toDate, string sortType)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/fundamental/get";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["fundamentalKey"] = fundamentalKey;

          
            query["fromDate"] = fromDate.ToString("yyyy-MM-dd");
            query["toDate"] = toDate?.ToString("yyyy-MM-dd");


            query["sortType"] = sortType;

            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string SubscribeFundamental(Guid subscriptionId, string fundamentalKey)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/fundamental/subscribe";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionId.ToString();
            query["fundamentalkey"] = fundamentalKey;

           
            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string ChangeFundamentalSubscriptionUrl(Guid subscriptionId, string fundamentalKey)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/fundamental/changesubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionId.ToString();
            query["fundamentalkey"] = fundamentalKey;

            
            builder.Query = query.ToString();

            return builder.ToString();
        }

       
        public string StopFundamentalSubscriptionUrl(Guid subscriptionId)
        {
            string apiPath = ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/fundamental/stopsubscription";

            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["subscriptionId"] = subscriptionId.ToString();
            builder.Query = query.ToString();

            return builder.ToString();
        }





        public string GetValidateTokenUrl()
        {
            return ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/validatetoken/get";
        }

        public string GetActiveDerivativesMetadataUrl()
        {
            return ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/getmetadataforactivecontracts";
        }

        public string GetExpiredDerivativesMetadataUrl()
        {
            return ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/getmetadataforexpiredcontracts";
        }

        public string GetSpecifiedDerivativesMetadataUrl(List<string> symbolKeys)
        {
            string apiPath=  ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/derivatives/getmetadataforspecifiedcontracts";
            var builder = new UriBuilder(apiPath);
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (string symbol in symbolKeys)
            {
                query.Add("symbolKeys", symbol);
            }

            builder.Query = query.ToString();

            return builder.ToString();
        }

        public string GetSpotMetadataUrl()
        {
            return ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/spot/getmetadata";
        }

        public string GetFundamentalMetadataUrl()
        {
            return ConfigurationManager.AppSettings["MontelWebApiUri"].TrimEnd('/') + "/fundamental/getmetadata";
        }
    }
}