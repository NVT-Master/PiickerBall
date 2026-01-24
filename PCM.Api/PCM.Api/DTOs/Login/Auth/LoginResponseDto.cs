namespace PCM.Api.DTOs.Login.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpireAt { get; set; }
        public UserDto User { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }

    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
    }
}
