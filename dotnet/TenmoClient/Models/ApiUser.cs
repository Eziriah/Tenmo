namespace TenmoClient.Models
{
    /// <summary>
    /// Return value from login endpoint
    /// </summary>
    public class ApiUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Model to provide login parameters
    /// </summary>
    public class LoginUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class OtherUser
    {
        public string Username { get; set; }
        public string UserId { get; set; }
    }
    public class Transfer
    {
        public int UserIdToReceive { get; set; }
        public decimal AmountToTransfer { get; set; }

    }
    public class Account
    {
        public decimal Balance { get; set; }
    }
}
