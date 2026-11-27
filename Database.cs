public class User{
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
    public string Email { get; set; } = "";

    // Player — игрок, Coach — тренер
    public string Role { get; set; } = "Player";
}