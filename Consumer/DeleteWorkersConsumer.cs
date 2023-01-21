using MassTransit;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebModelDto.Request;
using WebModelDto.Response;
using WebWorkersData;
using WebWorkersMapper;

namespace Lanit_HW5_Server.Consumer
{
    public class DeleteWorkersConsumer : IConsumer<DeleteWorkersRequest>
    {
        private IWorkerRepository _repository;
        public DeleteWorkersConsumer(IWorkerRepository repository)
        {
            _repository = repository;
        }
        public async Task Consume(ConsumeContext<DeleteWorkersRequest> context)
        {
           DeleteWorkersResponse response = new();

            if (await _repository.DoesSuchAWorkerIdExist(context.Message.Id))
            {
                bool worker = false;
                worker = await _repository.DeleteAsync(context.Message);
                if (worker)
                {
                    response.IsSuccess = true;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Errors = new List<string> { "id does`t exist" };
            }

            await context.RespondAsync(response);
        }
    }
}
