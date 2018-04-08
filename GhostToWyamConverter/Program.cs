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
            // args[2] = @"C:\output\drafts";

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Input filename and output location parameters are required");
                return;
            }

            Directory.CreateDirectory(args[1]);
            Directory.CreateDirectory(args[2]);
            foreach (var item in Directory.GetFiles(args[1]).Concat(Directory.GetFiles(args[2])))
            {
                File.Delete(item);
            }

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
                    string slug = post.Value<string>("slug");
                    var published = post.Value<string>("published_at");
                    var filePath = string.IsNullOrEmpty(published) ? args[2] : args[1];

                    var filename = Path.Combine(filePath, slug + ".md");

                    Console.WriteLine("Creating " + filename);

                    using (var mdFile = File.CreateText(filename))
                    {
                        var postId = post.Value<int>("id");
                        mdFile.WriteLine("---");
                        mdFile.Write("title: ");
                        mdFile.WriteLine(Escape(post.Value<string>("title")));

                        if (!string.IsNullOrEmpty(published))
                        {
                            var pubDate = DateTimeOffset.Parse(published);
                            mdFile.WriteLine("published_at: {0:O}", pubDate.ToString("dd MMM yy HH:mm"));
                        }

                        var updated = post.Value<string>("updated_at");
                        if (!string.IsNullOrEmpty(published))
                        {
                            var pubDate = DateTimeOffset.Parse(published);
                            mdFile.WriteLine("updated_at: {0:O}", pubDate.ToString("dd MMM yy HH:mm"));
                        }

                        mdFile.Write("image: ");
                        mdFile.WriteLine(post.Value<string>("image"));

                        if (postTags.ContainsKey(postId) && postTags[postId].Any())
                        {
                            mdFile.Write("tags: ");
                            mdFile.WriteLine(string.Join(", ", postTags[postId].Select(x => tags[x])));
                        }
                        string excerpt = Escape(post.Value<string>("meta_description"));
                        if(!string.IsNullOrEmpty(excerpt))
                        {
                            mdFile.Write("excerpt: ");
                            mdFile.WriteLine(excerpt);
                        }
                        
                        mdFile.WriteLine("---");
                        string markdown = post.Value<string>("markdown");
                        mdFile.Write(markdown.Replace(@"```language-", @"```").Replace(@"``` language-", @"```"));
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
