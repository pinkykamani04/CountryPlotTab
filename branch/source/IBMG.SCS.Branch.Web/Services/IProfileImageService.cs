namespace IBMG.SCS.Branch.Web.Services
{
    public interface IProfileImageService
    {
        Task<string> UploadAsync(
             byte[] imageBytes,
             string fileName,
             string? existingImageUrl,
             CancellationToken cancellationToken);

    }
}
