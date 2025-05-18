using SBM.Shared.Responses;
using System.Net;

namespace SBM.WEB.Repositories
{
    public class HttpResponseWrapper<T>
    {
        public HttpResponseWrapper(Response<T>? response, bool error, HttpResponseMessage httpResponseMessage)
        {
            Error = error;
            Response = response;
            HttpResponseMessage = httpResponseMessage;
        }

        public bool Error { get; set; }
        public Response<T>? Response { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }

        public async Task<string?> GetErrorMessageAsync()
        {
            if (!Error) return null;

            return HttpResponseMessage.StatusCode switch
            {
                HttpStatusCode.NotFound => "Recurso no encontrado",
                HttpStatusCode.BadRequest => await HttpResponseMessage.Content.ReadAsStringAsync(),
                HttpStatusCode.Unauthorized => "Tienes que logearte para hacer esta operación",
                HttpStatusCode.Forbidden => "No tienes permisos para hacer esta operación",
                _ => "Ha ocurrido un error inesperado"
            };
        }
    }
}
