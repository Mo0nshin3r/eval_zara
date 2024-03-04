using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;
using System.Text;
using MySqlConnector;

int numeroLigne = 0;
//db connection
string m_strMySQLConnectionString;
m_strMySQLConnectionString = "server=localhost;userid=root;port=6033;password=root;database=clothing";

using (var mysqlconnection = new MySqlConnection(m_strMySQLConnectionString))
{
    mysqlconnection.Open();
    bool premiereLigne = true;
    int i = 0;
    Random random = new Random();
    foreach (string line in System.IO.File.ReadAllLines(@"D:\store_zara2.csv",
                 System.Text.Encoding.UTF8))
    {   //csv reader
        string image = "";
        var fix = line.Replace("'", "");
        fix = line.Replace("’", "");
        var columns = line.Split(",");
        string brand = "'" + columns[0] + "'";
        string url = "'" + columns[1] + "'";
        string sku = "'" + columns[2] + "'";
        string name = "'" + columns[3] + "'";
        string price = columns[4];
        string currency = "'" + columns[5] + "'";
        string scrapedAt = "'" + columns[6] + "'";
        string terms = "'" + columns[7] + "'";
        string section = "'" + columns[8] + "'";

        //replacing space by "-" in the name column
        name = name.Replace(" ", "-");
        numeroLigne = numeroLigne++;

        //converting usd to chf
        double priceConvert;
        double priceCHF = 0;
        Console.WriteLine(price);
        Console.WriteLine(line);

        priceConvert = Convert.ToDouble(price);
        priceCHF = priceConvert / 1.1;
        priceCHF = Math.Round(priceCHF, 2);
        double priceUSD = priceConvert;
        string finalPrice = "'" + priceCHF + "CHF (" + price + "$)" + "'";
        //condition for image name
        if (priceCHF < 25)
        {
            image = "low.png";

        }
        else if (priceCHF > 25 && priceCHF < 50)
        {
            image = "ok.png";

        }
        else if (priceCHF > 50)
        {
            image = "high.png";

        }
        //inserting data into db for every item above 5usd
        if (priceUSD > 5)
        {
            string sql =
                $"INSERT INTO t_clothes( brand, url, sku, name, priceUSD, priceCHF, scrapedAt, terms, section, priceImage) VALUES ({/brand + "," + url + "," + sku + "," + name + "," + priceUSD + "," + priceCHF + "," + scrapedAt + "," + terms + "," + section + "," + "'" + image + "'"})";
            using (MySqlCommand cmd = mysqlconnection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 300;
                cmd.CommandText = sql;
                try
                {
                    i += cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(sql + "\n" + e.Message);
                }
            }
        }
    }

    Console.WriteLine($"{i} lignes ajoutees");
    premiereLigne = false;
    mysqlconnection.Close();
}