using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GenericODataWebApi
{
    public class IfODataMethodEnabledAttribute : ActionFilterAttribute
    {
        public ODataOperations[] Operations { get; set; }
        public IfODataMethodEnabledAttribute(params ODataOperations[] operations)
        {
            Operations = operations;
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if(Operations.All(o => o.IsEnabled()))
                return base.OnActionExecutingAsync(actionContext, cancellationToken);

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}