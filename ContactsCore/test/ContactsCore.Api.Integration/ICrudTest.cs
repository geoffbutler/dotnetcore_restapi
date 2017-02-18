using System.Threading.Tasks;

namespace ContactsCore.Api.Integration
{
    public interface ICrudTest
    {
        Task CreateAndReadByUid();
        Task CreateAndRead();
        Task CreateAndUpdate();
        Task CreateAndDelete();
    }
}
