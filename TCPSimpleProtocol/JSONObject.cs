using System.Text.Json;

public class JSONObject
{
    public string Cmd { get; set; }
    public int FirstNum { get; set; }
    public int SecondNum { get; set; }

    public static JSONObject Deserialize(string jsonString)
    {
        return JsonSerializer.Deserialize<JSONObject>(jsonString);
    }

    public JSONObject()
    {
    }
    public JSONObject(string cmd, int firstNum, int secondNum)
    {
        Cmd = cmd;
        FirstNum = firstNum;
        SecondNum = secondNum;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}