using System.Net;
using System.Web;

namespace Common.Utilities
{
    public static class TelegramUtil
    {
        public static void SendToTelegram(string value)
        {
            var client = new WebClient { Encoding = System.Text.Encoding.UTF8 };
                client.DownloadString("https://api.telegram.org/bot5286892246:AAGsKEo1RTIGEAJxPuN67PNHVJJxl3tPRHE/sendMessage?chat_id=-1001691255653&text=" + HttpUtility.UrlEncode(value));
       
        }
    }
}
