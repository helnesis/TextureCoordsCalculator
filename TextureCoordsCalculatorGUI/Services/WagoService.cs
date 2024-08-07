using System.IO;
using System.Net.Http;

namespace TextureCoordsCalculatorGUI.Services
{
    public sealed class WagoService : IWagoService
    {
        private readonly string _baseUrl = "https://wago.tools/api";
        private readonly HttpClient _client = new();

        /// <summary>
        /// Gets a file from the Wago API, then store it in memory.
        /// </summary>
        /// <param name="fdid">FDID</param>
        /// <returns>File as stream</returns>
        public async Task<Stream> GetCascFile(uint fdid)
        {
            var fullUri = string.Concat(_baseUrl, $"/casc/{fdid}");

            var stream = await _client.GetByteArrayAsync(fullUri);

            return new MemoryStream(stream);
        }
    }
}
