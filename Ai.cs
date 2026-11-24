using System.Text.Json;

class AiApi
{
    private HttpClient client = new HttpClient();


    string apiKey = ""; // <---- רשמו את המפתח שלכם פה


    // gemini של api הכתובת של ה
    private string url;


    public AiApi()
    {
        // בתוספת המפתח שלכם gemini של api הכתובת של ה
        url = "https://generativelanguage.googleapis.com/" +
            $"v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}";
    }


    public async Task<string> SendTextRequest(string message)
    {
        // כאן אפשר לראות את מבנה הבקשה
        var body = new
        {
            contents = new[] // מערך של חלקי השיחה
            {
                new // התוכן של חלקי השיחה
                {
                    role = "user", // הודעה על ידי המשתמש
                    parts = new[]  // התוכן של ההודעה של המשתמש
                    {
                        new // מעבירים את הבקשה לבינה
                        {
                            text = message
                        }
                    }
                }
            }
        };


        // שולחים את הבקשה
        HttpResponseMessage response = await client.PostAsJsonAsync(url, body);


        // קוראים את התשובה של השרת
        string json = await response.Content.ReadAsStringAsync();


        // json ממירים אותו לתשובה מסוג json
        using JsonDocument doc = JsonDocument.Parse(json);


        string answer = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "";


        return answer;
    }
}
