namespace Masa.Mc.Infrastructure.Weixin.Work;

public interface IWeixinWorkSender
{
    Task<WeixinWorkMessageResponseBase> SendTextAsync(WeixinWorkTextMessage message);

    Task<WeixinWorkMessageResponseBase> SendTextCardAsync(WeixinWorkTextCardMessage message);
}