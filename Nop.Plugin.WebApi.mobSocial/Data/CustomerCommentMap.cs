using Mob.Core;
using Mob.Core.Data;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace Nop.Plugin.WebApi.MobSocial.Data
{
    public class CustomerCommentMap : BaseMobEntityTypeConfiguration<CustomerComment>
    {
        public CustomerCommentMap()
        {
            Property(x => x.CustomerId);
            Property(x => x.EntityId);
            Property(x => x.EntityName);
            Property(x => x.CommentText);
            Property(x => x.AdditionalData);
        } 
    }
}