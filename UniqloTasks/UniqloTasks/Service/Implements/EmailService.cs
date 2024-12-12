using Microsoft.Extensions.Options;
using NuGet.Protocol.Plugins;
using System.Net;
using System.Net.Mail;
using UniqloTasks.Helpers;
using UniqloTasks.Service.Abstracts;

namespace UniqloTasks.Service.Implements
{
	public class EmailService : IEmailService
	{
		readonly SmtpClient _client;
		readonly MailAddress _from;
		readonly HttpContext Context;
		public EmailService(IOptions<SmtpOptions> option, IHttpContextAccessor acc)
		{
			var opt = option.Value;
			_client = new(opt.Host, opt.Port);
			_client.Credentials = new NetworkCredential(opt.Sender, opt.Password);
			_client.EnableSsl = true;
			_from = new MailAddress(opt.Sender, "Uniqlo");
			Context = acc.HttpContext;
		}

		public void SendEmailConfirmation(string reciever, string name, string token)
		{
			MailAddress to = new(reciever);
			MailMessage msg = new MailMessage(_from, to);
			msg.IsBodyHtml = true;
			msg.Subject = "Confirm your email adress";
			string url = Context.Request.Scheme + "://" + Context.Request.Host + "/Account/VerifyEmail?code=" + token + "&user=" + name;
			msg.Body = EmailTemplates.VerifyEmail.Replace("__$name", name).Replace("__$link", url);
			_client.Send(msg);
		}
	}
}
