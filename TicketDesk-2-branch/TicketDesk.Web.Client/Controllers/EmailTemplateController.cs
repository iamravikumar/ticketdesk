﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDesk.Web.Client.Models;
using System.IO;
using System.ComponentModel.Composition;
using TicketDesk.Domain.Services;
using System.Text;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace TicketDesk.Web.Client.Controllers
{
    [HandleError]
    [Export("EmailTemplate", typeof(IController))]
    public partial class EmailTemplateController : Controller
    {
        [Import(typeof(INotificationSendingService))]
        private TicketDesk.Domain.Services.NotificationSendingService noteService { get; set; }

        public string Test()
        {
            noteService.ProcessWaitingTicketEventNotifications();
            return "Notes have been processed";
        }

        public string GenerateTicketNotificationHtmlEmail(TicketDesk.Domain.Models.TicketEventNotification notification)
        {
            var ticket = notification.TicketComment.Ticket;
           
            var vd = new ViewDataDictionary(notification);

            using (StringWriter sw = new StringWriter())
            {

                var fakeResponse = new HttpResponse(sw);
                var fakeContext = new HttpContext(new HttpRequest("", "http://mySomething/", ""), fakeResponse);
                var fakeControllerContext = new ControllerContext
                (
                    new HttpContextWrapper(fakeContext),
                    new RouteData(),
                    this
                );
                fakeControllerContext.RouteData.Values.Add("controller", "EmailTemplate");
                
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(fakeControllerContext, "~/Views/EmailTemplate/HtmlEmailTemplate.ascx");
                ViewContext vc = new ViewContext(fakeControllerContext, new FakeView(), new ViewDataDictionary(), new TempDataDictionary(), sw);

                HtmlHelper h = new HtmlHelper(vc, new ViewPage());

                h.RenderPartial("~/Views/EmailTemplate/HtmlEmailTemplate.ascx", notification);
                
                return sw.GetStringBuilder().ToString();
            }
        }

        public class FakeView : IView
        {
            #region IView Members
            public void Render(ViewContext viewContext, System.IO.TextWriter writer)
            {
                throw new NotImplementedException();
            }
            #endregion
        }


    }
}
