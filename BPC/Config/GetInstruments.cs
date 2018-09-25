using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BPC.Config
{
    public class GetInstruments
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        ConfigHandler configHandler = new ConfigHandler();
        public Dictionary<string, Instrument> GiveInstruments()
        {
            logger.Trace("..in i GiveInstruments..");
            var Instruments = new Dictionary<string, Instrument>();

            XmlSerializer serializer = new XmlSerializer(typeof(List<Instrument>),new XmlRootAttribute("Instruments"));

            try
            {
                String FilePath = configHandler.GetInstrumentsPath();
                using (FileStream stream = File.OpenRead(FilePath))//svto
                {
                    List<Instrument> dezerializedList = (List<Instrument>)serializer.Deserialize(stream);
                    foreach (Instrument item in dezerializedList)
                    {
                        Instruments.Add(item.MontelSymbol, item);
                        logger.Trace("Instrument lagts i dictionary:" + item.MontelSymbol);
                    }
                }
            }
            catch (Exception ex )
            {
                logger.Error(ex.Message);
            }
            logger.Trace("...Giveinstruments är avslutad");
            return Instruments;
        }



    }
}
