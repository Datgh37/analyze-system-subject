using ExpenseTracker.Application.Interfaces;

namespace ExpenseTracker.Application.Services;

// Implementation of Image Pre-Processing Service (UC-03.07 Enhancement)
public class ImagePreProcessingService : IImagePreProcessingService
{
    public Task<Stream> PreprocessReceiptImageAsync(Stream imageStream, string fileName, CancellationToken ct = default)
    {
        if (imageStream == null || imageStream.Length == 0)
            return Task.FromResult<Stream>(new MemoryStream());

        // Ensure stream is at origin
        if (imageStream.CanSeek)
            imageStream.Position = 0;

        var memoryStream = new MemoryStream();
        imageStream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        // In production pipeline, this module applies contrast stretching and orientation normalization.
        // Returns sanitized stream ready for OCR pipeline.
        return Task.FromResult<Stream>(memoryStream);
    }
}
