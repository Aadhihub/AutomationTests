using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace TheCocktailDB.ApiTest
{
    [TestFixture]
    public class Tests
    {
        private HttpClient httpClient;

        [SetUp]
        public void SetUp()
        {
            httpClient = new HttpClient();
        }


        [Test]
        [TestCase("vodka")]
        [TestCase("gin")]
        public async Task GivenIngredientsName_ReturnsIngredientsDetails_CheckAlcoholicorNotValuesAsync(string TCIngredient)
        {
            // Arrange
            var apiUrl = "https://www.thecocktaildb.com/api/json/v1/1/search.php?i="+ TCIngredient;

            // Act
            var response = await httpClient.GetAsync(apiUrl);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            responseBody.Should().NotBeNullOrEmpty();

            var responseObject = JObject.Parse(responseBody);

            var ingredients = responseObject["ingredients"];

            ingredients.Should().NotBeNull();
            ingredients.Should().NotBeEmpty();

            // Iterate through each ingredient and validate the fields
            foreach (var ingredient in ingredients)
            {
                ingredient["idIngredient"].Should().NotBeNull();
                ingredient["strIngredient"].Should().NotBeNull();
                ingredient["strDescription"].Should().NotBeNull();
                ingredient["strType"].Should().NotBeNull();

                if (ingredient["strAlcohol"]!.ToString().Equals("Yes"))
                {
                    ingredient["strABV"].Should().NotBeNull();
                }
                else
                {
                    ingredient["strAlcohol"].Should().BeNull();
                    ingredient["strABV"].Should().BeNull();
                }
            }
        }

        [Test]
        public async Task GiveCocktailsName_ReturnsCocktailsDetailsAsync()
        {
            // Arrange
            var apiUrl = "https://www.thecocktaildb.com/api/json/v1/1/search.php?s=margarita";

            // Act
            var response = await httpClient.GetAsync(apiUrl);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            responseBody.Should().NotBeNullOrEmpty();

            var responseObject = JObject.Parse(responseBody);

            //Assert schema properties
            using (new AssertionScope())
            {
                responseObject["drinks"].Should().NotBeNull();
                responseObject["drinks"]![0]!["strDrink"].Should().NotBeNull();
                responseObject["drinks"]![0]!["strTags"].Should().NotBeNull();
                responseObject["drinks"]![0]!["strCategory"].Should().NotBeNull();
                responseObject["drinks"]![0]!["strAlcoholic"]!.Should().NotBeNull();
                responseObject["drinks"]![0]!["strGlass"]!.Should().NotBeNull();
                responseObject["drinks"]![0]!["strInstructions"]!.Should().NotBeNull();
                responseObject["drinks"]![0]!["strIngredient1"]!.Should().NotBeNull();
                responseObject["drinks"]![0]!["strMeasure1"]!.Should().NotBeNull();
                responseObject["drinks"]![0]!["strCreativeCommonsConfirmed"]!.Should().NotBeNull();
                responseObject["drinks"]![0]!["dateModified"]!.Should().NotBeNull();
            }
        }

        [Test]
        public async Task GiveNonExistingCocktail_ReturnsNullDrinksAsync()
        {
            // Arrange
            var apiUrl = "https://www.thecocktaildb.com/api/json/v1/1/search.php?s=TestCocktail";

            // Act
            var response = await httpClient.GetAsync(apiUrl);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            responseBody.Should().NotBeNullOrEmpty();

            var responseObject = JObject.Parse(responseBody);

            responseObject["drinks"].Should().BeEmpty();
        }

        [Test]
        [TestCase("11007")]
        [TestCase("11001")]
        public async Task GiveCocktailID_ReturnsCocktailDetailsAsync(string TcID)
        {
            // Arrange
            var apiUrl = "https://www.thecocktaildb.com/api/json/v1/1/lookup.php?i="+ TcID;

            // Act
            var response = await httpClient.GetAsync(apiUrl);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            responseBody.Should().NotBeNullOrEmpty();

            var responseObject = JObject.Parse(responseBody);

            responseObject["drinks"].Should().NotBeNull();
        }

        [Test]
        public async Task LookupRandomCocktail_ReturnsRandomCocktailAsync()
        {
            // Arrange
            var apiUrl = "https://www.thecocktaildb.com/api/json/v1/1/random.php";

            // Act
            var response = await httpClient.GetAsync(apiUrl);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            responseBody.Should().NotBeNullOrEmpty();

            var responseObject = JObject.Parse(responseBody);

            responseObject["drinks"].Should().NotBeNull();
        }
    }
}
