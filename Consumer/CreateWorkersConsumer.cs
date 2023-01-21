using MassTransit;
using System.Threading.Tasks;
using WebWorkersMapper;
using WebWorkersData;
using WebDataProvider;
using WebDataProvider.Db;
using System.Collections.Generic;
using System.Linq;
using WebModelDto.Request;
using WebModelDto.Response;

namespace Lanit_HW5_Server.Consumer
{
    public class CreateWorkersConsumer : IConsumer<CreateWorkersRequest>
    {
        private IWorkerRepository _repository;
        private IDbWorkersMapper _mapper;
        public CreateWorkersConsumer(IWorkerRepository repository, IDbWorkersMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<CreateWorkersRequest> context)
        {
            bool chechBranch = false;

            using (WorkerDbContext BranchContext = new WorkerDbContext())
            {
                DbWorker workers = BranchContext.Workers.FirstOrDefault(b => b.BranchId == context.Message.BranchId);

                if (workers != null)
                {
                    chechBranch = true;
                }
            }
            
            CreateWorkersResponse  response = new();

            if (chechBranch)
            {
                response.Id = _repository.Create(_mapper.Map(context.Message));
                response.IsSuccess = true;
                
            }
            else
            {
                response.IsSuccess = false;
                response.Errors = new List<string> { "branch does`t exist" };
            }

            await context.RespondAsync(response);
        }
    }
}
