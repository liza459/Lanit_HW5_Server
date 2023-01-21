using MassTransit;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebDataProvider.Db;
using WebDataProvider;
using WebModelDto.Response;
using WebWorkersData;
using WebWorkersMapper;
using WebModelDto.Request;
using System.Linq;

namespace Lanit_HW5_Server.Consumer
{
    public class UpdateWorkersConsumer :IConsumer<UpdateWorkersRequest>
    {
        private IWorkerRepository _repository;
        public UpdateWorkersConsumer(IWorkerRepository repository)
        {
            _repository = repository;
        }
        public async Task Consume(ConsumeContext<UpdateWorkersRequest> context)
        {

            UpdateWorkersResponse response = new();

            if (await _repository.DoesSuchABranchExist(context.Message.BranchId) && await _repository.DoesSuchAWorkerIdExist(context.Message.Id))
            {
                var worker = await _repository.UpdateAsync(context.Message);
                response.Id = worker.Id;
                response.Name = worker.Name;
                response.LastName = worker.LastName;
                response.BranchId = worker.BranchId;
                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
                response.Errors = new List<string> { "branch or id does`t exist" };
            }

            await context.RespondAsync(response);
        }
    }
}
