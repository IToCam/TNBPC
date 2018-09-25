

using BPC.Authenticate;

namespace BPC
{
    public interface ITicketHolder
    {
        void SetTicket(MontelTicket montelTicket);
        MontelTicket GetTicket();
    }

    public class TicketHolder : ITicketHolder
    {
        private MontelTicket _montelTicket;
        public void SetTicket(MontelTicket montelTicket)
        {
            _montelTicket = montelTicket;
        }

        public MontelTicket GetTicket()
        {
            return _montelTicket;
        }
    }
}
