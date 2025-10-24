using Optiviera.Data;
using Optiviera.Models;

namespace Optiviera.Services.Interfaces
{
    public interface ITTicketService
    {
        public Task<List<Ticket>> GetAllTicketsAsync();

        public Task<Ticket> GetTicketByIdAsync(int ticketId);

        public Task AddTicketCommentAsync(Comment comment);

        public Task<List<Ticket>> GetAllTicketsByPriorityAsync(string priorityName);

        public Task<int?> LookupTicketPriorityIdAsync(string priorityName);
    }
}
