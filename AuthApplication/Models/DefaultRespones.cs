namespace AuthApplication.Models
{
    public class DefaultRespones
    {
        public object? Data { get; set; }
        public string? Message { get; set; }
    }
	public enum RegisterStatus
	{
		Success,
		UserIsExist,
		UserNameIsNotValid,
		EmailIsNotValid,
		PhoneIsNotValid,
		PasswordIsNotValid,
		PasswordConfirmIsNotValid,
		PasswordConfirmIsNotMatch
	}
}
