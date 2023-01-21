using MassTransit;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebDataProvider.Db;
using WebModelDto.Request;
using WebModelDto.Response;
using WebModelDto.Response.Info;
using WebWorkersData;
using WebWorkersMapper;

namespace Lanit_HW5_Server.Consumer
{
    public class GetWorkersConsumer : IConsumer<GetWorkersRequest>
    {
        private readonly IWorkerRepository _repository;
        private readonly IGetWorkerMapper _mapper;
        public GetWorkersConsumer(IWorkerRepository repository, IGetWorkerMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<GetWorkersRequest> context)
        {
            GetWorkersResponse response = new();
            List<WorkerInfo> workers = new();
            foreach (DbWorker worker in await _repository.GetAllAsync(context.Message.Branch, context.Message.Achievements))
            {
                workers.Add(_mapper.Map(worker));
            }
            response.IsSuccess = true;
            response.WorkerInfosList = workers;
            response.Errors = null;

            await context.RespondAsync(response);
        }
    }
}

            