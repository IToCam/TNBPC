using System;

namespace BPC
{
    public interface IConnectionHandler
    {
        event EventHandler Disconeccted;
        event EventHandler Connected;
        void FireDisconnectedEvent();
        void FireConnectedEvent();
    }

    public class ConnectionHandler : IConnectionHandler
    {
        public event EventHandler Disconeccted;
        public event EventHandler Connected;

        public void FireDisconnectedEvent()
        {
            Disconeccted?.Invoke(this, new EventArgs());
        }

        public void FireConnectedEvent()
        {
            Connected?.Invoke(this, new EventArgs());
        }
    }
}
