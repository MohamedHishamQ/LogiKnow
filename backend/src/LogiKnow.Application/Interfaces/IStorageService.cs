using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LogiKnow.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType, string directory = "uploads", CancellationToken ct = default);
}
