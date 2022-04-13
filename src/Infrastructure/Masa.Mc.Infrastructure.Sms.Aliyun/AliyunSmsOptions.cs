namespace Masa.Mc.Infrastructure.Sms.Aliyun
{
    public class AliyunSmsOptions
    {
        public string AccessKeySecret { get; set; }

        public string AccessKeyId { get; set; }

        public string EndPoint { get; set; }

        public void Initialize(AliyunSmsOptions options)
        {
            AccessKeySecret = options.AccessKeySecret;
            AccessKeyId = options.AccessKeyId;
            EndPoint = options.EndPoint;
        }
    }
}