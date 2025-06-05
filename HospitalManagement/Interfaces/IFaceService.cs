
namespace HospitalManagement.Interfaces
{
    public interface IFaceService
    {
        Task CreatePersonGroupIfNotExistsAsync();
        Task<string> RegisterFaceAsync(string userName, Stream imageStream);
        Task<string?> IdentifyFaceAsync(Stream imageStream);
    }
}
