using System.Text;
using System.Text.Json;

namespace deaplom.Services
{
    public class GeminiService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        private const string SystemPrompt =
            "Ты — оператор технической поддержки мобильного приложения «ТИС Диалог» (интернет-провайдер). " +
            "Отвечай кратко (2-4 предложения), вежливо, на русском языке. " +
            "Помогай с вопросами о тарифах, услугах, оплате, балансе, подключении интернета. " +
            "Если вопрос не по теме — вежливо перенаправь к профильному специалисту.";

        public GeminiService(HttpClient http)
        {
            _http = http;
            _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
        }

        public async Task<string> GetReplyAsync(string userMessage)
        {
            if (!string.IsNullOrEmpty(_apiKey))
            {
                var aiReply = await CallGeminiAsync(userMessage);
                if (aiReply != null) return aiReply;
            }

            return GetRuleBasedReply(userMessage);
        }

        private async Task<string?> CallGeminiAsync(string userMessage)
        {
            try
            {
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";

                var body = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = $"{SystemPrompt}\n\nВопрос клиента: {userMessage}" }
                            }
                        }
                    },
                    generationConfig = new { maxOutputTokens = 256, temperature = 0.7 }
                };

                var response = await _http.PostAsync(url,
                    new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode) return null;

                var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                return doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();
            }
            catch
            {
                return null;
            }
        }

        private static string GetRuleBasedReply(string message)
        {
            var m = message.ToLower();

            if (m.Contains("тариф"))
                return "По вопросу тарифного плана: для смены или уточнения условий тарифа перейдите в раздел «Тарифы» в приложении. Если нужна помощь — уточните, что именно вас интересует.";

            if (m.Contains("отключ") || m.Contains("услуг"))
                return "Для управления услугами перейдите в раздел «Услуги» приложения. Там вы можете отключить или подключить нужные опции. Если возникли трудности — опишите ситуацию подробнее.";

            if (m.Contains("платёж") || m.Contains("оплат") || m.Contains("расход") || m.Contains("счёт"))
                return "История платежей и расходов доступна в разделе «История платежей». Если вас интересует конкретная транзакция — уточните дату и сумму, мы разберёмся.";

            if (m.Contains("баланс") || m.Contains("пополн"))
                return "Текущий баланс отображается на главном экране приложения. Для пополнения нажмите кнопку «Пополнить баланс» и выберите удобный способ оплаты.";

            if (m.Contains("интернет") || m.Contains("скорость") || m.Contains("не работ") || m.Contains("соединен"))
                return "По вопросу работы интернета: уточните ваш адрес подключения, и мы проверим состояние сети. Также попробуйте перезагрузить роутер — это решает большинство проблем.";

            if (m.Contains("пароль") || m.Contains("войти") || m.Contains("вход") || m.Contains("логин"))
                return "Если забыли пароль — воспользуйтесь функцией восстановления на экране входа. Если аккаунт заблокирован — свяжитесь с поддержкой по номеру 8-800-000-00-00.";

            if (m.Contains("привет") || m.Contains("здравствуй") || m.Contains("добрый"))
                return "Добрый день! Я помощник поддержки ТИС Диалог. Чем могу помочь? Вы можете спросить о тарифах, услугах, балансе или технических вопросах.";

            return "Спасибо за обращение! Ваш вопрос принят. Специалист ответит в ближайшее время (обычно в течение 2 часов в рабочее время). Если вопрос срочный — позвоните на горячую линию: 8-800-000-00-00.";
        }
    }
}
