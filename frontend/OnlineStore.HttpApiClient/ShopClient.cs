using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;
using OnlineStore.HttpModels.Requests;
using OnlineStore.HttpModels.Responses;

namespace OnlineStore.HttpApiClient;

public class ShopClient : IShopClient
{
    private const string DefaultHost = "https://api.mysite.com";
    private readonly string _host;
    private readonly HttpClient _httpClient;
    
    
    public ShopClient(string host = DefaultHost, HttpClient? httpClient = null)
    {
        _host = host;
        _httpClient = httpClient ?? new HttpClient();
    }
    
    
    
    
    // GET ALL PRODUCTS
    public async Task<IReadOnlyList<Product>> GetProducts(CancellationToken cts = default)
    {
        var uri = $"{_host}/products/get_all";
        var response = await _httpClient.GetFromJsonAsync<IReadOnlyList<Product>>(uri,cts);
        return response!;
    }
    
    // GET PRODUCT BY ID
    public async Task<Product> GetProduct(Guid id,CancellationToken cts = default)
    {
        var uri = $"{_host}/products/get_by_id?id={id}";
        var response = await _httpClient.GetFromJsonAsync<Product>(uri,cts);
        return response!;
    }
    
    // ADD PRODUCT
    public async Task AddProduct(Product product,CancellationToken cts = default)
    {
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }
        var uri = $"{_host}/products/add";
        var response = await _httpClient.PostAsJsonAsync(uri, product, cts);
        response.EnsureSuccessStatusCode();
    }
    
    // UPDATE PRODUCT BY ID
    public async Task UpdateProduct(Guid id,Product product,CancellationToken cts = default)
    {
        var uri = $"{_host}/products/update?id={id}";
        var response = await _httpClient.PutAsJsonAsync(uri, product,cts);
        response.EnsureSuccessStatusCode();
    }
    
    // DELETE PRODUCT BY ID
    public async Task DeleteById(Guid id,CancellationToken cts = default)
    {
        var uri = $"{_host}/products/delete_by_id?id={id}";
        var response = await _httpClient.DeleteAsync(uri,cts);
        response.EnsureSuccessStatusCode();
    }

    public async Task Registration(RegisterRequest request, CancellationToken cts = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        var uri = $"{_host}/accounts/register";
        var response = await _httpClient.PostAsJsonAsync(uri,request,cts);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var json = await response.Content.ReadAsStringAsync(cts);
            throw new HttpBadRequestException(json);
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task<LogInResponse> Login(string email, string password, CancellationToken cts = default)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));
        if (password == null) throw new ArgumentNullException(nameof(password));
    
        var uri = $"{_host}/accounts/log_in";
        var responseMessage = await _httpClient.PostAsJsonAsync(uri,email,cts);
        if(responseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            var json = await responseMessage.Content.ReadAsStringAsync(cts);
            throw new HttpBadRequestException(json);
        }
        
        responseMessage.EnsureSuccessStatusCode();
        
        var response = await responseMessage.Content.ReadFromJsonAsync<LogInResponse>(
            cancellationToken: cts);
        
        _httpClient.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", response!.Token);
        
        return response;
    }


    // DELETE ALL PRODUCT
    // public async Task Delete(CancellationToken cts= default)
    // {
    //     var uri = $"{_host}/products/delete";
    //     await _httpClient.DeleteAsync(uri,cts);
    // }
    
    
    
    
    
    
}