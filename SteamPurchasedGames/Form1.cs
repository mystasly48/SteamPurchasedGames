using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteamPurchasedGames {
  public partial class Form1 : Form {
    public Form1() {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      button1.Enabled = false;
      Game[] games = ExtractGamesFromHtml(richTextBox1.Text);
      foreach (Game game in games) {
        dataGridView1.Rows.Add(game.Date.ToShortDateString(), game.Title, game.Method);
      }
      button1.Enabled = true;
    }

    private Game[] ExtractGamesFromHtml(string html) {
      List<Game> games = new List<Game>();
      // <div class="free_license_remove_link"><a href="javascript:RemoveFreeLicense(なんたらかんたら);">（なんたらかんたら）</a></div>
      html = Regex.Replace(html, @"<div class=""free_license_remove_link"">.*?</div>", "", RegexOptions.Singleline);

      MatchCollection items = Regex.Matches(html,
        @"<tr>\s*?<td class=""license_date_col"">(?<date>.*?)</td>\s*?<td>(?<title>.*?)</td>\s*?<td class=""license_acquisition_col"">(?<method>.*?)</td>\s*?</tr>",
        RegexOptions.Singleline);
      foreach (Match item in items) {
        Console.WriteLine("Date: {0}, Title: {1}, Method: {2}", item.Groups["date"].Value, item.Groups["title"].Value, item.Groups["method"].Value);
        games.Add(new Game() {
          Date = DateTime.Parse(item.Groups["date"].Value.Trim()),
          Title = item.Groups["title"].Value.Trim(),
          //Method = ToMethod(item.Groups["method"].Value.Trim())
          Method = item.Groups["method"].Value.Trim()
        });
      }
      return games.ToArray();
    }

    //private Method ToMethod(string method) {
    //  switch (method) {
    //    case "Steam ストア":
    //      return Method.Store;
    //    case "小売店":
    //      return Method.Retail;
    //    case "ギフト／ゲストパス":
    //      return Method.Gift;
    //    case "無料サービス":
    //      return Method.Complimentary;
    //    default:
    //      throw new ArgumentException();
    //  }
    //}
  }

  public class Game {
    public DateTime Date; // license_date_col
    public string Title;
    // you have to search all the items from Title to know the app id.
    // it means this processing takes a long time and i dont recommend.
    // by the way is there any library that can get the appid from title fast.
    //public long Id; 
    //public Method Method; // license_acquisition_col
    public string Method;
  }

  //public enum Method {
  //  /* にほんごのみたいおうしています This system only supports Japanese html.
  //   * Store: "Steam Store", "Steam ストア"
  //   * Retail: "Retail", "小売店"
  //   * Gift: "Gift/Guest Pass ", "ギフト／ゲストパス"
  //   * Free: "Complimentary", "無料サービス"
  //   */
  //  Store, Retail, Gift, Complimentary
  //}
}
