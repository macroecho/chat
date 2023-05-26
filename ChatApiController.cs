#region Information
/*
 * =================================================================
 * Copyright(c) 2018 KeiSoft,All Rights Reserved.
 * Author: macro
 * Date: 2018-08-07
 * Version: 1.0.0
 * =================================================================
 */
#endregion

using System.Net.Http;
using System.Web;
using System.Web.Http;

using Keisoft.Apply.Utilities;
using Keisoft.Interface.Utilities;

using Chat.App.Logic;
using Chat.App.Logic.DependencyResolver;

using Chat.Core;
using Chat.Web.Library;

namespace Chat.Web.Controllers.Api
{

    /// <summary>
    /// 聊天 Web Api 控制器。
    /// </summary>
    [Route("gateway/chat/{action}")]
    public class ChatApiController : BaseApiController
    {

        /// <summary>
        /// 建立聊天连接。
        /// </summary>
        /// <returns> http 响应消息。</returns>
        [HttpGet]
        public HttpResponseMessage Connect()
        {
            // 是一个 WebSocket 请求才去处理。 
            if (HttpContext.Current.IsWebSocketRequest)
            {
                ConversationManage conversation;

                try
                {
                    conversation = new ConversationManage(AccountContext.CurrentAccountId());
                }
                catch
                {
                    // 返回错误提示。
                    return GatewayHelp.RequesArgError();
                }

                // 接受 WebSocket 请求。
                HttpContext.Current.AcceptWebSocketRequest(conversation.Start);
            }

            return Request.CreateResponse(System.Net.HttpStatusCode.SwitchingProtocols);
        }

        /// <summary>
        /// 获取聊天文件信息。
        /// </summary>
        /// <returns> http 响应消息；搜索成功 Value 为聊天文件信。</returns>
        [HttpGet, HttpPost]
        public HttpResponseMessage GetFile()
        {
            string id = WebUtility.GetRequestForm("id");

            if (string.IsNullOrWhiteSpace(id))
            {
                return GatewayHelp.RequesArgError();
            }

            // 初始化业务逻辑。
            var logic = LogicDependencyResolver.GetService<IMessageLogic>();
            var logicResult = logic.GetChatFile(new System.Guid(id));

            if (logicResult.IsSucceed)
            {
                return GatewayHelp.RequesSucceed(logicResult.Value);
            }

            // 返回错误提示。
            return GatewayHelp.RequesError(logicResult.StateCode, logicResult.Message);
        }

    }
}
