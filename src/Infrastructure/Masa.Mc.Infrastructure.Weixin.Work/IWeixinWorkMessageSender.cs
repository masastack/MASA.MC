namespace Masa.Mc.Infrastructure.Weixin.Work;

public interface IWeixinWorkMessageSender
{
    Task<WeixinWorkMessageResponseBase> SendTextAsync(WeixinWorkTextMessage message);

    Task<WeixinWorkMessageResponseBase> SendTextCardAsync(WeixinWorkTextCardMessage message);
}