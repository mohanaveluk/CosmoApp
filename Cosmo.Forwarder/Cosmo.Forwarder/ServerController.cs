using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Cosmo.Forwarder
{
    public class ServerController: ApiController
    {
        readonly IServerRepository _repository;

        public ServerController() : this(new ServerRepository())
        {

        }

        public ServerController(IServerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AcceptVerbs("Get")]
        public PerformanceResponse GetServerDetail(string srn)
        {
            return _repository.GeterverPerformance(srn);
        }
    }
}
