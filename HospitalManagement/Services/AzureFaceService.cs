using HospitalManagement.Interfaces;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

public class AzureFaceService : IFaceService
{
    private readonly string _subscriptionKey;
    private readonly string _endpoint;
    private readonly string _personGroupId = "mediconnect_patients";
    private readonly HttpClient _httpClient;

    public AzureFaceService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _subscriptionKey = configuration["FaceApi:ApiKey"];
        _endpoint = configuration["FaceApi:Endpoint"]?.TrimEnd('/');

        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
    }

    public async Task CreatePersonGroupIfNotExistsAsync()
    {
        var groupCheckUrl = $"{_endpoint}/face/v1.0/persongroups/{_personGroupId}";
        var checkResponse = await _httpClient.GetAsync(groupCheckUrl);
        if (!checkResponse.IsSuccessStatusCode)
        {
            var createUrl = $"{_endpoint}/face/v1.0/persongroups/{_personGroupId}";
            var body = new { name = "Registered Users" };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            await _httpClient.PutAsync(createUrl, content);
        }
    }

    public async Task<string> RegisterFaceAsync(string userName, Stream imageStream)
    {
        var personUrl = $"{_endpoint}/face/v1.0/persongroups/{_personGroupId}/persons";
        var personBody = new { name = userName };
        var personContent = new StringContent(JsonConvert.SerializeObject(personBody), Encoding.UTF8, "application/json");
        var personResponse = await _httpClient.PostAsync(personUrl, personContent);
        var personJson = await personResponse.Content.ReadAsStringAsync();
        dynamic personData = JsonConvert.DeserializeObject(personJson);
        string personId = personData.personId;

        var faceUrl = $"{_endpoint}/face/v1.0/persongroups/{_personGroupId}/persons/{personId}/persistedFaces";
        var byteContent = new StreamContent(imageStream);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        await _httpClient.PostAsync(faceUrl, byteContent);

        var trainUrl = $"{_endpoint}/face/v1.0/persongroups/{_personGroupId}/train";
        await _httpClient.PostAsync(trainUrl, null);

        return personId;
    }

    public async Task<string?> IdentifyFaceAsync(Stream imageStream)
    {
        var detectUrl = $"{_endpoint}/face/v1.0/detect?returnFaceId=true";
        var byteContent = new StreamContent(imageStream);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var detectResponse = await _httpClient.PostAsync(detectUrl, byteContent);
        var detectJson = await detectResponse.Content.ReadAsStringAsync();
        dynamic faces = JsonConvert.DeserializeObject(detectJson);
        if (faces.Count == 0) return null;

        string faceId = faces[0].faceId;

        var identifyUrl = $"{_endpoint}/face/v1.0/identify";
        var identifyBody = new
        {
            personGroupId = _personGroupId,
            faceIds = new[] { faceId },
            maxNumOfCandidatesReturned = 1,
            confidenceThreshold = 0.5
        };
        var identifyContent = new StringContent(JsonConvert.SerializeObject(identifyBody), Encoding.UTF8, "application/json");

        var identifyResponse = await _httpClient.PostAsync(identifyUrl, identifyContent);
        var identifyJson = await identifyResponse.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(identifyJson);

        if (result.Count == 0 || result[0].candidates.Count == 0)
            return null;

        return result[0].candidates[0].personId;
    }
}
