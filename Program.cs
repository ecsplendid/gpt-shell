using System.Diagnostics;
using System.Net;
using Newtonsoft.Json.Linq;

// make a prompt string from the first element of args
var prompt = string.Join(" ", args);

// if the prompt contains (z) then don't do the following
prompt = prompt.Contains("zsh") 
    ? $"single line, succinct, minimally verbose zshell/zsh command to {prompt.Replace("zsh","")}:".Trim() 
    : $"single line, succinct, minimally verbose powershell command using aliases to {prompt}:";

var request = (HttpWebRequest)WebRequest.Create("https://api.openai.com/v1/completions");
request.ContentType = "application/json";

// load in const string key string from system environment variables
var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

request.Headers.Add("Authorization", $@"Bearer {key}");
request.Method = "POST";

await using (var streamWriter = new StreamWriter(request.GetRequestStream()))
{
    prompt = prompt.Replace("\"", "");
    
    // replace new lines in the summary with \n
    prompt = prompt.Replace("\n", "\\n");
    
    var json = "{\"model\": \"text-davinci-002\",\"prompt\": \""
               +prompt
               +"\",\"temperature\": 0.9,\"max_tokens\": 256,\"top_p\": 1,\"frequency_penalty\": 0,\"presence_penalty\": 0}";
    
    await streamWriter.WriteAsync(json);
}

var response = (HttpWebResponse)request.GetResponse();
using (var streamReader = new StreamReader(response.GetResponseStream()))
{
    var json = await streamReader.ReadToEndAsync();
    
    var root = JObject.Parse(json);
    
    if (root != null)
    {
        var text = root["choices"][0]["text"].Value<string>();
    
        Console.Write(text.Trim());
    }
}