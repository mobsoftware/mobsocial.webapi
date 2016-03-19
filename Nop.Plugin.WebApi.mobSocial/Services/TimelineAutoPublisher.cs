using System;
using Nop.Core;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Services
{
    public class TimelineAutoPublisher : ITimelineAutoPublisher
    {
        private readonly ITimelineService _timelineService;
        private readonly IWorkContext _workContext;
        public TimelineAutoPublisher(ITimelineService timelineService, IWorkContext workContext)
        {
            _timelineService = timelineService;
            _workContext = workContext;
        }

        public void Publish<T>(T entity, string postTypeName) where T : BaseEntity
        {
            //create new timeline post
            var post = new TimelinePost() {
                Message = string.Empty,
                AdditionalAttributeValue = string.Empty,
                PostTypeName = postTypeName,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                OwnerId = _workContext.CurrentCustomer.Id,
                IsSponsored = false,
                OwnerEntityType = TimelinePostOwnerTypeNames.Customer,
                LinkedToEntityName = typeof(T).Name,
                LinkedToEntityId = entity.Id,
                PublishDate = DateTime.UtcNow
            };
            //save the post
            _timelineService.Insert(post);
        }
    }
}
