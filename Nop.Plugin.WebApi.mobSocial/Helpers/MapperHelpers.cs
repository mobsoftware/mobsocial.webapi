using AutoMapper;

namespace Nop.Plugin.WebApi.MobSocial.Helpers
{
    public static class MapperHelpers
    {
        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return (TDestination)Mapper.Map(source, destination, typeof(TSource), typeof(TDestination));
        }
    }
}
