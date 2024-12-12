namespace UniqloTasks.Service.Abstracts
{
	public interface IEmailService
	{
		void SendEmailConfirmation(string reciever, string name, string token);

	}
}
