namespace HospitalManagement.Interfaces
{
    public interface IActivityLogger
    {
        Task Log(string activity, string userId);
    }
}
