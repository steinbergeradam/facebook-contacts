using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using MixERP.Net.VCards;
using MixERP.Net.VCards.Types;
using MixERP.Net.VCards.Models;
using MixERP.Net.VCards.Serializer;

namespace facebook_contacts
{
    class Program
    {
        static void Main(string[] args)
        {
            var html = new HtmlDocument();
            html.Load("your_address_books.html");
            var document = html.DocumentNode;
            var contacts = document.QuerySelectorAll(".pam");
            foreach (var node in contacts) {
                var vcard = new VCard() {
                    Version = VCardVersion.V4
                };
                var name = node.QuerySelector("._3-96._2pio._2lek._2lel").InnerText;
                vcard.FormattedName = name;
                var nameItems = name.Split(' ');
                if (nameItems.Length == 1) {
                    vcard.FirstName = name;
                } else if (nameItems.Length == 2) {
                    vcard.FirstName = nameItems[0];
                    vcard.LastName = nameItems[1];
                } else if (nameItems.Length > 2) {
                    vcard.FirstName = nameItems[0];
                    vcard.MiddleName = nameItems[1];
                    vcard.LastName = nameItems[2];
                }
                var infos = node.QuerySelectorAll("._3hls");
                var phones = new List<Telephone>();
                var emails = new List<Email>();
                foreach (var info in infos) {
                    var text = info.InnerText.Replace("&#064;", "@");
                    if (text.Contains("@")) {
                        emails.Add(new Email() {
                            EmailAddress = text
                        });
                    } else {
                        phones.Add(new Telephone() {
                            Number = text
                        });
                    }
                }
                vcard.Telephones = phones;
                vcard.Emails = emails;
                var serialized = vcard.Serialize();
                using (StreamWriter file = File.AppendText(@"contacts-new.vcf")) {
                    file.WriteLine(serialized);
                }
            }
        }
    }
}
