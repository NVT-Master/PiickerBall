namespace PCM.Api.DTOs.Login.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpireAt { get; set; }
    }

}
