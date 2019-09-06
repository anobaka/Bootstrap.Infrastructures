using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Bootstrap.Infrastructures.Extensions
{
    /// <summary>
    /// 每一种【安全验证等级（如短息验证、密码验证）】可对应一系列Token，Token可能用于：修改密码，支付等目标；可根据需求更换数据源
    /// </summary>
    public abstract class SessionTicketManager<TTicketPurpose> where TTicketPurpose : Enum
    {
        private class Ticket
        {
            public TTicketPurpose Purpose { get; set; }
            public DateTimeOffset? ExpireDt { get; set; }
        }

        private readonly IHttpContextAccessor _accessor;
        private const string KeyPrefix = "Ticket-";

        protected HttpContext HttpContext => _accessor.HttpContext;

        protected SessionTicketManager(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public Task AddToken(TTicketPurpose purpose, DateTimeOffset? expireDt)
        {
            HttpContext.Session.Set($"{KeyPrefix}{purpose}", new Ticket {Purpose = purpose, ExpireDt = expireDt});
            return Task.CompletedTask;
        }

        public Task DisableToken(TTicketPurpose purpose)
        {
            HttpContext.Session.Remove($"{KeyPrefix}{purpose}");
            return Task.CompletedTask;
        }

        public async Task<bool> ValidateToken(TTicketPurpose purpose, bool consume = true)
        {
            var token = HttpContext.Session.Get<Ticket>($"{KeyPrefix}{purpose}");
            var success = false;
            if (token != null)
            {
                if (token.ExpireDt.HasValue || token.ExpireDt > DateTimeOffset.Now)
                {
                    success = true;
                }

                if (consume)
                {
                    await DisableToken(purpose);
                }
            }

            return success;
        }
    }
}