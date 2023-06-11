// See https://aka.ms/new-console-template for more information
using ChainResources;
using ChainResources.Interfaces;
using ChainResources.Storages;

Console.WriteLine("Hello, Mize!");

const string FILE_NAME = "ExchangeRateList.json";
string directory = Directory.GetCurrentDirectory();

var storages = new List<IStorage<ExchangeRateList?>>
{
    new MemoryStorage<ExchangeRateList?>(),
    new FileSystemStorage(directory, FILE_NAME),
    new WebServiceStorage()
};

var chainResource = new ChainResource<ExchangeRateList?>(storages);
ExchangeRateList? exchangeRateList = null; 

// try running some parallel processes to read the value
var taskList = new List<Task<ExchangeRateList?>>();
for (int j=0; j < 100; j++)
{
    var task = Task.Run(() => GetValue(chainResource));
    taskList.Add(task);
}

await Task.WhenAll(taskList);

exchangeRateList = taskList[0].Result;  


var str = exchangeRateList?.ToString();

Console.WriteLine($"Write Counter: {chainResource.WriteCounter}, Read Counter: {chainResource.ReadCounter}");
Console.WriteLine($"exchangeRateList exists: {!string.IsNullOrWhiteSpace(str)}");
Console.WriteLine(str);


async Task<ExchangeRateList?> GetValue(ChainResource<ExchangeRateList?> resource)
{
    return await resource.GetValue();
}