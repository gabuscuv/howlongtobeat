using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace HowLongToBeat.Tests
{
	public class HLTBHtmlParserTests
	{
		[Test]
		public async Task Test_HTML_Parser()
		{
			// arrange
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

			// act
			var ws = new HLTBHtmlParser();
			var result = await ws.GetGameDetailsAsync(html);

			// assert
			var pf = new Game(title: "Pathfinder: Kingmaker", imgURL: "https://howlongtobeat.com/games/60050_Pathfinder_Kingmaker.jpg", main: "74½ Hours", mainandextras: "126 Hours", completionist: "188 Hours");
			var actual1 = result.First();
			Assert.That(actual1.Title, Is.EqualTo(pf.Title));
			Assert.That(actual1.ImgURL, Is.EqualTo(pf.ImgURL));
			Assert.That(actual1.Main, Is.EqualTo(pf.Main));
			Assert.That(actual1.MainAndExtras, Is.EqualTo(pf.MainAndExtras));
			Assert.That(actual1.Completionist, Is.EqualTo(pf.Completionist));

			var wor = new Game(title: "Pathfinder: Wrath of the Righteous", imgURL: "https://howlongtobeat.com/games/83856_Pathfinder_Wrath_of_the_Righteous.jpg", main: "57½ Hours", mainandextras: "125 Hours", completionist: "200 Hours");
			var actual2 = result.Last();
			Assert.That(actual2.Title, Is.EqualTo(wor.Title));
			Assert.That(actual2.ImgURL, Is.EqualTo(wor.ImgURL));
			Assert.That(actual2.Main, Is.EqualTo(wor.Main));
			Assert.That(actual2.MainAndExtras, Is.EqualTo(wor.MainAndExtras));
			Assert.That(actual2.Completionist, Is.EqualTo(wor.Completionist));
		}
	}
}