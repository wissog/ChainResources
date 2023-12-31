﻿using ChainResources.Interfaces;

namespace ChainResources.Storages
{
    internal class WebServiceStorage : IStorage<ExchangeRateList?>
    {
        const string JSON_URL = "https://openexchangerates.org/api/latest.json?app_id=af92c721f0a8435cac9bbefefd68b0ff";
        static readonly HttpClient client = new();

        public WebServiceStorage() { }

        public string Name => "WebService";

        public bool IsExpired() => false;

        public async Task<ExchangeRateList?> Read()
        {
            try
            {
                string json = await client.GetStringAsync(JSON_URL);
                var exchangeRateList = await Task.Run(() => ExchangeRateList.ConvertFromJson(json));
                return exchangeRateList;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Reading from WebService Error: {0} ", e.Message);
                return null;
            }
        }
    }
}
