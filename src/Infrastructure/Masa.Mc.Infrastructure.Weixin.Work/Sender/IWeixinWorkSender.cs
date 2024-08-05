namespace Masa.Mc.Infrastructure.Weixin.Work.Sender;

public interface IWeixinWorkSender
{
    Task<WeixinWorkMessageResponse> SendTextAsync(WeixinWorkTextMessage message);

    Task<WeixinWorkMessageResponse> SendTextCardAsync(WeixinWorkTextCardMessage message);
}