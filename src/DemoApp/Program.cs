using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Azure OpenAI demos");
var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json");

var configuration = builder.Build();

// Examples taken from here:
// https://github.com/Azure-Samples/openai-dotnet-samples/blob/main/qna.ipynb

var endpoint = configuration.GetValue<string>("OpenAI:Endpoint");
var modelName = configuration.GetValue<string>("OpenAI:ModelName");

//var credentials = new Azure.AzureKeyCredential(AOAI_KEY);
var openAIClient = new OpenAIClient(new Uri(endpoint), new DefaultAzureCredential());

var prompt =
    """
    I am a highly intelligent question answering bot. If you ask me a question that is rooted in truth, I will give you the answer. If you ask me a question that is nonsense, trickery, or has no clear answer, I will respond with \"Unknown\".

    Q: What is human life expectancy in the United States?
    A: Human life expectancy in the United States is 78 years.

    Q: Who was president of the United States in 1955?
    A: Dwight D. Eisenhower was president of the United States in 1955.

    Q: Which party did he belong to?
    A: He belonged to the Republican Party.

    Q: What is the square root of banana?
    A: Unknown

    Q: How does a telescope work?
    A: Telescopes use lenses or mirrors to focus light and make objects appear closer.

    Q: Where were the 1992 Olympics held?
    A: The 1992 Olympics were held in Barcelona, Spain.

    Q: How many squigs are in a bonk?
    A: Unknown

    Q: Where is the Valley of Kings?
    A:
    """;

var completionOptions = new CompletionsOptions
{
    Prompts = { prompt },
    MaxTokens = 100,
    Temperature = 0f,
    FrequencyPenalty = 0.0f,
    PresencePenalty = 0.0f,
    NucleusSamplingFactor = 1 // Top P
};

var response = await openAIClient.GetCompletionsAsync(modelName, completionOptions);

Console.WriteLine(response.Value.Choices.First().Text);