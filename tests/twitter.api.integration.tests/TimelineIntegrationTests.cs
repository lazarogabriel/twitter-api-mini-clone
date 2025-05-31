using System.Net.Http.Json;
using System.Threading.Tasks;
using twitter.api.web.Models.Responses;
using twitter.api.integration.tests.Factory;
using twitter.api.web.Models.Responses.Base;
using System;
using System.Net.Http;
using System.Linq;

namespace twitter.api.integration.tests
{
    public class TimelineIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TimelineIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTimeline_ShouldReturnTweetsFromFollowedUsers()
        {
            // Arrange
            // 1. Crea 3 usuarios (UserA, UserB, UserC)
            // 2. UserA sigue a UserB y UserC
            // 3. UserB y UserC hacen tweets
            // 4. UserA pide timeline

            // 1. Crea UserA
            var userA = await CreateUser("userA");
            var userB = await CreateUser("userB");
            var userC = await CreateUser("userC");

            // 2. UserA sigue a UserB y UserC
            await Follow(userA.Id, userB.Id);
            await Follow(userA.Id, userC.Id);

            // 3. UserB y UserC publican tweets
            var tweetB = await CreateTweet(userB.Id, "Tweet de B");
            var tweetC = await CreateTweet(userC.Id, "Tweet de C");

            // 4. Act
            var response = await GetTimeline(userA.Id);

            // Assert
            Assert.NotNull(response);

            // Chequear cantidad de tweets
            Assert.Equal(2, response.Items.Count());

            // Chequear que están los tweets correctos (por contenido)
            var contents = response.Items.Select(x => x.Content).ToList();
            Assert.Contains("Tweet de B", contents);
            Assert.Contains("Tweet de C", contents);

            // Chequear que los autores sean los correctos
            var authorIds = response.Items.Select(x => x.AuthorId).ToList();
            Assert.Contains(userB.Id, authorIds);
            Assert.Contains(userC.Id, authorIds);

            // Chequear que el tweet B tenga el autor correcto
            var b = response.Items.FirstOrDefault(x => x.Content == "Tweet de B");
            Assert.NotNull(b);
            Assert.Equal(userB.Id, b.AuthorId);

            // Chequear que el tweet C tenga el autor correcto
            var c = response.Items.FirstOrDefault(x => x.Content == "Tweet de C");
            Assert.NotNull(c);
            Assert.Equal(userC.Id, c.AuthorId);

            // Chequear que la respuesta no contiene tweets de userA (solo de los seguidos)
            Assert.DoesNotContain(response.Items, x => x.AuthorId == userA.Id);
        }

        #region Private Methods

        private async Task<BasicUserResponse> CreateUser(string username)
        {
            var result = await _client.PostAsJsonAsync("/Users", new { Username = username });
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<BasicUserResponse>();
        }

        private async Task Follow(Guid followerId, Guid toFollowId)
        {
            _client.DefaultRequestHeaders.Remove("x-user-id");
            _client.DefaultRequestHeaders.Add("x-user-id", followerId.ToString());

            var result = await _client.PostAsync($"/Users/{toFollowId}/followers", null);

            result.EnsureSuccessStatusCode();
        }

        private async Task<TweetResponse> CreateTweet(Guid authorId, string content)
        {
            _client.DefaultRequestHeaders.Remove("x-user-id");
            _client.DefaultRequestHeaders.Add("x-user-id", authorId.ToString());

            var result = await _client.PostAsJsonAsync("/Tweets", new { Content = content });

            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<TweetResponse>();
        }

        private async Task<PaginatedResponse<TweetResponse>> GetTimeline(Guid userId)
        {
            _client.DefaultRequestHeaders.Remove("x-user-id");
            _client.DefaultRequestHeaders.Add("x-user-id", userId.ToString());

            var response = await _client.GetAsync("/Timeline");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaginatedResponse<TweetResponse>>();
        }

        #endregion
    }
}