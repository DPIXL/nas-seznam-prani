using HtmlAgilityPack;

namespace darkovy_kosik.HelperMethods;

public class GetImageUrl {
    public async Task<string?> GetImageUrlAsync(string itemUrl)
    {
        try
        {
            using var client = new HttpClient();
            
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            
            var html = await client.GetStringAsync(itemUrl);
            
            var document = new HtmlDocument();
            document.LoadHtml(html);
            
            var ogImageNode = document.DocumentNode.SelectSingleNode("//meta[@property='og:image']");

            if (ogImageNode != null)
            {
                return ogImageNode.GetAttributeValue("content", null);
            }
        }
        catch
        {
            return null;
        }

        return null;
    }
}