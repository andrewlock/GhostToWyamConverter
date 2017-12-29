using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GhostToWyamConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example
            // args[0] = @"C:\input\andrew-lock-net-escapades.ghost.json";
            // args[1] = @"C:\output\posts";

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Input filename and output location parameters are required");
                return;
            }

            Directory.CreateDirectory(args[1]);

            using (var stream = File.OpenRead(args[0]))
            using (var txt = new StreamReader(stream))
            using (var reader = new JsonTextReader(txt))
            {
                var jObject = JObject.Load(reader);

                // build tag list.
                var tags = jObject["db"].First["data"]["tags"].ToDictionary(tag => tag.Value<int>("id"), tag => tag.Value<string>("name"));

                var postTags = new Dictionary<int, List<int>>();
                foreach (var token in jObject["db"].First["data"]["posts_tags"])
                {
                    var postId = token.Value<int>("post_id");
                    var tagId = token.Value<int>("tag_id");
                    if (!postTags.ContainsKey(postId))
                    {
                        postTags.Add(postId, new List<int>());
                    }

                    postTags[postId].Add(tagId);
                }


                foreach (JToken post in jObject["db"].First["data"]["posts"])
                {
                    var filename = Path.Combine(args[1], post.Value<string>("slug") + ".md");
                    Console.WriteLine("Creating " + filename);
                    using (var mdFile = File.CreateText(filename))
                    {
                        var postId = post.Value<int>("id");
                        mdFile.Write("Title: ");
                        mdFile.WriteLine(Escape(post.Value<string>("title")));

                        var published = post.Value<string>("published_at");
                        if (!string.IsNullOrEmpty(published))
                        {
                            var pubDate = DateTimeOffset.Parse(published);
                            mdFile.WriteLine("Published: {0:O}", pubDate.ToString("dd MMM yy HH:mm"));
                        }

                        mdFile.Write("Image: ");
                        mdFile.WriteLine(post.Value<string>("image"));

                        if (postTags.ContainsKey(postId) && postTags[postId].Any())
                        {
                            mdFile.WriteLine("Tags: ");
                            foreach (var tagId in postTags[postId])
                            {
                                mdFile.WriteLine("- {0}", tags[tagId]);
                            }
                        }
                        mdFile.Write("Lead: ");
                        mdFile.WriteLine(Escape(post.Value<string>("meta_description")));


                        mdFile.WriteLine("---");
                        string markdown = post.Value<string>("markdown");
                        mdFile.Write(markdown.Replace(@"```language-", @"```"));
                        mdFile.Write(markdown.Replace(@"``` language-", @"```"));
                    }
                }
            }

#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
        }

        static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) { return value; }
            if (value.Contains(":"))
            {
                return $@"""{value}""";
            }
            return value;
        }
    }
}
