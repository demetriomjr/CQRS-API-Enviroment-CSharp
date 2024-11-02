namespace FrontDoor.ServiceConsumer
{
    public static class Microservices
    {
        private static readonly string host = "localhost", port = "5001";

        public static async Task<HttpContent> Get(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Path.Combine(host, port, url));

                if (!response.IsSuccessStatusCode)
                    return null!;

                return response.Content;
            }
        }

        public static async Task<HttpContent> Post(string url, HttpContent content)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(Path.Combine(host, port, url), content);

                if (!response.IsSuccessStatusCode)
                    return null!;

                return response.Content;
            }
        }

        public static async Task<HttpContent> Put(string url, HttpContent content)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PutAsync(Path.Combine(host, port, url), content);

                if (!response.IsSuccessStatusCode)
                    return null!;

                return response.Content;
            }
        }

        public static async Task<HttpContent> Delete(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.DeleteAsync(Path.Combine(host, port, url));

                if (!response.IsSuccessStatusCode)
                    return null!;

                return response.Content;
            }
        }
    }
}
