using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HowLongToBeat.Tests
{
	public class HLTBWebScraperTests
	{

		[Test]
		public async Task RealWorldTest()
		{
			
			var client = new HowLongToBeat.HLTBWebScraper(new System.Net.Http.HttpClient());
			var result = await client.Search("Gravity Rush");

			ICollection<string> titles = result.Select(element=>element.Title).ToList();
			CollectionAssert.Contains(titles,"Gravity Rush");
			CollectionAssert.Contains(titles,"Gravity Rush: Remastered");
			
		}

		[Test]
		public async Task ReusingSameHttpClient()
		{
			IList<Game> result;
			var client = new HowLongToBeat.HLTBWebScraper(new System.Net.Http.HttpClient());
			result = await client.Search("Gravity Rush");
			try
			{
				result = await client.Search("Atelier Rorona Plus: The Alchemist of Arland");
			}
			//    System.FormatException : Cannot add value because header 'Referer' does not support multiple values.
			catch(System.FormatException)
			{
				Assert.Fail();
			}
			Assert.Pass();
		}

		[Test]
		public async Task Test_Search()
		{
			// arrange
			var query = "pathfinder";
			var content = new StringContent(ExampleContent());
			var fakeHandler = new FakeHttpMessageHandler(content);
			var client = new HttpClient(fakeHandler);
			var ws = new HLTBWebScraper(client);

			// act
			var actual = await ws.Search(query);

			// assert
			var pathfinder = new Game(title: "Pathfinder: Kingmaker", imgURL: "https://howlongtobeat.com/games/60050_Pathfinder_Kingmaker.jpg", main: "74½ Hours", mainandextras: "126 Hours", completionist: "188 Hours");
			var wor = new Game(title: "Pathfinder: Wrath of the Righteous", imgURL: "https://howlongtobeat.com/games/83856_Pathfinder_Wrath_of_the_Righteous.jpg", main: "57½ Hours", mainandextras: "125 Hours", completionist: "200 Hours");
			var expected = new List<Game> { pathfinder, wor };

			Assert.That(JsonSerializer.Serialize(actual), Is.EqualTo(JsonSerializer.Serialize(expected)));
		}

		public static string ExampleContent()
		{
			var html =
				@"
<div class=""global_padding shadow_box back_blue center"">
	<h3> We Found 8 Games for ""Pathfinder"" </h3>
</div>
<ul>
	<div class=""clear""></div>
	<li class=""back_darkish""
		style=""background-image:linear-gradient(rgb(31, 31, 31), rgba(31, 31, 31, 0.9)), url('/games/60050_Pathfinder_Kingmaker.jpg')"">
		<div class=""search_list_image"">
			<a aria-label=""Pathfinder Kingmaker"" title=""Pathfinder Kingmaker"" href=""game?id=60050"">
				<img alt=""Box Art"" src=""/games/60050_Pathfinder_Kingmaker.jpg"" />
			</a>
		</div>
		<div class=""search_list_details"">
			<h3 class=""shadow_text"">
				<a class=""text_white"" title=""Pathfinder Kingmaker"" href=""game?id=60050"">Pathfinder: Kingmaker</a>
			</h3>
			</h3>
			<div class=""search_list_details_block"">
				<div>
					<div class=""search_list_tidbit text_white shadow_text"">Main Story</div>
					<div class=""search_list_tidbit center time_70"">74&#189; Hours </div>
					<div class=""search_list_tidbit text_white shadow_text"">Main + Extra</div>
					<div class=""search_list_tidbit center time_100"">126 Hours </div>
					<div class=""search_list_tidbit text_white shadow_text"">Completionist</div>
					<div class=""search_list_tidbit center time_100"">188 Hours </div>
				</div>
			</div>
		</div>
	</li>
	<li class=""back_darkish""
		style=""background-image:linear-gradient(rgb(31, 31, 31), rgba(31, 31, 31, 0.9)), url('/games/83856_Pathfinder_Wrath_of_the_Righteous.jpg')"">
		<div class=""search_list_image"">
			<a aria-label=""Pathfinder Wrath of the Righteous"" title=""Pathfinder Wrath of the Righteous""
				href=""game?id=83856"">
				<img alt=""Box Art"" src=""/games/83856_Pathfinder_Wrath_of_the_Righteous.jpg"" />
			</a>
		</div>
		<div class=""search_list_details"">
			<h3 class=""shadow_text"">
				<a class=""text_white"" title=""Pathfinder Wrath of the Righteous"" href=""game?id=83856"">Pathfinder: Wrath
					of the Righteous</a>
			</h3>
			<div class=""search_list_details_block"">
				<div>
					<div class=""search_list_tidbit text_white shadow_text"">Main Story</div>
					<div class=""search_list_tidbit center time_50"">57&#189; Hours </div>
					<div class=""search_list_tidbit text_white shadow_text"">Main + Extra</div>
					<div class=""search_list_tidbit center time_100"">125 Hours </div>
					<div class=""search_list_tidbit text_white shadow_text"">Completionist</div>
					<div class=""search_list_tidbit center time_40"">200 Hours </div>
				</div>
			</div>
		</div>
	</li>
	<div class=""clear""></div>
</ul>
";

			return html;
		}
	}

	public class FakeHttpMessageHandler : HttpMessageHandler
	{
		private readonly HttpContent content;

		public FakeHttpMessageHandler(HttpContent content)
		{
			this.content = content;
		}

		public HttpRequestMessage? RequestMessage { get; private set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			RequestMessage = request;
			var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = content
			};
			return Task.FromResult(responseMessage);
		}
	}
}