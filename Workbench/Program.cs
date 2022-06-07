using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

var client = new HttpClient();
var request = new HttpRequestMessage()
{
    RequestUri = new Uri("http://localhost:9811/SearchBySkroutzID/SKZ-012323234"),
    Method = HttpMethod.Get,
};
var cookieValue = "ss-id=FyfC1A7UadoCEKWxqUSC; domain=localhost; path=/";
request.Headers.Add("Cookie", cookieValue);

var response = await client.SendAsync(request);
response.EnsureSuccessStatusCode();

var data = await response.Content.ReadFromJsonAsync<ResponseData>();
Console.WriteLine(data);
Console.ReadKey(true);



public class ResponseData
{
    public int StatusCode { get; set; }
    public int TotalRows { get; set; }
    public JsonElement Element { get; set; }
};
