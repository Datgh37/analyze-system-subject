namespace ExpenseTracker.Application.Interfaces;

// Service for receipt image pre-processing (contrast enhancement, orientation check) before LLM OCR
public interface IImagePreProcessingService
{
    // Validates and prepares receipt image stream for OCR pipeline
    Task<Stream> PreprocessReceiptImageAsync(
        Stream imageStream,
        string fileName,
        CancellationToken ct = default);
}
