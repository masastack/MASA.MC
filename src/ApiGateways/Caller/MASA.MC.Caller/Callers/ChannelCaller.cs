using MASA.MC.Contracts.Admin.Dtos.Channels;

namespace MASA.MC.Caller.Callers
{
    public class ChannelCaller : HttpClientCallerBase
    {
        protected override string BaseAddress { get; set; } = "http://localhost:6067";

        public ChannelCaller(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = nameof(ChannelCaller);
        }

        public async Task<List<ChannelDto>> GetListAsync()
        {
            return await CallerProvider.GetAsync<List<ChannelDto>>($"channel/list");
        }
    }
}