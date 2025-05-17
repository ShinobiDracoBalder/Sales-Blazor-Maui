namespace SBM.Shared.Responses
{
    public class Response<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Código de estado HTTP (por ejemplo, 200, 400, 404). int statusCode = (int)response.StatusCode;
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Mensaje adicional que describe el resultado.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Resultado genérico de la operación.
        /// </summary>
        public T? Result { get; set; }
    }
}
