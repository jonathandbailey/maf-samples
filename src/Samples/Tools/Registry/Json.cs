using System.Text.Json;

namespace Tools.Registry;

public static class Json
{
    public static readonly JsonSerializerOptions FunctionCallSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}