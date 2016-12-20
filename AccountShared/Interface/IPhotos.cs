using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VkData.Interface
{
    public interface IPhotos<TPhoto, in TPhotoSize> 
    {
        List<string> GetPathById(IEnumerable<TPhoto> photos, TPhotoSize size);
        ReadOnlyCollection<TPhoto> GetById(IEnumerable<string> ids);
        List<string> GetPathByAttachments(IEnumerable<object> attachments, TPhotoSize size);
    }
}