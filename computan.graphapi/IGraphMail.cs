using computan.graphapi.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace computan.graphapi
{
    public interface IGraphMail
    {
        Task SendAsync(GraphMailModel mailModel, List<HttpPostedFileBase> file);
    }
}