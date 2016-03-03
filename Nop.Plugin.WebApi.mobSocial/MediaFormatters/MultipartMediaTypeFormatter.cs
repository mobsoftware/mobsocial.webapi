using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Nop.Plugin.WebApi.MobSocial.MediaFormatters.Converters;

namespace Nop.Plugin.WebApi.MobSocial.MediaFormatters
{
    
    public class MultipartMediaTypeFormatter : MediaTypeFormatter
    {
        const string SupportedMediaType = "multipart/form-data";

        public MultipartMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(SupportedMediaType));
        }
        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);

            //need add boundary
            //(if add when fill SupportedMediaTypes collection in class constructor then receive post with another boundary will not work - Unsupported Media Type exception will thrown)
            if (headers.ContentType == null)
                headers.ContentType = new MediaTypeHeaderValue(SupportedMediaType);

            if (!String.Equals(headers.ContentType.MediaType, SupportedMediaType, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Not a Multipart Content");

            if (headers.ContentType.Parameters.All(m => m.Name != "boundary"))
                headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", "MultipartDataMediaFormatterBoundary1q2w3e"));
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
                                                               IFormatterLogger formatterLogger)
        {
            var httpContentToFormDataConverter = new HttpContentToFormDataConverter();
            var multipartFormData = await httpContentToFormDataConverter.Convert(content);

            var dataToObjectConverter = new FormDataToObjectConverter(multipartFormData);
            var result = dataToObjectConverter.Convert(type);


            return result;
        }

    }
}
