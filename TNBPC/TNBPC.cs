using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BPC.Config;
using BPC.Authenticate;
using BPC;
using BPC.Http;
using BPC.SignalR;

namespace TNBPC
{
    public partial class TNBPC : ServiceBase
    {
       BPC_Handler bPC_Handler = new BPC_Handler();
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public TNBPC()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                logger.Trace("TNBPC as started!");
                logger.Trace("GetTicked is called");
                bPC_Handler.GetTicket();
                logger.Trace("Subscribed is called");
                bPC_Handler.Subscribe();
            }
            catch (Exception ex)
            {
                
                logger.Error("OnStart error! " + ex.Message);
            }

        }

        protected override void OnStop()
        {
            logger.Trace("Unsubscribed called");
            bPC_Handler.UnSubscribe();
            logger.Trace("TNBPC as stopped!");
        }
    }
}
