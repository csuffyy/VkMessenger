using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace VkData.Interface
{
    public interface IPhotos<TPhoto, in TPhotoSize>
    {
        Task<List<string>> GetPathById(IEnumerable<TPhoto> photos, TPhotoSize size);
        ReadOnlyCollection<TPhoto> GetById(IEnumerable<string> ids);
        Task<List<string>> GetPathByAttachments(IEnumerable<object> attachments, TPhotoSize size);
    }
}