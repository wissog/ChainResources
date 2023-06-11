using Newtonsoft.Json;

namespace ChainResources
{
    internal class ExchangeRateList
    {
        private Dictionary<string, double> _dicRates;

        public ExchangeRateList(Dictionary<string, double>? dicRates = null)
        {
            if (dicRates == null)
            {
                _dicRates = new Dictionary<string, double>();
            }
            else 
            { 
                _dicRates = dicRates; 
            }
        }

        public override string ToString()
        {
            return string.Join("", _dicRates.Select(kv => $"{kv.Key}={kv.Value}"));
        }

        public void AddRate(string name, double rate)
        {
            _dicRates.Add(name, rate);
        }

        public double GetRate(string name)
        {
            return _dicRates[name];
        }

        public void RemoveRate(string name)
        {
            _dicRates.Remove(name);
        }

        public string GetAsJson()
        {
            var json = JsonConvert.SerializeObject(_dicRates);
            return json;
        }

        public static ExchangeRateList? ConvertFromJson(string json)
        {
            try
            {
                var dictionary = string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                var ratesJson = dictionary?["rates"]?.ToString();
                var dicRates = string.IsNullOrWhiteSpace(ratesJson)
                    ? null
                    : JsonConvert.DeserializeObject<Dictionary<string, double>>(ratesJson);

                var exchangeRateList = new ExchangeRateList(dicRates);
                return exchangeRateList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading json", ex);
            }
        }
    }
}
