using Nop.Core;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public interface ITimelineAutoPublisher
    {
        void Publish<T>(T entity, string postTypeName) where T: BaseEntity;
    }
}