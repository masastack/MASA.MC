using Microsoft.Extensions.Options;

namespace Masa.Mc.Service.Admin.Infrastructure.Sms
{
    public class ChannelAliyunSmsSender: AliyunSmsSender
    {
        private AliyunConfig Config;
        public ChannelAliyunSmsSender(IOptionsSnapshot<AliyunSmsOptions> options):base(options)
        {

        }

        protected override AliyunClient CreateClient()
        {
            return new(new AliyunConfig
            {
                AccessKeyId = Options.AccessKeyId,
                AccessKeySecret = Options.AccessKeySecret,
                Endpoint = Options.EndPoint
            });
        }
    }
}
